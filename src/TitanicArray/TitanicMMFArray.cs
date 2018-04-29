using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HugeStructures.TitanicArray
{
	public class TitanicMMFArray<T> : ITitanicArray<T>, IDisposable
	{
		public TitanicMMFArray() : this(TitanicArrayConfig<T>.Default)
		{
		}

		public TitanicMMFArray(ITitanicArrayConfig<T> config)
		{
			this.config = config;
			TSize = GetSizeInBytes(config.DataSerializer);
			Length = config.Capacity;
			var stream = File.Open(
				config.BackingStoreFileName,
				FileMode.Create,
				FileAccess.ReadWrite,
				FileShare.Read
			);
			
			#if MONO
			//TODO try stream.SetLength(config.Capacity * TSize);
			for(long b=0; b<config.Capacity * TSize; b++) {
				stream.WriteByte(0);
			}
			stream.Seek(0,SeekOrigin.Begin);
			#endif

			mmf = MemoryMappedFile.CreateFromFile(
				stream,
				null,
				config.Capacity * TSize,
				MemoryMappedFileAccess.ReadWrite,
				HandleInheritability.None,
				false
			);
			mmva = mmf.CreateViewAccessor();
		}

		public long Length { get; private set; }

		public T this[long index] {
			get {
				if (index < 0 || index >= Length) {
					throw new IndexOutOfRangeException();
				}

				byte[] buff = new byte[TSize];
				mmva.ReadArray(index * TSize, buff, 0, TSize);
				T val = config.DataSerializer.Deserialize(buff);
				return val;
			}
			set {
				if (index < 0 || index - 1 > Length) {
					throw new IndexOutOfRangeException();
				}
				byte[] buff = config.DataSerializer.Serialize(value);
				mmva.WriteArray(index * TSize,buff,0,TSize);
			}
		}

		static int GetSizeInBytes(IDataSerializer<T> ser)
		{
			var data = ser.Serialize(default(T));
			return data.Length;
		}

		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				mmva.Dispose();
				mmf.Dispose();
			}
			if (config.IsTemporary) {
				RemoveFile(config.BackingStoreFileName);
			}
		}

		static void RemoveFile(string name)
		{
			try {
				if (File.Exists(name)) {
					//Debug.WriteLine("deleting "+config.BackingStoreFileName);
					File.Delete(name);
				}
			} catch (UnauthorizedAccessException e) {
				Trace.WriteLine(e.ToString());
			}
		}

		MemoryMappedViewAccessor mmva = null;
		MemoryMappedFile mmf = null;
		int TSize = -1;
		ITitanicArrayConfig<T> config = null;
	}
}

using RaptorDB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HugeStructures.TitanicArray
{
	public class TitanicRaptorDBArray<T> : ITitanicArray<T>
	{
		public TitanicRaptorDBArray() : this(TitanicArrayConfig<T>.Default)
		{
		}

		public TitanicRaptorDBArray(ITitanicArrayConfig<T> config)
		{
			this.config = config;
			TSize = GetSizeInBytes(config.DataSerializer);
			rdb = RaptorDB<long>.Open(config.BackingStoreFileName,false);
		}

		public T this[long index] {
			get {
				if (rdb.Get(index,out byte[] buff)) {
					T data = config.DataSerializer.Deserialize(buff);
					return data;
				} else {
					return default(T);
				}
			}
			set {
				byte[] data = config.DataSerializer.Serialize(value);
				rdb.Set(index,data);
			}
		}

		public long Length { get {
			return config.Capacity;
		}}

		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				rdb.Dispose();
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

		static int GetSizeInBytes(IDataSerializer<T> ser)
		{
			var data = ser.Serialize(default(T));
			return data.Length;
		}

		byte[] buff;
		int TSize;
		ITitanicArrayConfig<T> config;
		RaptorDB<long> rdb;
	}
}

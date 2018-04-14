using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HugeStructures
{
	public class TitanicFileArray<T> : ITitanicArray<T>, IDisposable
	{
		public TitanicFileArray() : this(TitanicArrayConfig<T>.Default)
		{
		}

		public TitanicFileArray(ITitanicArrayConfig<T> config, int cacheSize = 2000)
		{
			this.config = config;
			store = File.Open(config.BackingStoreFileName,FileMode.Create,FileAccess.ReadWrite,FileShare.Read);
			itemSize = GetSizeInBytes(config.DataSerializer);
			buffer = new byte[itemSize];
			InitializeFile();
			cache = new LRUCache<long,T>(cacheSize);
		}
	
		public T this[long index] {
			get {
				if (cache.TryGet(index,out T val)) {
					return val;
				}

				buffer.Initialize();
				store.Seek(index * itemSize,SeekOrigin.Begin);
				store.Read(buffer,0,itemSize);
				return config.DataSerializer.Deserialize(buffer);
			}
			set {
				KeyValuePair<long,T> evicted;
				if (cache.AddOrUpdate(index,value,out evicted))
				{
					byte[] buff = config.DataSerializer.Serialize(evicted.Value);
					store.Seek(evicted.Key * itemSize,SeekOrigin.Begin);
					store.Write(buff,0,itemSize);
				}
			}
		}

		public long Length { get {
			return store.Length / itemSize;
		}}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing) {
				if (store != null) {
					store.Dispose();
				}

				try {
					if (File.Exists(config.BackingStoreFileName)) {
						//Debug.WriteLine("deleting "+config.BackingStoreFileName);
						File.Delete(config.BackingStoreFileName);
					}
				} catch (UnauthorizedAccessException e) {
					Trace.WriteLine(e.ToString());
				}
			}
		}

		void InitializeFile()
		{
			store.Seek(0,SeekOrigin.Begin);
			long len = config.Capacity * itemSize;
			for(long i=0; i<len; i++) {
				store.WriteByte(0);
			}
		}

		static int GetSizeInBytes(IDataSerializer<T> ser)
		{
			var data = ser.Serialize(default(T));
			return data.Length;
		}

		FileStream store;
		int itemSize;
		byte[] buffer;
		ITitanicArrayConfig<T> config;
		LRUCache<long,T> cache;
	}
}

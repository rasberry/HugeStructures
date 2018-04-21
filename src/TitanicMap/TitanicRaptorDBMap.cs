using RaptorDB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HugeStructures.TitanicMap
{
	public class TitanicRaptorDBMap<K, V> : ITitanicMap<K, V> where K : IComparable<K>
	{
		public TitanicRaptorDBMap() : this(TitanicMapConfig<K,V>.Default)
		{
		}

		public TitanicRaptorDBMap(ITitanicMapConfig<K,V> config)
		{
			this.config = config;
			rdb = RaptorDB<K>.Open(config.BackingStoreFileName,false);
		}

		public V this[K key] {
			get {
				if (!TryGetValue(key,out V val)) {
					throw new KeyNotFoundException();
				}
				return val;
			}
			set {
				byte[] data = config.ValueSerializer.Serialize(value);
				rdb.Set(key,data);
			}
		}

		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
			var en = rdb.Enumerate(default(K));
			foreach(var sd in en) {
				int rec = sd.Value;
				byte[] data = rdb.FetchRecordBytes(rec);
				V val = config.ValueSerializer.Deserialize(data);
				yield return new KeyValuePair<K, V>(sd.Key,val);
			}
		}

		public bool TryGetValue(K key, out V value)
		{
			value = default(V);
			bool hasData = rdb.Get(key,out byte[] data);
			if (hasData) {
				value = config.ValueSerializer.Deserialize(data);
			}
			return hasData;
		}

		public bool TryRemove(K key)
		{
			return rdb.RemoveKey(key);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing) {
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

		static int GetSizeInBytes(IDataSerializer<K> ser)
		{
			var data = ser.Serialize(default(K));
			return data.Length;
		}

		ITitanicMapConfig<K,V> config;
		RaptorDB<K> rdb;

	}
}

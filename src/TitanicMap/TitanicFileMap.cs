using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HugeStructures.TitanicMap
{
	#if false
	public class TitanicFileMap<K, V> : ITitanicMap<K, V>
	{
		public TitanicFileMap() : this(TitanicMapConfig<K,V>.Default)
		{
		}

		public TitanicFileMap(ITitanicMapConfig<K,V> config)
		{
			this.config = config;

			keyStore = File.Open(config.BackingStoreFileName+".k"
				,FileMode.Create,FileAccess.ReadWrite,FileShare.Read);
			valueStore = File.Open(config.BackingStoreFileName+".v"
				,FileMode.Create,FileAccess.ReadWrite,FileShare.Read);

			//cache = new LRUCache<long,T>(cacheSize);
		}

		public V this[K key] {
			get {
				
			}
			set {
				
			}
		}

		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public bool TryGetValue(K key, out V value)
		{
			throw new NotImplementedException();
		}

		public bool TryRemove(K key)
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				// TODO: dispose managed state (managed objects).
			}

			// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
			// TODO: set large fields to null.
		}

		public void Dispose()
		{
			Dispose(true);
		}

		ITitanicMapConfig<K,V> config;
		FileStream valueStore;
		FileStream keyStore;
		byte[] keyBuffer;
		byte[] valueBuffer;
		//LRUCache<long,V> cache;
	}
	#endif
}

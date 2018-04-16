using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HugeStructures.TitanicMap
{
	public class TitanicFileMap<K, V> : ITitanicMap<K, V>
	{
		//public TitanicFileMap() : this(TitanicMapConfig.Default)
		//{
		//}

		//public TitanicFileMap(ITitanicMapConfig

		public V this[K key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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
	}
}

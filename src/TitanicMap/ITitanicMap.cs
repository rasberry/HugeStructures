using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HugeStructures.TitanicMap
{
	public interface ITitanicMap<K,V> : IEnumerable<KeyValuePair<K,V>>, IDisposable
	{
		V this[K key] { get; set; }
		bool TryGetValue(K key, out V value);
		bool TryRemove(K key);
	}
}

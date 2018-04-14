using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HugeStructures
{
	//https://stackoverflow.com/questions/754233/is-it-there-any-lru-implementation-of-idictionary
	public class LRUCache<K,V> : IEnumerable<KeyValuePair<K,V>>
	{
		private int capacity;
		//map of keys to nodes
		private Dictionary<K, LinkedListNode<LRUCacheItem<K, V>>> cacheMap =
			new Dictionary<K, LinkedListNode<LRUCacheItem<K, V>>>();
		//LRU item list
		private LinkedList<LRUCacheItem<K, V>> lruList = new LinkedList<LRUCacheItem<K, V>>();

		public LRUCache(int capacity)
		{
			this.capacity = capacity;
		}

		public V Get(K key)
		{
			V val;
			if (TryGet(key,out val))
			{
				return val;
			}
			return default(V);
		}

		public bool TryGet(K key, out V val)
		{
			LinkedListNode<LRUCacheItem<K, V>> node;
			if (cacheMap.TryGetValue(key, out node))
			{
				V value = node.Value.value;
				lruList.Remove(node);
				lruList.AddLast(node);
				val = value;
				return true;
			}
			val = default(V);
			return false;
		}

		//returns true if something was evicted
		public bool Add(K key, V val, out KeyValuePair<K,V> evicted)
		{
			bool isMore = false;
			isMore = cacheMap.Count >= capacity;

			if (isMore)
			{
				LinkedListNode<LRUCacheItem<K, V>> rmnode = lruList.First;

				// Remove from LRUPriority
				lruList.RemoveFirst();

				// Remove from cache
				cacheMap.Remove(rmnode.Value.key);

				evicted = new KeyValuePair<K, V>(rmnode.Value.key,rmnode.Value.value);
			}
			else
			{
				evicted = default(KeyValuePair<K,V>);
			}

			var cacheItem = new LRUCacheItem<K, V>(key, val);
			var node = new LinkedListNode<LRUCacheItem<K, V>>(cacheItem);

			lruList.AddLast(node);
			cacheMap.Add(key, node);
			return isMore;
		}

		public bool AddOrUpdate(K key, V val, out KeyValuePair<K,V> evicted)
		{
			LinkedListNode<LRUCacheItem<K, V>> node;
			bool isCached = false;
			if (isCached = cacheMap.TryGetValue(key, out node))
			{
				node.Value.value = val;
			}

			if (!isCached)
			{
				return Add(key,val, out evicted);
			}
			else
			{
				evicted = default(KeyValuePair<K,V>);
				return false;
			}
		}

		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
			foreach(var kvp in cacheMap) {
				yield return new KeyValuePair<K, V>(kvp.Key,kvp.Value.Value.value);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	class LRUCacheItem<K,V>
	{
		public LRUCacheItem(K k, V v)
		{
			key = k;
			value = v;
		}
		public K key;
		public V value;
	}
}

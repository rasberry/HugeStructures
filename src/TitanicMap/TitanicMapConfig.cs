using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HugeStructures.TitanicMap
{
	public interface ITitanicMapConfig<K,V>
	{
		string BackingStoreFileName { get; }
		IDataKeyHash<K> KeyHasher { get; }
		IDataSerializer<V> ValueSerializer { get; }
		bool IsTemporary { get; }
	}
	
	public class TitanicMapConfig<K,V> : ITitanicMapConfig<K,V>
	{
		public string BackingStoreFileName { get; set; }
		public IDataKeyHash<K> KeyHasher { get; set; }
		public IDataSerializer<V> ValueSerializer { get; set; }
		public bool IsTemporary { get; set; }
	
		public static TitanicMapConfig<K,V> Default { get {
			string fn = Guid.NewGuid().ToString("n");
			return new TitanicMapConfig<K,V> {
				BackingStoreFileName = Path.Combine(Path.GetTempPath(),fn),
				KeyHasher = new DefaultDataKeyHash<K>(),
				ValueSerializer = new DefaultDataSerializer<V>(),
				IsTemporary = false
			};
		} }
	}
}

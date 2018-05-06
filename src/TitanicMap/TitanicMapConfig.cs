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
		IDataKeySerializer<K> KeySerializer { get; }
		IDataSerializer<V> ValueSerializer { get; }
		bool IsTemporary { get; }
	}
	
	public class TitanicMapConfig<K,V> : ITitanicMapConfig<K,V>
	{
		public string BackingStoreFileName { get; set; }
		public IDataKeySerializer<K> KeySerializer { get; set; }
		public IDataSerializer<V> ValueSerializer { get; set; }
		public bool IsTemporary { get; set; }
	
		public static TitanicMapConfig<K,V> Default { get {
			string fn = Guid.NewGuid().ToString("n");
			return new TitanicMapConfig<K,V> {
				BackingStoreFileName = Path.Combine(Path.GetTempPath(),fn),
				KeySerializer = new DefaultDataKeySerializer<K>(),
				ValueSerializer = new DefaultDataSerializer<V>(),
				IsTemporary = false
			};
		} }
	}
}

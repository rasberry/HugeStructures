using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HugeStructures.TitanicMap
{
	//public interface ITitanicMapConfig<K,V>
	//{
	//	string BackingStoreFileName { get; }
	//	IDataSerializer<K,V> DataSerializer { get; }
	//}
	//
	//public class TitanicMapConfig<K,V> : ITitanicMapConfig<K,V>
	//{
	//	public string BackingStoreFileName { get; set; }
	//	public IDataSerializer<T> DataSerializer { get; set; }
	//
	//	public static TitanicMapConfig<K,V> Default { get {
	//		string fn = Guid.NewGuid().ToString("n");
	//		return new TitanicMapConfig<K,V> {
	//			BackingStoreFileName = Path.Combine(Path.GetTempPath(),fn),
	//			DataSerializer = new DefaultDataSerializer<K,V>()
	//		};
	//	} }
	//}
}

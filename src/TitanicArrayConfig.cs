using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HugeStructures
{
	public interface ITitanicArrayConfig
	{
		string BackingStoreFileName { get; }
		long Capacity { get; }
		IDataSerializer DataSerializer { get; }
	}

	public class TitanicArrayConfig : ITitanicArrayConfig
	{
		public string BackingStoreFileName { get; set; }
		public long Capacity { get; set; }
		public IDataSerializer DataSerializer { get; set; }

		public static TitanicArrayConfig Default { get {
			string fn = Guid.NewGuid().ToString("n");
			return new TitanicArrayConfig {
				BackingStoreFileName = Path.Combine(Path.GetTempPath(),fn),
				Capacity = 16,
				DataSerializer = new DefaultDataSerializer()
			};
		} }
	}
}

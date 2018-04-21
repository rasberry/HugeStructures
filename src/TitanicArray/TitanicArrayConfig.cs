using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HugeStructures.TitanicArray
{
	public interface ITitanicArrayConfig<T>
	{
		string BackingStoreFileName { get; }
		long Capacity { get; }
		IDataSerializer<T> DataSerializer { get; }
		bool IsTemporary { get; set; }
	}

	public class TitanicArrayConfig<T> : ITitanicArrayConfig<T>
	{
		public string BackingStoreFileName { get; set; }
		public long Capacity { get; set; }
		public IDataSerializer<T> DataSerializer { get; set; }
		public bool IsTemporary { get; set; }

		public static TitanicArrayConfig<T> Default { get {
			string fn = Guid.NewGuid().ToString("n");
			return new TitanicArrayConfig<T> {
				BackingStoreFileName = Path.Combine(Path.GetTempPath(),fn),
				Capacity = 16,
				DataSerializer = new DefaultDataSerializer<T>(),
				IsTemporary = false
			};
		} }
	}
}

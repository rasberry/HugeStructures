using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace HugeStructures
{
	public interface IDataSerializer
	{
		byte[] Serialize<T>(T item);
		T Deserialize<T>(byte[] bytes);
	}

	public class DefaultDataSerializer : IDataSerializer
	{
		readonly BinaryFormatter _formatter = new BinaryFormatter();

		public T Deserialize<T>(byte[] bytes)
		{
			var mem = new MemoryStream(bytes);
			object o = _formatter.Deserialize(mem);
			return (T)o;
		}

		public byte[] Serialize<T>(T item)
		{
			MemoryStream mem = new MemoryStream();
			_formatter.Serialize(mem, item);
			mem.Position = 0;
			return mem.ToArray();
		}
	}
}

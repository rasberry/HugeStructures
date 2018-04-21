using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace HugeStructures
{
	public interface IDataSerializer<T>
	{
		byte[] Serialize(T item);
		T Deserialize(byte[] bytes);
	}

	public class DefaultDataSerializer<T> : IDataSerializer<T>
	{
		readonly BinaryFormatter _formatter = new BinaryFormatter();

		public T Deserialize(byte[] bytes)
		{
			var mem = new MemoryStream(bytes);
			object o = _formatter.Deserialize(mem);
			return (T)o;
		}

		public byte[] Serialize(T item)
		{
			MemoryStream mem = new MemoryStream();
			_formatter.Serialize(mem, item);
			mem.Position = 0;
			return mem.ToArray();
		}
	}

	public interface IDataKeyHash<T>
	{
		long GetKeyHash(T item);
	}

	public class DefaultDataKeyHash<T> : IDataKeyHash<T>
	{
		public long GetKeyHash(T item)
		{
			var eq = EqualityComparer<T>.Default;
			return eq.GetHashCode(item);
		}
	}
}

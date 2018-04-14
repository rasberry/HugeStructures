using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HugeStructures.Test
{
	public class CustomByteSerializer : IDataSerializer<byte>
	{
		public byte Deserialize(byte[] bytes)
		{
			return bytes[0];
		}

		public byte[] Serialize(byte item)
		{
			return new byte[] { item };
		}
	}

	public class CustomDoubleSerializer : IDataSerializer<double>
	{
		public double Deserialize(byte[] bytes)
		{
			return BitConverter.ToDouble(bytes,0);
		}

		public byte[] Serialize(double item)
		{
			return BitConverter.GetBytes(item);
		}
	}
}

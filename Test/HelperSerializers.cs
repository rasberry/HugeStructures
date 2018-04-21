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
			double d = BitConverter.ToDouble(bytes,0);
			Console.WriteLine("des "+BitConverter.ToString(bytes,0)+"\t"+d);
			return d;
		}

		public byte[] Serialize(double item)
		{
			byte[] b = BitConverter.GetBytes(item);
			Console.WriteLine("ser "+BitConverter.ToString(b,0)+"\t"+item);
			return b;
		}
	}
}

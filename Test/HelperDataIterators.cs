using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HugeStructures.Test
{
	public interface IDataIterator<T>
	{
		void Reset();
		T GetNext();
		bool AreEqual(T a, T b);
	}

	public class ByteDataIterator : IDataIterator<byte>
	{
		public void Reset() { 
			_curr = 0;
		}
		public byte GetNext() {
			return _curr++;
		}
		public bool AreEqual(byte a, byte b) {
			return a == b;
		}
		byte _curr = 0;
	}
	public class ByteRandomIterator : IDataIterator<byte>
	{
		public void Reset() {
			rnd = new Random(0);
		}
		public byte GetNext() {
			return (byte)(rnd.Next() % 255);
		}
		public bool AreEqual(byte a, byte b) {
			return a == b;
		}

		long _curr = 0;
		Random rnd = new Random(0);
	}

	public class DoubleDataIterator : IDataIterator<double>
	{
		public void Reset() {
			_curr = 0;
		}
		public double GetNext() {
			return BitConverter.Int64BitsToDouble(_curr++);
		}
		public bool AreEqual(double a, double b) {
			return a == b;
		}
		long _curr = 0;
	}

	public class DoubleRandomIterator : IDataIterator<double>
	{
		public void Reset() {
			rnd = new Random(0);
		}
		public double GetNext() {
			return rnd.NextDouble();
		}
		public bool AreEqual(double a, double b) {
			return a == b;
		}

		long _curr = 0;
		Random rnd = new Random(0);
	}
}

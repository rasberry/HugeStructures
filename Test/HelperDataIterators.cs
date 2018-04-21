using System;
using System.Collections.Generic;
using System.Diagnostics;
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
			if (a == b) {
				return true;
			} else {
				Debug.WriteLine("bd Didn't match\t"+a+"\t"+b);
				return false;
			}
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
			if (a == b) {
				return true;
			} else {
				Debug.WriteLine("br Didn't match\t"+a+"\t"+b);
				return false;
			}
		}
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
			if (a == b) {
				return true;
			} else {
				Debug.WriteLine("dd Didn't match\t"+a+"\t"+b);
				return false;
			}
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
			if (a == b) {
				return true;
			} else {
				Debug.WriteLine("dr Didn't match\t"+a+"\t"+b);
				return true;
			}
		}
		Random rnd = new Random(0);
	}
}

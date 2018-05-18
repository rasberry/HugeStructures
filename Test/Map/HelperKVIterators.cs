using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HugeStructures.Test
{
	public interface IKVIterator<K,V>
	{
		void Reset();
		KeyValuePair<K,V> GetNext();
		bool AreEqual(K ka, V va, K kb, V vb);
		long Length { get; }
	}

	public class LongDoubleIterator : IKVIterator<long,double>
	{
		public LongDoubleIterator(long len)
		{
			Length = len;
		}
		public void Reset()
		{
			_curr = 0;
		}

		public KeyValuePair<long,double> GetNext()
		{
			long k = _curr;
			double v = BitConverter.Int64BitsToDouble(Length - _curr);
			_curr++;
			return new KeyValuePair<long, double>(k,v);
		}

		public bool AreEqual(long ka, double va, long kb, double vb)
		{
			return ka == kb && va == vb;
		}

		public long Length { get; private set; }
		long _curr = 0;
	}

	public class LongDoubleIteratorRandom : IKVIterator<long,double>
	{
		public LongDoubleIteratorRandom(long len)
		{
			Length = len;
			Reset();
		}
		public void Reset()
		{
			seq = LinearFeedbackShiftRegister.SequenceLength(
				TitanicMapHelpers.lfsrStart,(ulong)Length).GetEnumerator();
		}
		public KeyValuePair<long,double> GetNext()
		{
			seq.MoveNext();
			long k = (long)seq.Current;
			double v = BitConverter.Int64BitsToDouble(Length - k);
			return new KeyValuePair<long, double>(k,v);
		}
		public bool AreEqual(long ka, double va, long kb, double vb)
		{
			return ka == kb && va == vb;
		}
		public long Length { get; set; }
		IEnumerator<ulong> seq;
	}
}
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
		public void Reset()
		{
		}

		public KeyValuePair<long,double> GetNext()
		{
			return new KeyValuePair<long, double>(0,0);
		}

		public bool AreEqual(long ka, double va, long kb, double vb)
		{
			return true;
		}

		public long Length { get { return 0; }}
	}
}
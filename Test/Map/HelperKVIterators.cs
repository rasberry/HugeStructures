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
		K GetNextKey();
		V GetNextValue();
		bool AreEqual(K ka, V va, K kb, V vb);
	}
}
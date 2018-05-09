using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HugeStructures.TitanicMap;

namespace HugeStructures.Test
{
	public static class TitanicMapHelpers
	{
		const ulong lfsrStart = 1301L;

		public static void ReadWriteTest<K,V>(ITitanicMap<K,V> map, IKVIterator<K,V> iter)
		{
			iter.Reset();
			long len = iter.Length;
			while(0 < len--) {
				var kv = iter.GetNext();
				map[kv.Key] = kv.Value;
			}
			iter.Reset();
			len = iter.Length;
			while(0 < len--) {
				var kv = iter.GetNext();
				V val = map[kv.Key];
				Assert.IsTrue(iter.AreEqual(kv.Key,kv.Value,kv.Key,val));
			}
		}
	}
}

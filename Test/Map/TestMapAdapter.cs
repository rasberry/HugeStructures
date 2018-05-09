using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using HugeStructures.TitanicMap;

namespace HugeStructures.Test
{
	public abstract class TestMapAdapter<K,V>
	{
		public abstract ITitanicMap<K,V> CreateMap(ITitanicMapConfig<K,V> c = null);
		public abstract IKVIterator<K,V> CreateIterator();

		[TestMethod]
		public void DefaultSerializer()
		{
			using(var map = CreateMap()) {
				TitanicMapHelpers.ReadWriteTest(map,CreateIterator());
			}
		}

		static Random rnd = new Random();
		public static string GetLocalTempFileName()
		{
			string name = Guid.NewGuid().ToString("n") + rnd.Next();
			return Path.Combine(Environment.CurrentDirectory,name);
		}
	}
}
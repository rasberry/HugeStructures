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
		public abstract ITitanicMap<K,V> CreateMap();

		[TestMethod]
		public void DefaultSerializer()
		{
			using(var map = CreateMap()) {
				//TitanicArrayHelpers.ReadWriteTest(arr,CreateIterator());
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
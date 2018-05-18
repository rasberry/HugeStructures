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
		public abstract IKVIterator<K,V> CreateIterator(bool rnd = false);
		public abstract void CreateSerializers(out IDataKeySerializer<K> keyser, out IDataSerializer<V> valser);

		[TestMethod]
		public void DefaultSerializer()
		{
			using(var map = CreateMap()) {
				TitanicMapHelpers.ReadWriteTest(map,CreateIterator());
			}
		}

		[TestMethod]
		public void CustomSerializer(bool rnd = false)
		{
			var c = TitanicMapConfig<K,V>.Default;
			CreateSerializers(out var keyser, out var valser);
			c.KeySerializer = keyser;
			c.ValueSerializer = valser;
			c.BackingStoreFileName = TitanicArrayHelpers.GetLocalTempFileName();

			using(var map = CreateMap(c)) {
				TitanicMapHelpers.ReadWriteTest(map,CreateIterator(rnd));
			}
		}

		[TestMethod]
		public void CustomSerializerRandom()
		{
			CustomSerializer(true);
		}

		static Random rnd = new Random();
		public static string GetLocalTempFileName()
		{
			string name = Guid.NewGuid().ToString("n") + rnd.Next();
			return Path.Combine(Environment.CurrentDirectory,name);
		}
	}
}
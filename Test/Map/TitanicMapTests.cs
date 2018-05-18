

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
	[TestClass]
	public class TitanicRaptorDBMapTests : TestMapAdapter<long,double>
	{
		const long IterLength = 1 * 1024;

		public override ITitanicMap<long,double> CreateMap(ITitanicMapConfig<long,double> c = null)
		{
			return c == null
				? new TitanicRaptorDBMap<long,double>()
				: new TitanicRaptorDBMap<long,double>(c)
			;
		}
		public override IKVIterator<long,double> CreateIterator(bool rnd = false) {
			return rnd
				? (IKVIterator<long,double>)new LongDoubleIteratorRandom(IterLength)
				: (IKVIterator<long,double>)new LongDoubleIterator(IterLength)
			;
		}
		public override void CreateSerializers(out IDataKeySerializer<long> keyser, out IDataSerializer<double> valser) {
			valser = new CustomDoubleSerializer();
			keyser = new CustomLongSerializer();
		}
	}
}
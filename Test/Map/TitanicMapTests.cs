

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
	public class TitanicZipMapTests : TestMapAdapter<long,double>
	{
		public override ITitanicMap<long,double> CreateMap(ITitanicMapConfig<long,double> c = null)
		{
			return c == null
				? new TitanicZipMap<long,double>()
				: new TitanicZipMap<long,double>(c)
			;
		}
		public override IKVIterator<long,double> CreateIterator() {
			return new LongDoubleIterator();
		}

		#if false
		public override ITitanicArray<double> CreateArray(ITitanicArrayConfig<double> c = null)
		{
			return c == null
				? new TitanicFileArray<double>()
				: new TitanicFileArray<double>(c)
			;
		}
		public override IDataIterator<double> CreateIterator() {
			return new DoubleDataIterator();
		}
		public override IDataSerializer<double> CreateSerializer() {
			return new CustomDoubleSerializer();
		}
		#endif
	}
}
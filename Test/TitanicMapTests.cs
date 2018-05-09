

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HugeStructures.TitanicArray;

namespace HugeStructures.Test
{
	#if false
	[TestClass]
	public class TitanicFileArrayTests : TestMapAdapter<double>
	{
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
	}
	#endif
}
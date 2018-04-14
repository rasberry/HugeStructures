using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HugeStructures.Test
{
	[TestClass]
	public class TitanicFileArrayTests : TestAdapter<double>
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
	[TestClass]
	public class TitanicFileArrayRandom : TitanicFileArrayTests
	{
		public override IDataIterator<double> CreateIterator() {
			return new DoubleRandomIterator();
		}
	}

	[TestClass]
	public class TitanicSQLiteArrayTests : TestAdapter<double>
	{
		public override ITitanicArray<double> CreateArray(ITitanicArrayConfig<double> c = null)
		{
			return c == null
				? new TitanicSQLiteArray<double>()
				: new TitanicSQLiteArray<double>(c)
			;
		}
		public override IDataIterator<double> CreateIterator() {
			return new DoubleDataIterator();
		}
		public override IDataSerializer<double> CreateSerializer() {
			return new CustomDoubleSerializer();
		}
	}
	[TestClass]
	public class TitanicSQLiteArrayRandom : TitanicSQLiteArrayTests
	{
		public override IDataIterator<double> CreateIterator() {
			return new DoubleRandomIterator();
		}
	}

	//[TestClass]
	//public class TitanicLiteDBArrayTests : TestAdapter<double>
	//{
	//	public override ITitanicArray<double> CreateArray(ITitanicArrayConfig<double> c = null)
	//	{
	//		return c == null
	//			? new TitanicLiteDBArray<double>()
	//			: new TitanicLiteDBArray<double>(c)
	//		;
	//	}
	//	public override IDataIterator<double> CreateIterator() {
	//		return new DoubleDataIterator();
	//	}
	//	public override IDataSerializer<double> CreateSerializer() {
	//		return new CustomDoubleSerializer();
	//	}
	//}
	//[TestClass]
	//public class TitanicLiteDBArrayRandom : TitanicLiteDBArrayTests
	//{
	//	public override IDataIterator<double> CreateIterator() {
	//		return new DoubleRandomIterator();
	//	}
	//}

	//[TestClass]
	//public class TitanicMMFArrayByteTests : TestAdapter<byte>
	//{
	//	public override ITitanicArray<byte> CreateArray(ITitanicArrayConfig<byte> c = null)
	//	{
	//		return c == null
	//			? new TitanicMMFArray<byte>()
	//			: new TitanicMMFArray<byte>(c)
	//		;
	//	}
	//	public override IDataIterator<byte> CreateIterator() {
	//		return new ByteDataIterator();
	//	}
	//	public override IDataSerializer<byte> CreateSerializer() {
	//		return new CustomByteSerializer();
	//	}
	//}
	//[TestClass]
	//public class TitanicMMFArrayByteRandom : TitanicMMFArrayByteTests
	//{
	//	public override IDataIterator<byte> CreateIterator() {
	//		return new ByteRandomIterator();
	//	}
	//}
	
	[TestClass]
	public class TitanicMMFArrayDoubleTests : TestAdapter<double>
	{
		public override ITitanicArray<double> CreateArray(ITitanicArrayConfig<double> c = null)
		{
			return c == null
				? new TitanicMMFArray<double>()
				: new TitanicMMFArray<double>(c)
			;
		}
		public override IDataIterator<double> CreateIterator() {
			return new DoubleDataIterator();
		}
		public override IDataSerializer<double> CreateSerializer() {
			return new CustomDoubleSerializer();
		}
	}

	[TestClass]
	public class TitanicMMFArrayDoubleRandom : TitanicMMFArrayDoubleTests
	{
		public override IDataIterator<double> CreateIterator() {
			return new DoubleRandomIterator();
		}
	}
}

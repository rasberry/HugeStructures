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
	//[TestClass]
	//public class TitanicFileArrayTests : TestAdapter
	//{
	//	public override ITitanicArray<byte> CreateArray(ITitanicArrayConfig c = null)
	//	{
	//		return c == null
	//			? new TitanicFileArray<byte>()
	//			: new TitanicFileArray<byte>(c)
	//		;
	//	}
	//}

	[TestClass]
	public class TitanicMMFArrayTests : TestAdapter
	{
		public override ITitanicArray<byte> CreateArray(ITitanicArrayConfig c = null)
		{
			return c == null
				? new TitanicMMFArray<byte>()
				: new TitanicMMFArray<byte>(c)
			;
		}
	}

	//[TestClass]
	//public class TitanicSQLiteArrayTests : TestAdapter
	//{
	//	public override ITitanicArray<byte> CreateArray(ITitanicArrayConfig c = null)
	//	{
	//		return c == null
	//			? new TitanicSQLiteArray<byte>()
	//			: new TitanicSQLiteArray<byte>(c)
	//		;
	//	}
	//}

	//[TestClass]
	//public class TitanicLiteDBArrayTests : TestAdapter
	//{
	//	public override ITitanicArray<byte> CreateArray(ITitanicArrayConfig c = null)
	//	{
	//		return c == null
	//			? new TitanicLiteDBArray<byte>()
	//			: new TitanicLiteDBArray<byte>(c)
	//		;
	//	}
	//}
}

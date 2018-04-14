﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HugeStructures.Test
{
	public abstract class TestAdapter
	{
		public abstract ITitanicArray<byte> CreateArray(ITitanicArrayConfig c = null);

		[TestMethod]
		public void DefaultSerializer()
		{
			using(var arr = CreateArray()) {
				Helpers.ReadWriteTest(arr);
			}
		}

		[TestMethod]
		public void CustomSerializer()
		{
			var c = TitanicArrayConfig.Default;
			c.DataSerializer = new CustomByteSerializer();
			c.BackingStoreFileName = Helpers.GetLocalTempFileName();

			using(var arr = CreateArray(c)) {
				Helpers.ReadWriteTest(arr);
			}
		}

		[TestMethod]
		public void LotsOfData()
		{
			using(var arr = CreateArray(Helpers.LotsOfDataConfig)) {
				Helpers.ReadWriteTest(arr);
			}
		}

		[TestMethod]
		public void TimingTest()
		{
			using(var arr = CreateArray(Helpers.TimingConfig)) {
				Helpers.TimingTest(arr);
			}
		}

		//[TestMethod]
		//public void TimingTestNormalFile()
		//{
		//	var ds = new CustomByteSerializer();
		//	var fn = Path.Combine(Environment.CurrentDirectory,Guid.NewGuid().ToString("n"));
		//	long len = 1024 * 1024 * 16;
		//
		//	var s2 = Stopwatch.StartNew();
		//	using(var fs = File.Open(fn,FileMode.Create,FileAccess.ReadWrite,FileShare.Read))
		//	{
		//		for(long i=0; i<len; i++) {
		//			byte b = (byte)(i % 255);
		//			fs.Write(ds.Serialize(b),0,1);
		//		}
		//		Debug.WriteLine("2w="+s2.ElapsedMilliseconds);
		//		fs.Seek(0,SeekOrigin.Begin);
		//		byte[] buff = new byte[1];
		//		for(long i=0; i<len; i++) {
		//			fs.Read(buff,0,1);
		//			Assert.IsTrue(buff[0] == (byte)(i % 255));
		//		}
		//		Debug.WriteLine("2t="+s2.ElapsedMilliseconds);
		//	}
		//	File.Delete(fn);
		//}
	}
}

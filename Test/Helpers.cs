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
	public static class Helpers
	{
		public static void ReadWriteTest(ITitanicArray<byte> arr)
		{
			long len = arr.Length;
			for(long i=0; i<len; i++) {
				arr[i] = (byte)(i % 255);
			}
			for(long i=0; i<len; i++) {
				Assert.IsTrue(arr[i] == (byte)(i % 255));
			}
		}

		public static ITitanicArrayConfig LotsOfDataConfig { get {
			var c = new TitanicArrayConfig {
				DataSerializer = new CustomByteSerializer(),
				Capacity = 1024 * 1024 * 1,
				BackingStoreFileName = GetLocalTempFileName(),
			};
			return c;
		}}

		public static ITitanicArrayConfig TimingConfig { get {
			var ds = new CustomByteSerializer();
			var c = new TitanicArrayConfig {
				DataSerializer = ds,
				Capacity = 1024 * 1024 * 4,
				BackingStoreFileName = GetLocalTempFileName(),
			};
			return c;
		}}

		public static void TimingTest(ITitanicArray<byte> arr)
		{
			string name = arr.GetType().Name;

			var s1 = Stopwatch.StartNew();
			long len = arr.Length;
			for(long i=0; i<len; i++) {
				arr[i] = (byte)(i % 255);
			}
			Debug.WriteLine(name+" w="+s1.ElapsedMilliseconds);
			for(long i=0; i<len; i++) {
				Assert.IsTrue(arr[i] == (byte)(i % 255));
			}
			Debug.WriteLine(name+" t="+s1.ElapsedMilliseconds);
		}

		static Random rnd = new Random();
		public static string GetLocalTempFileName()
		{
			string name = Guid.NewGuid().ToString("n") + rnd.Next();
			return Path.Combine(Environment.CurrentDirectory,name);
		}
	}

	public class CustomByteSerializer : IDataSerializer
	{
		public T Deserialize<T>(byte[] bytes)
		{
			return (T)((object)bytes[0]);
		}

		public byte[] Serialize<T>(T item)
		{
			return new byte[] { (byte)((object)item) };
		}
	}
}

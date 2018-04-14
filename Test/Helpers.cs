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
		public static void ReadWriteTest<T>(ITitanicArray<T> arr, IDataIterator<T> iter)
		{
			long len = arr.Length;
			iter.Reset();
			for(long i=0; i<len; i++) {
				arr[i] = iter.GetNext();
			}
			iter.Reset();
			for(long i=0; i<len; i++) {
				Assert.IsTrue(iter.AreEqual(arr[i],iter.GetNext()));
			}
		}

		public static ITitanicArrayConfig<T> LotsOfDataConfig<T>(IDataSerializer<T> ser)
		{
			var c = new TitanicArrayConfig<T> {
				DataSerializer = ser,
				Capacity = 1024 * 1024 * 1,
				BackingStoreFileName = GetLocalTempFileName(),
			};
			return c;
		}

		public static ITitanicArrayConfig<T> TimingConfig<T>(IDataSerializer<T> ser)
		{
			var c = new TitanicArrayConfig<T> {
				DataSerializer = ser,
				Capacity = 1024 * 1024 * 1,
				BackingStoreFileName = GetLocalTempFileName(),
			};
			return c;
		}

		public static void TimingTest<T>(ITitanicArray<T> arr, IDataIterator<T> iter)
		{
			string name = arr.GetType().Name + "-" + iter.GetType().Name;

			var s1 = Stopwatch.StartNew();
			long len = arr.Length;
			iter.Reset();
			for(long i=0; i<len; i++) {
				arr[i] = iter.GetNext();
			}
			Debug.WriteLine(name+"\tw="+s1.ElapsedMilliseconds);
			iter.Reset();
			for(long i=0; i<len; i++) {
				Assert.IsTrue(iter.AreEqual(arr[i],iter.GetNext()));
			}
			Debug.WriteLine(name+"\tt="+s1.ElapsedMilliseconds);
		}

		static Random rnd = new Random();
		public static string GetLocalTempFileName()
		{
			string name = Guid.NewGuid().ToString("n") + rnd.Next();
			//return Path.Combine(Environment.CurrentDirectory,name);
			return Path.Combine("c:\\temp",name);
		}
	}
}

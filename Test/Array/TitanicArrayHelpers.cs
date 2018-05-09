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
	public static class TitanicArrayHelpers
	{
		const ulong lfsrStart = 1301L;

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

		static void WriteRandom<T>(ITitanicArray<T> arr, IDataIterator<T> iter)
		{
			long len = arr.Length;
			var seq = LinearFeedbackShiftRegister.SequenceLength(lfsrStart,(ulong)len).GetEnumerator();

			iter.Reset();
			for(long i=0; i<len; i++) {
				seq.MoveNext();
				long x = (long)seq.Current;
				T next = iter.GetNext();
				arr[x] = next;
			}
		}

		public static void ReadWriteRandom<T>(ITitanicArray<T> arr, IDataIterator<T> iter)
		{
			WriteRandom(arr,iter);
			ReadAndTestRandom(arr,iter);
		}

		static void ReadAndTestRandom<T>(ITitanicArray<T> arr, IDataIterator<T> iter)
		{
			long len = arr.Length;
			var seq = LinearFeedbackShiftRegister.SequenceLength(lfsrStart,(ulong)len).GetEnumerator();
			iter.Reset();
			for(long i=0; i<len; i++) {
				seq.MoveNext();
				long x = (long)seq.Current;
				T next = iter.GetNext();
				Assert.IsTrue(iter.AreEqual(arr[x],next));
			}
		}

		public static ITitanicArrayConfig<T> TimingConfig<T>(IDataSerializer<T> ser)
		{
			var c = new TitanicArrayConfig<T> {
				DataSerializer = ser,
				Capacity = 1024 * 1024 * 1,
				BackingStoreFileName = GetLocalTempFileName(),
				IsTemporary = true
			};
			return c;
		}

		public static void TimingTest<T>(ITitanicArray<T> arr, IDataIterator<T> iter)
		{
			string name = "TimingTest "+arr.GetType().Name + "-" + iter.GetType().Name;

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

		public static void TimingRandom<T>(ITitanicArray<T> arr, IDataIterator<T> iter)
		{
			string name = "TimingRand "+arr.GetType().Name + "-" + iter.GetType().Name;

			var s1 = Stopwatch.StartNew();
			WriteRandom(arr,iter);
			Debug.WriteLine(name+"\tw="+s1.ElapsedMilliseconds);
			ReadAndTestRandom(arr,iter);
			Debug.WriteLine(name+"\tt="+s1.ElapsedMilliseconds);
		}

		static Random rnd = new Random();
		public static string GetLocalTempFileName()
		{
			string name = Guid.NewGuid().ToString("n") + rnd.Next();
			return Path.Combine(Environment.CurrentDirectory,name);
		}
	}
}

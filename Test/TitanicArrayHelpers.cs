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
			var seq = LinearFeedbackShiftRegister.SequenceLength(1L,len).GetEnumerator();

			iter.Reset();
			for(long i=0; i<len; i++) {
				long x;
				do {
					x = (long)seq.Current;
					//note: MoveNext() must come after Current since LFSR sequence length is 2^n-1
					// so we need to produce a 0 as the first element to make it 2^n
					seq.MoveNext();
				} while(x >= len); //skip numbers above the length since sequencelength rounds up
				T next = iter.GetNext();
				arr[x] = next;
				//Debug.WriteLine(x+" = "+next);
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
			var seq = LinearFeedbackShiftRegister.SequenceLength(1L,len).GetEnumerator();
			iter.Reset();
			for(long i=0; i<len; i++) {
				long x;
				do {
					x = (long)seq.Current;
					seq.MoveNext();
				} while(x >= len);
				T next = iter.GetNext();
				//Debug.WriteLine(x+" = "+next);
				Assert.IsTrue(iter.AreEqual(arr[x],next));
			}
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
			//return Path.Combine(Environment.CurrentDirectory,name);
			return Path.Combine("c:\\temp",name);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HugeStructures.Test
{
	//Random number generator that does not repeat until all numbers have been visited
	// it's not as random as the basic Random number generator
	// 

	public class UniqueRandom
	{
		public UniqueRandom(int min = int.MinValue, int max = int.MaxValue)
		{
			RndMin = min;
			RndMax = max;
			Reset();
		}

		void Reset()
		{
			int len = IntCeil((int)((long)RndMax - RndMin),SizeOfLong);
			BitMasks = new List<long>(len);
			for(int i=0; i<len; i++) { BitMasks.Add(0L); }
			CurrentBucket = Rnd.Next(0,len);
		}

		public int Next()
		{
			long bits = BitMasks[CurrentBucket];
			while (bits == long.MaxValue) {
				//remove empty bitmask
				int last = BitMasks.Count - 1;
				BitMasks[CurrentBucket] = BitMasks[BitMasks.Count - 1];
				BitMasks.RemoveAt(BitMasks.Count - 1);
				if (BitMasks.Count < 1) { Reset(); }
				CurrentBucket = Rnd.Next(0,BitMasks.Count);
				bits = BitMasks[CurrentBucket];
			}

			int min = 0;
			int max = SizeOfLong;
			int pickbit = 0;

			while(min < max)
			{
				pickbit = Rnd.Next(min,max);
				if ((bits & 1L << pickbit) == 0)
				{
					bits |= 1L << pickbit;
					break;
				}
				else
				{
					long maskL = (long)~0 << pickbit;
					long maskR = (long)~0 >> (SizeOfLong - pickbit);
					int countL = NumberOfSetBits(bits & maskL);
					int countR = NumberOfSetBits(bits & maskR);
					if (countL >= countR) {
						min = pickbit;
					} else {
						max = pickbit;
					}
				}
			}

			return (1 << pickbit) + RndMin;
		}

		// https://graphics.stanford.edu/~seander/bithacks.html#CountBitsSetParallel
		public static int NumberOfSetBits(long v)
		{
			v = v - ((v >> 1) & 0x5555555555555555L);
			v = (v & 0x3333333333333333L) + ((v >> 2) & 0x3333333333333333L);
			v = (v + (v >> 4) & 0xF0F0F0FF0F0F0FL);
			long c = (v * 0x0101010101010101L) >> 56;
			return (int)c;
		}

		static int IntCeil(int n, int d)
		{
			return n / d + (n % d != 0 ? 1 : 0);
		}

		//stores the buckets
		List<long> BitMasks;
		int CurrentBucket;
		Random Rnd = new Random();
		int RndMin;
		int RndMax;
		const int SizeOfLong = 64;
	}
}


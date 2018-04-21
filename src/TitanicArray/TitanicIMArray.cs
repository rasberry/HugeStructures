using ImageMagick;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HugeStructures.TitanicArray
{
	public class TitanicIMArray<T> : ITitanicArray<T>
	{
		public TitanicIMArray() : this(TitanicArrayConfig<T>.Default)
		{
		}

		public TitanicIMArray(ITitanicArrayConfig<T> config)
		{
			this.config = config;
			TSize = GetSizeInBytes(config.DataSerializer);
			pixelsPerItem = IntCeil(TSize,bytesPerPixel);
			long maxPixels = config.Capacity * pixelsPerItem;
			FindDimensions(maxPixels,out Width, out Height);

			MagickNET.SetTempDirectory(Path.GetDirectoryName(config.BackingStoreFileName));
			im = new MagickImage(MagickColor.FromRgba(0,0,0,0),Width,Height);
			pixels = im.GetPixels();
		}

		public T this[long index] {
			get {
				byte[] data = new byte[TSize];
				long fdex = index * TSize / bytesPerPixel;
				int foff = (int)(index * TSize % bytesPerPixel);

				int currFloatByte = foff % bytesPerFloat;
				long currPixel = fdex;
				int indexData = 0;
				int indexFloat = foff / bytesPerFloat;
				float[] pixelData = null;
				byte[] floatData = null;
				bool nextPixel = true;
				bool nextFloat = true;

				do {
					if (nextPixel) {
						pixelData = GetLinearPixel(pixels,currPixel,Width);
						nextPixel = false;
					}
					if (nextFloat) {
						float fc = pixelData[indexFloat];
						floatData = BitConverter.GetBytes(fc);
						nextFloat = false;
					}
					
					data[indexData] = floatData[currFloatByte];
					// Debug.WriteLine("get "+currPixel+"\t"+indexFloat+"\t"+currFloatByte+"\t"+indexData+"\t"+floatData[currFloatByte]);
					currFloatByte++;
					indexData++;

					if (currFloatByte >= bytesPerFloat) {
						indexFloat++;
						currFloatByte = 0;
						nextFloat = true;
					}
					if (indexFloat >= floatsPerPixel) {
						currPixel++;
						indexFloat = 0;
						nextPixel = true;
					}
				} while (indexData < TSize);

				T v = config.DataSerializer.Deserialize(data);
				return v;
			}
			set {
				byte[] data = config.DataSerializer.Serialize(value);
				long fdex = index * TSize / bytesPerPixel;
				int foff = (int)(index * TSize % bytesPerPixel);

				int currFloatByte = foff % bytesPerFloat;
				long currPixel = fdex;
				int indexData = 0;
				int indexFloat = foff / bytesPerFloat;
				float[] pixelData = null;
				byte[] floatData = null;
				bool nextPixel = true;
				bool nextFloat = true;
				bool isLast = false;

				do {
					if (nextPixel) {
						pixelData = GetLinearPixel(pixels,currPixel,Width);
						nextPixel = false;
					}
					if (nextFloat) {
						float fc = pixelData[indexFloat];
						floatData = BitConverter.GetBytes(fc);
						nextFloat = false;
					}
					
					floatData[currFloatByte] = data[indexData];
					// Debug.WriteLine("set "+currPixel+"\t"+indexFloat+"\t"+currFloatByte+"\t"+indexData+"\t"+floatData[currFloatByte]);
					currFloatByte++;
					indexData++;

					if (currFloatByte >= bytesPerFloat || isLast) {
						Debug.Write("\tBF "+BitConverter.ToString(floatData,0));
						float newFloat = BitConverter.ToSingle(floatData,0);
						Debug.WriteLine("\tAF "+BitConverter.ToString(BitConverter.GetBytes(newFloat),0));
						pixelData[indexFloat] = newFloat;
						indexFloat++;
						currFloatByte = 0;
						nextFloat = true;
					}
					if (indexFloat >= floatsPerPixel || isLast) {
						SetLinearPixel(pixels,pixelData,currPixel,Width);
						currPixel++;
						indexFloat = 0;
						nextPixel = true;
					}
					if (indexData >= TSize - 1) {
						isLast = true;
					}
				} while (indexData < TSize);
			}
		}

		public long Length { get { return config.Capacity; }}

		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing) {
				pixels.Dispose();
				im.Dispose();
			}
			if (config.IsTemporary) {
				RemoveFile(config.BackingStoreFileName);
			}
		}

		static void RemoveFile(string name)
		{
			try {
				if (File.Exists(name)) {
					//Debug.WriteLine("deleting "+config.BackingStoreFileName);
					File.Delete(name);
				}
			} catch (UnauthorizedAccessException e) {
				Trace.WriteLine(e.ToString());
			}
		}

		static int GetSizeInBytes(IDataSerializer<T> ser)
		{
			var data = ser.Serialize(default(T));
			return data.Length;
		}

		static void FindDimensions(long count, out int x, out int y)
		{
			double sq = Math.Sqrt(count);
			x = (int)Math.Floor(sq);
			bool isPastHalf = Math.Round(sq) > sq + 0.5;
			y = x + (isPastHalf ? 2 : 1);
		}

		static int IntCeil(int num, int den)
		{
			return num / den + (num % den == 0 ? 0 : 1);
		}

		static float[] GetLinearPixel(IPixelCollection p,long offset, int width)
		{
			int y = (int)(offset / width);
			int x = (int)(offset % width);
			float[] pixel = p.GetArea(x,y,1,1);
			Debug.WriteLine("get "+offset+"\t["+x+","+y+"]"
				+"\t"+BitConverter.ToString(BitConverter.GetBytes(pixel[0]),0)
				+"\t"+BitConverter.ToString(BitConverter.GetBytes(pixel[1]),0)
				+"\t"+BitConverter.ToString(BitConverter.GetBytes(pixel[2]),0)
				+"\t"+BitConverter.ToString(BitConverter.GetBytes(pixel[3]),0)
			);
			return pixel;
		}

		static void SetLinearPixel(IPixelCollection p, float[] pixel, long offset, int width)
		{
			int y = (int)(offset / width);
			int x = (int)(offset % width);
			p.SetArea(x,y,1,1,pixel);
			Debug.WriteLine("set "+offset+"\t["+x+","+y+"]"
				+"\t"+BitConverter.ToString(BitConverter.GetBytes(pixel[0]),0)
				+"\t"+BitConverter.ToString(BitConverter.GetBytes(pixel[1]),0)
				+"\t"+BitConverter.ToString(BitConverter.GetBytes(pixel[2]),0)
				+"\t"+BitConverter.ToString(BitConverter.GetBytes(pixel[3]),0)
			);
		}

		[StructLayout(LayoutKind.Explicit)]
		struct FloatAndInt
		{
			[FieldOffset(0)]
			public float Float;
			[FieldOffset(0)]
			public int Int;

			public byte[] GetBytes()
			{
				return BitConverter.GetBytes(Int);
			}
			public void SetBytes(byte[] data)
			{
				Int = BitConverter.ToInt32(data,0);
			}
		}

		ITitanicArrayConfig<T> config;
		IPixelCollection pixels;
		MagickImage im;
		int Width;
		int Height;
		int TSize;
		int pixelsPerItem;
	
		//each pixel is 4 floats = 16 bytes
		const int bytesPerFloat = 4;
		const int floatsPerPixel = 4;
		const int bytesPerPixel = bytesPerFloat * floatsPerPixel;

	}
}

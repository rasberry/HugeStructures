using LiteDB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HugeStructures
{
	public class TitanicLiteDBArray<T> : ITitanicArray<T>
	{
		public TitanicLiteDBArray() : this(TitanicArrayConfig.Default)
		{
		}

		public TitanicLiteDBArray(ITitanicArrayConfig config, int cacheSizePages = 5000)
		{
			this.config = config;
			TSize = GetSizeInBytes(config.DataSerializer);

			db = new LiteDatabase(
				"Filename="+config.BackingStoreFileName
				+";Journal=false"
				+";Cache Size="+cacheSizePages
			);
			collection = db.GetCollection("array");
		}

		public T this[long index] {
			get {
				var item = collection.FindById(new BsonValue(index));
				var val = item["v"].AsBinary;
				return config.DataSerializer.Deserialize<T>(val);
			}
			set {
				var bytes = config.DataSerializer.Serialize(value);
				var item = new BsonDocument();
				item.Add("v",new BsonValue(bytes));
				collection.Insert(new BsonValue(index),item);
			}
		}

		public long Length { get {
			return config.Capacity;
		}}

		static int GetSizeInBytes(IDataSerializer ser)
		{
			var data = ser.Serialize(default(T));
			return data.Length;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing) {
				db.Dispose();

				try {
					if (File.Exists(config.BackingStoreFileName)) {
						Debug.WriteLine("deleting "+config.BackingStoreFileName);
						File.Delete(config.BackingStoreFileName);
					}
				} catch (UnauthorizedAccessException e) {
					Trace.WriteLine(e.ToString());
				}
			}
		}

		LiteDatabase db;
		int TSize;
		ITitanicArrayConfig config;
		LiteCollection<BsonDocument> collection;
	}
}

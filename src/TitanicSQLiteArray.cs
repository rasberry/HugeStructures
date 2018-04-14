using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HugeStructures
{
	public class TitanicSQLiteArray<T> : ITitanicArray<T>
	{
		public TitanicSQLiteArray() : this(TitanicArrayConfig.Default)
		{
		}

		public TitanicSQLiteArray(ITitanicArrayConfig config, int cacheSizeKB = 2000)
		{
			this.config = config;
			TSize = GetSizeInBytes(config.DataSerializer);

			SQLiteConnection.CreateFile(config.BackingStoreFileName);
			sqConnection = new SQLiteConnection(
				"Data Source=" + config.BackingStoreFileName
				+";Version=3"
				+";Synchronous=OFF"
				+";Journal Mode=OFF"
				+";Cache Size=-" + cacheSizeKB
			);
			sqConnection.Open();

			string sql = "CREATE TABLE IF NOT EXISTS array ("
				+ "i INTEGER PRIMARY KEY ASC,"
				+ "v BLOB"
				+");"
			;

			using (SQLiteCommand command = new SQLiteCommand(sql, sqConnection))
			{
				command.ExecuteNonQuery();
			}
		}

		public long Length { get {
			return config.Capacity;

			//string sql = "SELECT max(i) FROM array";
			//using (SQLiteCommand command = new SQLiteCommand(sql, sqConnection))
			//{
			//	var reader = command.ExecuteReader(System.Data.CommandBehavior.SingleResult);
			//	if (reader.Read()) {
			//		if (!reader.IsDBNull(0)) {
			//			object o = reader.GetValue(0);
			//			return (long)o;
			//		}
			//	}
			//	return 0;
			//}
		}}

		public T this[long index] {
			get {
				string sql = "SELECT v FROM array WHERE i = "+(index+1)+";";
				using (SQLiteCommand command = new SQLiteCommand(sql, sqConnection))
				using (var reader = command.ExecuteReader())
				{
					while(reader.Read()) {
						byte[] buffer = GetBytes(reader);
						T data = config.DataSerializer.Deserialize<T>(buffer);
						return data;
					}
				}
				return default(T);
			}
			set {
				byte[] data = config.DataSerializer.Serialize(value);

				string sql = "INSERT INTO array (i,v) VALUES (@index,@val)";
				using (SQLiteCommand command = new SQLiteCommand(sql, sqConnection))
				{
					var pIndex = command.Parameters.Add("@index",System.Data.DbType.Int64,data.Length);
					var pVal = command.Parameters.Add("@val",System.Data.DbType.Binary,data.Length);
					pIndex.Value = index + 1;
					pVal.Value = data;
					command.ExecuteNonQuery();
				}
			}
		}

		// https://stackoverflow.com/questions/625029/how-do-i-store-and-retrieve-a-blob-from-sqlite
		byte[] GetBytes(SQLiteDataReader reader)
		{
			byte[] inbuffer = new byte[TSize];
			byte[] outbuffer = new byte[TSize];
			long bytesRead;
			long fieldOffset = 0;
			using (MemoryStream stream = new MemoryStream(outbuffer))
			{
				while ((bytesRead = reader.GetBytes(0, fieldOffset, inbuffer, 0, TSize)) > 0)
				{
					stream.Write(inbuffer, 0, (int)bytesRead);
					fieldOffset += bytesRead;
				}
				return outbuffer;
			}
		}

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
			if (disposing)
			{
				sqConnection.Dispose();
				// https://stackoverflow.com/questions/8511901/system-data-sqlite-close-not-releasing-database-file
				GC.Collect();
				GC.WaitForPendingFinalizers();
			}

			//try {
			//	if (File.Exists(config.BackingStoreFileName)) {
			//		Debug.WriteLine("deleting "+config.BackingStoreFileName);
			//		File.Delete(config.BackingStoreFileName);
			//	}
			//} catch (UnauthorizedAccessException e) {
			//	Trace.WriteLine(e.ToString());
			//}
		}

		ITitanicArrayConfig config;
		SQLiteConnection sqConnection;
		int TSize;
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HugeStructures.TitanicMap
{
	public class TitanicZipMap<K,V> : ITitanicMap<K, V>
	{
		public TitanicZipMap() : this(TitanicMapConfig<K,V>.Default)
		{
		}

		public TitanicZipMap(ITitanicMapConfig<K,V> config)
		{
			this.config = config;
			var stream = File.Open(
				config.BackingStoreFileName,
				FileMode.Create,
				FileAccess.ReadWrite,
				FileShare.Read
			);
			zip = new ZipArchive(stream,ZipArchiveMode.Update,false);
		}

		public V this[K key] {
			get {
				TryGetValue(key,out V val);
				return val;
			}
			set {
				if (!TryGetEntry(key,out var entry, out string name)) {
					value = default(V);
					entry = zip.CreateEntry(name,CompressionLevel.Optimal);
				}
				PopulateEntry(entry,key,value);
			}
		}

		public bool TryGetValue(K key, out V value)
		{
			if (!TryGetEntry(key,out var entry, out string _)) {
				value = default(V);
				return false;
			}

			PopulateKV(entry,out K _, out value);
			return true;
		}

		public bool TryRemove(K key)
		{
			if (!TryGetEntry(key,out var entry, out string name)) {
				return false;
			}
			entry.Delete();
			return true;
		}

		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
			foreach(var entry in zip.Entries) {
				PopulateKV(entry,out K key, out V value);
				yield return new KeyValuePair<K, V>(key,value);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (zip != null) {
					zip.Dispose();
				}
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

		bool TryGetEntry(K key, out ZipArchiveEntry entry, out string name)
		{
			long hash = config.KeySerializer.GetKeyHash(key);
			name = hash.ToString();
			entry = zip.GetEntry(name);
			return entry != null;
		}

		void PopulateEntry(ZipArchiveEntry entry, K key, V value)
		{
			var valData = config.ValueSerializer.Serialize(value);
			var keyData = config.KeySerializer.Serialize(key);
			byte[] buff = new byte[2*I + keyData.Length + valData.Length];
			
			Array.Copy(BitConverter.GetBytes(keyData.Length),0,buff,0,I);
			Array.Copy(BitConverter.GetBytes(valData.Length),0,buff,I,I);
			Array.Copy(keyData,0,buff,2*I,keyData.Length);
			Array.Copy(valData,0,buff,2*I + keyData.Length,valData.Length);

			using (var s = entry.Open()) {
				s.Write(buff,0,buff.Length);
			}
		}

		void PopulateKV(ZipArchiveEntry entry, out K key, out V value)
		{
			byte[] buff = new byte[entry.Length];
			using (var s = entry.Open()) {
				int total = 0;
				while(total < entry.Length) {
					total += s.Read(buff,total,(int)(entry.Length - total));
				}
			}
			int keyLength = BitConverter.ToInt32(buff,0);
			int valLength = BitConverter.ToInt32(buff,I);
			byte[] keyData = new byte[keyLength];
			byte[] valData = new byte[valLength];
			Array.Copy(buff,2*I,keyData,0,keyLength);
			Array.Copy(buff,2*I + keyLength,valData,0,valLength);

			key = config.KeySerializer.Deserialize(keyData);
			value = config.ValueSerializer.Deserialize(valData);
		}

		ITitanicMapConfig<K,V> config;
		ZipArchive zip;
		const int I = sizeof(int);
	}
}

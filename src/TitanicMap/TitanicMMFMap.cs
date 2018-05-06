using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HugeStructures.TitanicMap
{
	#if false
	//TODO humm.. basically need to re-invent a file system - might stick with Raptor DB

	public class TitanicMMFMap<K,V> : ITitanicMap<K, V>
	{
		public TitanicMMFMap() : this(TitanicMapConfig<K,V>.Default)
		{
		}

		public TitanicMMFMap(ITitanicMapConfig<K,V> config, long capacity = InitialCapacity)
		{
			this.config = config;
			InitMemoryMappedFile(capacity);
		}

		public V this[K key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public bool TryGetValue(K key, out V value)
		{
			throw new NotImplementedException();
		}

		public bool TryRemove(K key)
		{
			throw new NotImplementedException();
		}

		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
			throw new NotImplementedException();
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
				DisposeMMF();
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

		void DisposeMMF()
		{
			if (mmva != null) { mmva.Dispose();}
			if (mmf != null) { mmf.Dispose(); } //also disposes FileStream
		}

		void InitMemoryMappedFile(long size)
		{
			DisposeMMF();

			var stream = File.Open(
				config.BackingStoreFileName,
				FileMode.Create,
				FileAccess.ReadWrite,
				FileShare.Read
			);

			mmf = MemoryMappedFile.CreateFromFile(
				stream,
				null,
				size,
				MemoryMappedFileAccess.ReadWrite,
				HandleInheritability.None,
				false
			);
			mmva = mmf.CreateViewAccessor();
		}

		#region AVL Tree implementation
		// https://github.com/bitlush/avl-tree-c-sharp/blob/master/Bitlush.AvlTree/AvlTree.cs

		bool TreeSearch(K itemKey, out V itemValue)
		{
			Node? node = root;
			long key = config.KeyHasher.GetKeyHash(itemKey);

			while(node != null)
			{
				Node n = node.Value;
				if (key < n.Key) {
					node = GetNodeFromFile(n.Left);
				} else if (key > n.Key) {
					node = GetNodeFromFile(n.Right);
				} else {
					GetKeyValueFromFile(n.ValueIndex,out K _, out itemValue);
					return true;
				}
			}
			itemValue = default(V);
			return false;
		}

		bool TreeInsert(K itemKey, V itemValue)
		{
			Node? node = root;
			long key = config.KeyHasher.GetKeyHash(itemKey);

			while(node != null)
			{
				Node n = node.Value;
				if (key < n.Key) {
					Node? left = GetNodeFromFile(2*n.Index + 1);
					if (left == null) {
						Node l = new Node(n.Index,0,0,2*n.Index + 1,key,0,0);
						//TODO insert value into data and record offset
						TreeInsertBalance(l,1);
						return true;
					} else {
						node = left;
					}
				} else if (key > n.Key) {
					Node? right = GetNodeFromFile(2*n.Index + 2);
					if (right == null) {
						Node r = new Node(n.Index,0,0,2*n.Index + 2,key,0,0);
						//TODO insert value into data and record offset
						TreeInsertBalance(l,-1);
						return true;
					} else {
						node = right;
					}
				} else {
					Node u = new Node(n.Parent,0,0,n.Index,key,n.ValueIndex,n.Balance);
					//TODO insert value into data and record offset
					SetNodeToFile(n.Index,u);
				}
			}

		}

		#endregion

		Node? GetNodeFromFile(long index)
		{
			byte[] buff = new byte[Node.SizeBytes];
			long offset = index * Node.SizeBytes + treeStartOffset;
			if (offset + Node.SizeBytes > dataStartOffset) {
				return null;
			}
			mmva.ReadArray(offset, buff, 0, Node.SizeBytes);
			return Node.FromBytes(buff);
		}
		void SetNodeToFile(long index, Node node)
		{
			byte[] buff = node.ToBytes();
			long offset = index * Node.SizeBytes + treeStartOffset;
			if (offset + Node.SizeBytes > dataStartOffset) {
				//TODO move stuff to the end for more room
			}
			mmva.WriteArray(offset, buff, 0, Node.SizeBytes);
		}
		void GetKeyValueFromFile(long offset,out K itemKey, out V itemVal)
		{
			long fileOffset = offset + dataStartOffset;
			byte[] header = new byte[2*I];
			mmva.ReadArray(fileOffset,header,0,2*I);
			
			int keyLen = BitConverter.ToInt32(header,0);
			int valLen = BitConverter.ToInt32(header,I);
			
			byte[] kvData = new byte[keyLen + valLen];
			mmva.ReadArray(fileOffset + 2*I,kvData,0,keyLen + valLen);
			
			byte[] keyData = new byte[keyLen];
			byte[] valData = new byte[valLen];
			Array.Copy(kvData,0,keyData,0,keyLen);
			Array.Copy(kvData,keyLen,valData,0,valLen);
			
			itemKey = config.KeyHasher.Deserialize(keyData);
			itemVal = config.ValueSerializer.Deserialize(valData);
		}
		void SetKeyValueToFile(long offset, K itemKey, V itemValue)
		{
			byte[] keyData = config.KeyHasher.Serialize(itemKey);
			byte[] valData = config.ValueSerializer.Serialize(itemValue);

			long fileOffset = offset + dataStartOffset;
			int dataLen = 2*I + keyData.Length + valData.Length;

			if (fileOffset + dataLen > mmva.Capacity) {
				//TODO increase the mmf file size
			}

			byte[] buff = new byte[dataLen];
			byte[] keyLenData = BitConverter.GetBytes(keyData.Length);
			byte[] valLenData = BitConverter.GetBytes(valData.Length);
			
			Array.Copy(keyLenData,0,buff,0,I);
			Array.Copy(valLenData,0,buff,I,I);
			Array.Copy(keyData,0,buff,2*I,keyData.Length);
			Array.Copy(valData,0,buff,2*I + keyData.Length,valData.Length);

			mmva.WriteArray(fileOffset,buff,0,dataLen);
			//TODO need to update dataCurrentSize if necessary
		}

		struct Node
		{
			/*8*/ public long Parent;
			/*8*/ public long Left;
			/*8*/ public long Right;
			/*8*/ public long Index;
			/*8*/ public long Key;
			/*8*/ public long ValueIndex;
			/*4*/ public int Balance;

			public const int SizeBytes = 6*L + 1*I;

			public Node(long parent, long left, long right, long index, long key, long vi, int b)
			{
				Parent = parent;
				Left = left;
				Right = right;
				Index = index;
				Key = key;
				ValueIndex = vi;
				Balance = b;
			}

			public byte[] ToBytes()
			{
				byte[] data = new byte[SizeBytes];
				/*8*/ BitConverter.GetBytes(Parent)     .CopyTo(data,0);
				/*8*/ BitConverter.GetBytes(Left)       .CopyTo(data,0+L);
				/*8*/ BitConverter.GetBytes(Right)      .CopyTo(data,0+L+L);
				/*8*/ BitConverter.GetBytes(Index)      .CopyTo(data,0+L+L+L);
				/*8*/ BitConverter.GetBytes(Key)        .CopyTo(data,0+L+L+L+L);
				/*8*/ BitConverter.GetBytes(ValueIndex) .CopyTo(data,0+L+L+L+L+L);
				/*4*/ BitConverter.GetBytes(Balance)    .CopyTo(data,0+L+L+L+L+L+L);
				return data;
			}
			
			public static Node FromBytes(byte[] data)
			{
				if (data.Length != SizeBytes) {
					throw new InvalidDataException();
				}
				var node = new Node(
					/*8*/ BitConverter.ToInt64(data,0),
					/*8*/ BitConverter.ToInt64(data,0+L),
					/*8*/ BitConverter.ToInt64(data,0+L+L),
					/*8*/ BitConverter.ToInt64(data,0+L+L+L),
					/*8*/ BitConverter.ToInt64(data,0+L+L+L+L),
					/*8*/ BitConverter.ToInt64(data,0+L+L+L+L+L),
					/*4*/ BitConverter.ToInt32(data,0+L+L+L+L+L+L)
				);
				return node;
			}
		}

		const long InitialCapacity = Node.SizeBytes * 1024;
		ITitanicMapConfig<K,V> config;
		MemoryMappedFile mmf;
		MemoryMappedViewAccessor mmva;
		Node? root;
		long dataStartOffset;
		long dataCurrentSize;
		const long treeStartOffset = 8;
		const int L = sizeof(long);
		const int I = sizeof(int);
	}
	#endif
}

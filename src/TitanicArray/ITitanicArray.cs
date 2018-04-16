using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HugeStructures.TitanicArray
{
	public interface ITitanicArray<T> : IDisposable
	{
		long Length { get; }
		T this[long index] { get; set; }
	}
}

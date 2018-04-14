using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace HugeStructures.Test
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			Debug.Listeners.Add(new ConsoleTraceListener());

			var clist = Assembly.GetExecutingAssembly().GetTypes()
				.Where(t => t.GetCustomAttributes(typeof(TestClassAttribute),false).Any())
			;

			foreach(Type c in clist)
			{
				var inst = Activator.CreateInstance(c);
				var mlist = c.GetMethods()
					.Where(m => m.GetCustomAttributes(typeof(TestMethodAttribute),false).Any())
				;
				foreach(var m in mlist) {
					m.Invoke(inst,new object[0]);
				}
			}
		}
	}
}

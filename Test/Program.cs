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
		static void Main(string[] args)
		{
			try {
				MainMain(args);
			} catch(Exception e) {
				Exception c = e;
				while(c != null) {
					Console.Error.WriteLine(c.ToString());
					c = c.InnerException;
				}
			}
		}

		static void MainMain(string[] args)
		{
			Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

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

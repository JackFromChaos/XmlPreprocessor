using dio.data;
using dio.preprocessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XmlPreprocessor
{
	class Program
	{
		static void Main(string[] args)
		{
			string input = args[0];
			string macros = args[1];
			string output = args[2];
			Preprocessor pre = new Preprocessor();
			pre.loadMacros(new XmlDataReader(macros));
			var src = new XmlDataReader(input);
			DataReadWriter drw = pre.Processor(src);
			Console.WriteLine(drw.ToXmlString());
			drw.SaveXml(output);
		}
	}
}

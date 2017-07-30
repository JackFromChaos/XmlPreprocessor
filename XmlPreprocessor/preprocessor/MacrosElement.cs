using dio.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dio.preprocessor
{
	public class MacrosElement : IDataReader
	{
		public string name;
		IDataReader element;
		List<MacrosElement> elements;
		Dictionary<string, string> attributes = new Dictionary<string, string>();
		public MacrosElement(IDataReader element)
		{
			name = element.GetName();
			this.element = element;
			foreach (IDataAttribute a in element.GetAttributes())
				attributes[a.GetName()] = a.GetValue();
			elements = new List<MacrosElement>();
			foreach (IDataReader e in element)
				elements.Add(new MacrosElement(e));

		}
		internal DataReadWriter CreateDataReadWriter()
		{
			DataReadWriter ret = new DataReadWriter();
			ret.name = name;
			copy(ret, this);
			return ret;
		}

		private void copy(IDataWriter ret, MacrosElement macrosElement)
		{

			foreach (var a in macrosElement.attributes)
				ret.SetAttribute(a.Key, a.Value);
			foreach (var e in macrosElement.elements)
			{
				var child = ret.AddWriter(e.name);
				copy(child, e);
			}
		}

		public string GetAttribute(string p)
		{
			string ret;
			attributes.TryGetValue(p, out ret);
			return ret;

		}

		public IEnumerable<IDataAttribute> GetAttributes()
		{
			return element.GetAttributes();
		}

		public string GetName()
		{
			return element.GetName();
		}

		public string GetText()
		{
			return element.GetText();
		}

		public System.Collections.IEnumerator GetEnumerator()
		{
			return element.GetEnumerator();
		}
	}
}

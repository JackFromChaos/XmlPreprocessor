using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace dio.data
{
	public class XmlDataReader : IDataReader
	{
		//public 
		public XmlDataReader(XmlDocument doc)//:element(Nullable)
		{
			foreach (XmlNode node in doc.ChildNodes)
				if (node is XmlElement)
				{
					element = node as XmlElement;
					break;
				}
		}
		public XmlDataReader(XmlElement element)
		{
			this.element = element;
		}

		public XmlDataReader(string p)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(p);
			foreach (XmlNode node in doc.ChildNodes)
				if (node is XmlElement)
				{
					element = node as XmlElement;
					break;
				}

		}
		XmlElement element;
		//private string p;
		public struct DataAttribute : IDataAttribute
		{
			internal XmlAttribute attribute;
			public DataAttribute(XmlAttribute attribute)
			{
				this.attribute = attribute;
			}
			public string GetName()
			{
				return attribute.Name;
			}

			public string GetValue()
			{
				return attribute.Value;
			}
		}
		struct Attributes : IEnumerable<IDataAttribute>
		{
			internal XmlElement element;
			public IEnumerator<IDataAttribute> GetEnumerator()
			{
				foreach (var a in element.Attributes)
					yield return new XmlDataReader.DataAttribute(a as XmlAttribute);
			}
			IEnumerator IEnumerable.GetEnumerator()
			{
				foreach (var a in element.Attributes)
					yield return new XmlDataReader.DataAttribute(a as XmlAttribute);
			}
		}
		public IEnumerable<IDataAttribute> GetAttributes()
		{
			return new Attributes() { element = element };
		}

		public string GetName()
		{
			return element.Name;
		}

		public string GetText()
		{
			return element.InnerText;
		}

		public IEnumerator GetEnumerator()
		{
			foreach (XmlNode a in element.ChildNodes)
			{
				if (a is XmlElement)
					yield return new XmlDataReader(a as XmlElement);
			}

		}


		public String GetAttribute(string p)
		{
			return element.GetAttribute(p);
			//return atr;
			//return new XmlDataReader.DataAttribute(
		}
	}
}

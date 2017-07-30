using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace dio.data
{
	public class XmlDataWriter : IDataWriter
	{
		protected XmlElement element;
		public XmlDataWriter(XmlElement element)
		{
			this.element = element;
		}
		public void SetAttribute(string name, string value)
		{
			element.SetAttribute(name, value);
		}

		public IDataWriter AddWriter(string name)
		{
			XmlElement child = element.OwnerDocument.CreateElement(name);
			element.AppendChild(child);
			return new XmlDataWriter(child);
		}
	}
}

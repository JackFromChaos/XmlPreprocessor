using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace dio.data
{
	public class XmlDataWriterRoot : IDataWriter
	{
		public XmlDataWriterRoot(XmlDocument document)
		{
			this.document = document;
		}

		public XmlDocument document;
		public void SetAttribute(string name, string value)
		{
			throw new NotImplementedException();
		}

		public IDataWriter AddWriter(string name)
		{
			XmlElement child = document.CreateElement(name);
			document.AppendChild(child);
			return new XmlDataWriter(child);
		}
	}

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace dio.data
{
	public class DataReadWriter : IDataReader, IDataWriter
	{

		public string name;
		public string text;
		public List<DataReadWriter> childs = new List<DataReadWriter>();
		public Dictionary<string, IDataAttribute> attributes = new Dictionary<string, IDataAttribute>();
		public class Attribute : IDataAttribute
		{
			public string name;
			public string value;
			public string GetName()
			{
				return name;
			}

			public string GetValue()
			{
				return value;
			}
		}

		public IEnumerable<IDataAttribute> GetAttributes()
		{
			return attributes.Values;
		}

		public string GetName()
		{
			return name;
		}

		public string GetText()
		{
			return text;
		}

		public System.Collections.IEnumerator GetEnumerator()
		{
			return childs.GetEnumerator();
		}

		public void SetAttribute(string name, string value)
		{
			/*IDataAttribute atr;
            if (attributes.TryGetValue(name, out value))
            {
                (Att
            }*/
			if (attributes.ContainsKey(name))
				(attributes[name] as Attribute).value = value;
			else
				attributes[name] = new Attribute() { name = name, value = value };
		}

		public IDataWriter AddWriter(string name)
		{
			var ret = new DataReadWriter();
			ret.name = name;
			childs.Add(ret);
			return ret;
		}

		internal DataReadWriter AddReadWriter(string name)
		{
			var ret = new DataReadWriter();
			ret.name = name;
			childs.Add(ret);
			return ret;
		}
		public string ToXmlString()
		{
			XmlDocument doc = ToXmlDocument();
			//return ToXmlDocument().InnerXml;
			StringBuilder sb = new StringBuilder();
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Encoding = Encoding.UTF8;
			settings.Indent = true;
			settings.IndentChars = "\t";
			settings.NewLineChars = "\r\n";
			settings.NewLineHandling = NewLineHandling.Replace;
			using (XmlWriter writer = XmlWriter.Create(sb, settings))
			{
				doc.Save(writer);
			}
			string s = sb.ToString().Replace("encoding=\"utf-16\"", "encoding=\"utf-8\"");
			return s;
		}
		internal XmlDocument ToXmlDocument()
		{
			XmlDocument doc = new XmlDocument();
			XmlElement element = doc.CreateElement(name);
			doc.AppendChild(element);
			copy(element, this);
			return doc;
		}

		private void copy(XmlElement element, DataReadWriter reader)
		{
			foreach (var a in reader.attributes)
				element.SetAttribute(a.Key, a.Value.GetValue());
			foreach (var e in reader.childs)
			{
				XmlElement child = element.OwnerDocument.CreateElement(e.name);
				element.AppendChild(child);
				copy(child, e);
			}
		}


		public string GetAttribute(string p)
		{
			IDataAttribute atr;
			if (attributes.TryGetValue(p, out atr))
				return atr.GetValue();
			return string.Empty;
		}

		public void SaveXml(string fileName)
		{
			System.IO.File.WriteAllText(fileName, ToXmlString(), Encoding.UTF8);

		}
	}

}

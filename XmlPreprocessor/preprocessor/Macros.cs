using dio.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dio.preprocessor
{
	public class Macros : IDataReader
	{

		public void checkMacrosParams(IDataReader el, Preprocessor preprocessor)
		{
			if (Params == null)
				Params = initMacrosParams();

			foreach (IDataAttribute a in el.GetAttributes())
			{
				string name = a.GetName();
				if (name == "ui_show")
					continue;
				if (name == "comment")
					continue;
				if (name.IndexOf("sys-") == 0)
					continue;
				if (name.Length > 0 && name[0] == '_')
					continue;

				if (!Params.Contains(name))

					preprocessor.Error("Invalid param {0} for macros {1}", name, Name);
			}
		}
		private List<string> initMacrosParams()
		{
			List<string> param = new List<string>();
			foreach (IDataAttribute a in macrosReader.GetAttributes())
				param.Add(a.GetName());
			//_macrosParams[macros.Name.ToString()] = param;
			Params = param;
			return param;

		}
		public Macros(IDataReader el, string name = null)
		{
			// TODO: Complete member initialization
			this.macrosReader = el;
			if (string.IsNullOrEmpty(name))
				name = el.GetName();
			this.Name = name;
		}

		protected string Name;
		protected List<string> Params;
		protected List<MacrosElement> _elements;

		public List<MacrosElement> Elements
		{
			get
			{
				if (_elements == null)
				{
					_elements = new List<MacrosElement>();
					foreach (IDataReader el in this.macrosReader)
						_elements.Add(new MacrosElement(el));

				}
				return _elements;
			}
		}
		private IDataReader macrosReader;
		public string GetAttribute(string p)
		{
			return macrosReader.GetAttribute(p);
		}


		public IEnumerable<IDataAttribute> GetAttributes()
		{
			throw new NotImplementedException();
		}

		public string GetName()
		{
			return this.Name;
		}

		public string GetText()
		{
			throw new NotImplementedException();
		}

		public System.Collections.IEnumerator GetEnumerator()
		{
			return Elements.GetEnumerator();
		}
	}

}

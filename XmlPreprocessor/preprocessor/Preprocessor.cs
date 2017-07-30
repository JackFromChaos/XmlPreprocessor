using dio.data;
using dio.utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace dio.preprocessor
{
	public class Preprocessor
	{
		Dictionary<string, Macros> _macros = new Dictionary<string, Macros>();
		public List<string> errors = new List<string>();
		protected string _lastExpression = "";
		private Macros getMacros(IDataReader cur)
		{

			string name = cur.GetName();
			if (!_macros.ContainsKey(name))
			{
				name = cur.GetAttribute("sys-macros");
				if (string.IsNullOrEmpty(name))
					return null;

				if (!_macros.ContainsKey(name))
					return null;
			}
			return _macros[name];
		}

		public void Error(string s, params object[] p)
		{
			string result = string.Format(s, p);
			if (!string.IsNullOrEmpty(_lastExpression))
				result += " in expression: " + _lastExpression;

			/*if (_lastNode != null)
            {
                XmlNode node = _lastNode.ParentNode;
                string path = GetNodeName(_lastNode);
                while (node != null)
                {
                    path = GetNodeName(node) + "." + path;
                    node = node.ParentNode;
                }
                result += "\n\t" + path;
            }*/


			errors.Add(result);
			Console.WriteLine("ERROR: {0}", result);
		}

		public void loadMacros(string fileName)
		{
			XmlDataReader reader = new XmlDataReader(fileName);
			loadMacros(reader);
		}
		public void loadMacros(IDataReader root)
		{
			foreach (IDataReader el in root)
			{
				Macros m = new Macros(el);
				_macros[m.GetName()] = m;
			}
		}


		public class Context
		{
			public int macros = 0;
			public bool InMacros()
			{
				return macros > 0;
			}
			public List<IDataReader> readers = new List<IDataReader>();
			public void PushReader(IDataReader reader)
			{
				readers.Add(reader);
			}
			public IDataReader PopReader()
			{
				int count = readers.Count;
				if (count == 0)
					return null;
				var ret = readers[count - 1];
				readers.RemoveAt(count - 1);
				return ret;
			}
			internal string GetAttribute(string p)
			{
				for (int i = readers.Count - 1; i >= 0; i--)
				{
					IDataReader reader = readers[i];
					string cur = reader.GetAttribute(p);
					if (!string.IsNullOrEmpty(cur))
						return cur;
				}
				return null;
			}
		}
		public DataReadWriter Processor(string fileName)
		{
			XmlDataReader reader = new XmlDataReader(fileName);
			return Processor(reader);
		}
		public DataReadWriter Processor(IDataReader reader)
		{
			reader = FirstProcess(reader);
			DataReadWriter w = new DataReadWriter();
			w.name = reader.GetName();
			copyAttributes(w, reader);
			Context context = new Context();
			copyChildElements(w, reader, context);
			return w;
		}

		private void copyChildElements(DataReadWriter copyTo, IDataReader copyFrom, Context context)
		{
			if (!context.InMacros())
				context.PushReader(copyFrom);
			foreach (IDataReader element in copyFrom)
			{
				copyElement(copyTo, element, context);
			}
			if (!context.InMacros())
				context.PopReader();
		}

		private void copyElement(DataReadWriter copyTo, IDataReader element, Context context)
		{
			Macros macros = getMacros(element);
			if (macros != null)
			{
				//context.macros++;
				processMacros(macros, element, copyTo, context);
				//context.macros--;
			}
			else
			{
				DataReadWriter child = copyTo.AddReadWriter(element.GetName());
				copyAttributes(child, element, context);
				copyChildElements(child, element, context);
			}
		}

		private void processMacros(Macros macros, IDataReader element, DataReadWriter copyTo, Context context)
		{
			context.macros++;
			macros.checkMacrosParams(element, this);
			context.PushReader(macros);
			context.PushReader(element);
			int c = copyTo.childs.Count;
			copyChildElements(copyTo, macros, context);
			context.PopReader();
			context.PopReader();
			context.macros--;
			//Если макрос добавил строки, то вложенные элементы добавляем в первую из добавленных в макросе строк
			if (copyTo.childs.Count > c)
			{
				copyChildElements(copyTo.childs[c], element, context);
			}
		}

		private void copyAttributes(DataReadWriter child, IDataReader element, Context context)
		{
			foreach (var a in element.GetAttributes())
			{
				string value = calcAttributeValue(a.GetValue(), context);
				child.SetAttribute(a.GetName(), value);
			}
		}
		internal List<string> Scaner(string s)
		{
			List<string> tokens = new List<string>();
			bool inExpr = false;
			string token = "";
			for (int i = 0; i < s.Length; i++)
			{
				char c = s[i];
				if (inExpr)
				{
					if (c == ';')
					{
						if (token.Length > 1)
							tokens.Add(token);
						inExpr = false;
						token = "";
					}
					else if (c == '$')
					{
						if (token.Length > 1)
							tokens.Add(token);
						token = "$";
						inExpr = true;
					}
					else
						token += c;
				}
				else
				{
					if (c == '$')
					{
						if (token.Length > 0)
							tokens.Add(token);
						token = "$";
						inExpr = true;
					}
					else
						token += c;
				}


			}
			if (token.Length > 0)
				tokens.Add(token);
			return tokens;
		}
		private string calcStdExp(string str, Context context)
		{
			var tokens = Scaner(str);
			String result = "";

			foreach (var s in tokens)
			{
				if (s[0] == '$')
				{
					string t = getAttributeValueVar(s.Substring(1), context);
					t = calcAttributeValue(t, context);
					result += t;
				}
				else
					result += s;
			}
			return result;
		}
		private string calcAttributeValue(string str, Context context)
		{
			if (str.Length == 0)
				return str;
			/*if (!context.InMacros())
            {
                if (str[0] != '#')
                    return str;
                str = str.Substring(1);
            }*/
			if (str[0] == '{')
			{
				str = calcJS(str, context);
				return calcAttributeValue(str, context);
			}
			if (str[0] == '$')
			{
				str = calcStdExp(str, context);
				//str = getAttributeValueVar(str.Substring(1), context);
				return calcAttributeValue(str, context);
			}
			if (str[0] == '(')
			{
				str = calcAttributeValueExp(str, context);
				return calcAttributeValue(str, context);
			}
			if (str[0] == '[')
			{
				str = str.Substring(1, str.Length - 2);
				string[] arr = str.Split(',');
				for (int i = 0; i < arr.Length; i++)
				{
					arr[i] = calcAttributeValue(arr[i], context);
				}
				str = string.Join(",", arr);
			}
			return str;
		}

		private string calcAttributeValueExp(string str, Context context)
		{
			_lastExpression = str;
			MathParser p = new MathParser();
			p.CustomVars = delegate (string name)
			{
				if (name.Length > 0 && name[0] == '$')
				{
					return getAttributeValueVar(name.Substring(1), context);

				}
				return null;
			};
			decimal val = p.Parse(str);
			string result = val.ToString(CultureInfo.InvariantCulture);
			_lastExpression = "";
			return result;
		}

		private string getAttributeValueVar(string p, Context context)
		{
			string ret = context.GetAttribute(p);
			if (ret == null)
			{
				Error("Not set param: {0}", p);
				return "";
			}
			if (ret == "!")
				Error("Not set param: {0}", p);
			return ret;
		}

		private string calcJS(string str, Context context)
		{
			throw new NotImplementedException();
		}


		private DataReadWriter FirstProcess(IDataReader reader)
		{
			DataReadWriter ret = new DataReadWriter();
			ret.name = reader.GetName();
			copyFirstProcess(ret, reader);
			return ret;


		}
		private void copyAttributes(DataReadWriter w, IDataReader reader)
		{
			foreach (IDataAttribute a in reader.GetAttributes())
				w.SetAttribute(a.GetName(), a.GetValue());
		}
		private void copyFirstProcess(DataReadWriter ret, IDataReader reader)
		{
			///Надо сделать иначе!!!!
			///Сохранять стэк нисходящих нод, и при копировании атрибутов, выполнять для каждого из них calcAttributes
			copyAttributes(ret, reader);
			foreach (IDataReader r in reader)
			{
				string name = r.GetName();


				if (name.IndexOf("def-") == 0)
				{
					name = name.Substring(4);
					//cur.Name = name;
					Macros m = new Macros(r, name);
					_macros[name] = m;
					continue;
				}
				if (name == "sys-script")
				{
					continue;
					//PreDataElement ret = cur.NextNode;
					//!!!!!!!!!!!1var r = _engine.Execute(cur.Value).GetCompletionValue();
					//Console.WriteLine(r.Type);
					//object o=r.AsObject().
					//Jint.Native.Object.ObjectInstance oi = r.AsObject() as Jint.Native.Object.ObjectInstance;
					//oi.Properties.ContainsKey("node"
					//Console.WriteLine(r);
					/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1if (r.Type == Types.String)
                    {

                        try
                        {
                            string s = r.AsString();
                            if (s.Length > 0 && s[0] == '<')
                            {
                                XElement el = XElement.Parse(r.AsString());
                                cur.AddAfterSelf(el);
                                ret = el;
                            }
                        }
                        catch (Exception e)
                        {
                            Error(e.ToString());
                        }
                        //cur.AddAfterSelf(
                    }*/
					//Console.WriteLine(r.GetCompletionValue().Type);
					//cur.Remove();

				}




				DataReadWriter child = ret.AddWriter(r.GetName()) as DataReadWriter;
				copyFirstProcess(child, r);
			}

		}

	}
}

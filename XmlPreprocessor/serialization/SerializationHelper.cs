using dio.serialization.attributs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace dio.serialization
{
	public class SerializationHelper
	{
		#region Static

		public static XmlAttributeOverrides CreateXmlAttributeOverrides()
		{
			XmlAttributeOverrides xOver = new XmlAttributeOverrides();
			foreach (var e in _groups)
				e.Value.initXmlAttributeOverrides(xOver);
			return xOver;
		}

		static Dictionary<string, SerializationHelper> _groups = new Dictionary<string, SerializationHelper>();

		public static void RegisterField(string group, Type type, string field)
		{
			GetGroup(group).registerCommandField(type, field);
		}
		public static SerializationHelper GetGroup(string name)
		{
			SerializationHelper ret;
			if (_groups.TryGetValue(name, out ret))
				return ret;
			ret = new SerializationHelper();
			_groups[name] = ret;
			return ret;
		}
		public static void RegisterClass<T>()
		{
			RegisterClass(typeof(T));
		}
		public static void RegisterClass(Type type)
		{
			var arr = type.GetCustomAttributes(typeof(SerializationGroupAttribute), true);
			if (arr.Length == 0)
				throw new Exception("Error group");

			SerializationHelper g = GetGroup((arr[0] as SerializationGroupAttribute).name);
			g.registerClassType(type);
		}
		public static void RegisterClass(string group, Type type)
		{
			SerializationHelper g = GetGroup(group);
			g.registerClassType(type);
		}
		public static void RegisterClass(string group, Type type, string name)
		{
			SerializationHelper g = GetGroup(group);
			g.registerClassType(name, type);
		}
		public static void RegisterAssembly(string group, Assembly assembly, Type baseType)
		{
			foreach (var t in assembly.GetTypes())
			{
				if (baseType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
					RegisterClass(group, t);
			}
		}

		#endregion Static

		Dictionary<string, Type> _commands = new Dictionary<string, Type>();
		struct CommandsFields
		{
			public Type type;
			public string field;
		}


		List<CommandsFields> _commandsFields = new List<CommandsFields>();
		protected void registerCommandField(Type type, string field)
		{
			_commandsFields.Add(new CommandsFields() { field = field, type = type });
		}
		protected void registerClassType(string name, Type type)
		{
			_commands[name] = type;
		}
		protected void registerClassType(Type type)
		{
			string typeName = type.Name;
			var attributes = type.GetCustomAttributes(typeof(XmlRootAttribute), true);
			if (attributes.Length > 0)
				typeName = (attributes[0] as XmlRootAttribute).ElementName;
			else
				typeName = typeName.Substring(0, 1).ToLower() + typeName.Substring(1);
			registerClassType(typeName, type);
		}
		protected XmlAttributeOverrides createXmlAttributeOverrides()
		{
			XmlAttributeOverrides xOver = new XmlAttributeOverrides();
			initXmlAttributeOverrides(xOver);
			return xOver;
		}

		protected void initXmlAttributeOverrides(XmlAttributeOverrides xOver)
		{
			foreach (var field in _commandsFields)
				commandsToXmlAttributeOverrides(field.type, field.field, xOver);
		}
		protected void commandsToXmlAttributeOverrides(Type type, string field, XmlAttributeOverrides xOver)
		{
			XmlAttributes attrs = new XmlAttributes();
			foreach (var c in _commands)
				attrs.XmlElements.Add(new XmlElementAttribute(c.Key, c.Value));
			xOver.Add(type, field, attrs);
		}





	}
}

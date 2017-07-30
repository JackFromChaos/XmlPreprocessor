using dio.serialization.attributs;
using dio.serialization.converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace dio.serialization
{
	public static class ReflectionFacade
	{
		static internal Dictionary<Type, IStringTypeConverter> _typeConverters = new Dictionary<Type, IStringTypeConverter>();



		static ReflectionFacade()
		{
			RegisterAssembly(typeof(ReflectionFacade).Assembly);
			RegisterSimpleConverter<string>(
				delegate (string value, CultureInfo culture) { return value; },
				delegate (object value, CultureInfo culture) { return (string)value; }
				);
			/*registerSimpleConverter<String>(
                delegate(string value, CultureInfo culture) { return value; },
                delegate(object value, CultureInfo culture) { return (string)value; }
                );*/
			RegisterSimpleConverter<float>(
				delegate (string value, CultureInfo culture) { return float.Parse(value, culture); },
				delegate (object value, CultureInfo culture) { return ((float)value).ToString(culture); }
				);
			RegisterSimpleConverter<int>(
				delegate (string value, CultureInfo culture) { return int.Parse(value); },
				delegate (object value, CultureInfo culture) { return value.ToString(); }
				);
			/*registerSimpleConverter<int[]>(
                delegate(string value, CultureInfo culture) { return value.Split(; },
                delegate(object value, CultureInfo culture) { return value.ToString(); }
                );*/
			RegisterSimpleConverter<Int16>(
				delegate (string value, CultureInfo culture) { return Int16.Parse(value); },
				delegate (object value, CultureInfo culture) { return value.ToString(); }
				);
			RegisterSimpleConverter<bool>(
				delegate (string value, CultureInfo culture)
				{
					return value.ToLower() == "true";
				},
				delegate (object value, CultureInfo culture)
				{
					return (bool)value ? "true" : "false";
				}
				);



			/*            RegisterSimpleConverter<List<string>>(
                delegate(string value, CultureInfo culture) { return value.Split(new string[]{";;"}, StringSplitOptions.None).ToList(); },
                delegate(object value, CultureInfo culture) { return string.Join(";;",(value as List<string>).ToArray()); }
                );
*/

		}
		public static void RegisterSimpleConverter<T>(SimpleConverter.dConvertFromString from, SimpleConverter.dConvertToString to)
		{
			SimpleConverter converter = new SimpleConverter() { fromString = from, toString = to };
			_typeConverters[typeof(T)] = converter;

		}
		public static IStringTypeConverter GetStringTypeConverter(Type type)
		{
			IStringTypeConverter ret = null;
			if (_typeConverters.TryGetValue(type, out ret))
				return ret;
			var attr = type.GetCustomAttributes(typeof(StringTypeConverterAttribute), false);
			if (attr.Length > 0)
			{
				ret = (attr[0] as StringTypeConverterAttribute).CreateConverter();
				_typeConverters[type] = ret;
				return ret;
			}
			if (type.IsEnum)
			{
				ret = new EnumStringConverter(type);
				_typeConverters[type] = ret;
				return ret;
			}
			return ret;
		}

		public static void RegisterType(string name, Type type)
		{
			_userTypes[name] = type;
		}
		static List<Assembly> assemblys = new List<Assembly>();
		static bool inited = false;
		static Dictionary<string, Type> _fullTypes;
		static Dictionary<string, Type> _shortTypes;
		static Dictionary<string, Type> _customTypes;
		static Dictionary<string, Type> _userTypes = new Dictionary<string, Type>();
		public static Type GetType(string name)
		{
			checkInit();
			Type ret = null;
			if (_userTypes.TryGetValue(name, out ret))
				return ret;
			if (_customTypes.TryGetValue(name, out ret))
				return ret;
			if (_shortTypes.TryGetValue(name, out ret))
				return ret;
			if (_fullTypes.TryGetValue(name, out ret))
				return ret;
			return null;
		}
		public static object CreateObject(string name)
		{
			Type type = GetType(name);
			return Activator.CreateInstance(type);
		}

		public static void RegisterAssembly(Assembly assembly)
		{
			assemblys.Insert(0, assembly);
			inited = false;
		}
		static void checkInit(bool forsing = false)
		{
			if (!forsing && inited)
				return;
			inited = true;
			_fullTypes = new Dictionary<string, Type>();
			_shortTypes = new Dictionary<string, Type>();
			_customTypes = new Dictionary<string, Type>();
			foreach (var a in assemblys)
			{
				foreach (var t in a.GetTypes())
				{
					_fullTypes[t.FullName] = t;
					_shortTypes[t.Name] = t;
					var aliases = t.GetCustomAttributes(typeof(AliaseNameAttribute), false);
					foreach (AliaseNameAttribute aliase in aliases)
						_customTypes[aliase.name] = t;
				}
			}



		}



		/*static MetaDataFacade _instance;

        public static MetaDataFacade Instance
        {
            get 
            {
                if (MetaDataFacade._instance == null)
                    MetaDataFacade._instance = new MetaDataFacade();
                return MetaDataFacade._instance; 
            }
        }
        internal List<Assembly> assemblys = new List<Assembly>();
        internal void Add(Assembly assembly)
        {
            assemblys.Insert(0, assembly);
        }*/

		internal static object CreateObject(string subType, ICustomTypeManager typeManager)
		{
			return CreateObject(subType);
		}

		public static void RegisterType<T>(string name)
		{
			RegisterType(name, typeof(T));
		}

		public static Type GetType(string subType, ICustomTypeManager typeManager)
		{
			if (typeManager != null)
			{
				var ret = typeManager.GetType(subType);
				if (ret != null)
					return ret;
			}
			return GetType(subType);
		}

		internal static string GetTypeName(Type t, ICustomTypeManager typeManager)
		{
			if (typeManager != null)
			{
				string ret = typeManager.GetTypeName(t);
				if (ret != null)
					return ret;
			}
			return GetTypeName(t);
		}

		private static string GetTypeName(Type t)
		{
			string ret;
			if (tryGetName(t, _userTypes, out ret))
				return ret;
			if (tryGetName(t, _customTypes, out ret))
				return ret;
			if (tryGetName(t, _shortTypes, out ret))
				return ret;
			if (tryGetName(t, _fullTypes, out ret))
				return ret;
			return t.Name;
		}

		private static bool tryGetName(Type t, Dictionary<string, Type> dic, out string ret)
		{
			ret = null;
			foreach (var k in dic)
				if (k.Value == t)
				{
					ret = k.Key;
					return true;
				}
			return false;
		}
		static Dictionary<Type, IMetaData> _data = new Dictionary<Type, IMetaData>();
		/*internal static TypeMetaData GetMetaData(Type type)
        {
            TypeMetaData ret = null;
            if (_data.TryGetValue(type, out ret))
                return ret;
            ret = new TypeMetaData(type);
            _data[type] = ret;
            return ret;
        }*/
		internal static IMetaData GetMetaData(Type type)
		{
			IMetaData ret = null;
			if (_data.TryGetValue(type, out ret))
				return ret;
			ret = new TypeMetaData(type);
			_data[type] = ret;
			return ret;
		}

	}
}

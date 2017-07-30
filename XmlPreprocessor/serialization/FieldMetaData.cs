using dio.data;
using dio.serialization.attributs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace dio.serialization
{
	public class FieldMetaData : IFieldMetaData
	{
		protected MemberInfo _memberInfo;
		protected Type _type;
		protected IStringTypeConverter _converter;
		protected Dictionary<Type, string> childAliases;

		private string getTypeName(Type type, ICustomTypeManager typeManager)
		{
			if (childAliases != null)
			{
				string name;
				if (childAliases.TryGetValue(type, out name))
					return name;
			}
			return ReflectionFacade.GetTypeName(type, typeManager);
		}
		private string getObjectName(object o, ICustomTypeManager typeManager)
		{
			Type type = o.GetType();
			if (childAliases != null)
			{
				string name;
				if (childAliases.TryGetValue(type, out name))
					return name;
			}
			/*if (_type != type)
            {
                return GetName() + "-" + type.Name;
            }*/
			return ReflectionFacade.GetTypeName(type, typeManager);
		}

		public FieldMetaData(MemberInfo memberInfo)
		{
			_memberInfo = memberInfo;
			_type = getType();
			_converter = getConverter();

			var atr = ReflectionUtils.getAttributes<System.Xml.Serialization.XmlElementAttribute>(_memberInfo, true);
			if (atr.Count > 0)
			{
				childAliases = new Dictionary<Type, string>();
				foreach (var a in atr)
					childAliases[a.Type] = a.ElementName;
			}

		}


		private IStringTypeConverter getConverter()
		{
			var attr = _memberInfo.GetCustomAttributes(typeof(StringTypeConverterAttribute), false);
			if (attr.Length > 0)
				return (attr[0] as StringTypeConverterAttribute).CreateConverter();
			return ReflectionFacade.GetStringTypeConverter(_type);
		}


		/*public void SetValue(object obj, string value)
        {
            object data = _converter.ConvertFromString(null, CultureInfo.InvariantCulture, value);
            setValue(obj, data);
        }*/

		private Type getType()
		{
			if (_memberInfo is PropertyInfo)
				return (_memberInfo as PropertyInfo).PropertyType;
			if (_memberInfo is FieldInfo)
				return (_memberInfo as FieldInfo).FieldType;
			return null;
		}
		internal void setValue(object obj, object data)
		{
			if (_memberInfo is PropertyInfo)
			{
				(_memberInfo as PropertyInfo).SetValue(obj, data, null);
				return;
			}
			if (_memberInfo is FieldInfo)
			{
				(_memberInfo as FieldInfo).SetValue(obj, data);
				return;
			}
		}

		internal object getValue(object obj)
		{
			if (_memberInfo is PropertyInfo)
			{
				return (_memberInfo as PropertyInfo).GetValue(obj, null);
			}
			if (_memberInfo is FieldInfo)
			{
				return (_memberInfo as FieldInfo).GetValue(obj);
			}
			return null;
		}

		public string GetName()
		{
			return _memberInfo.Name;
		}

		public void SetValue(object obj, string value, Type type)
		{
			var converter = _converter;
			if (type != null)
				converter = ReflectionFacade.GetStringTypeConverter(type);
			object data = converter.ConvertFromString(new CustomTypeDescriptorContext() { _instance = obj }, CultureInfo.InvariantCulture, value);
			setValue(obj, data);
		}

		public void Deserialize(object obj, IDataReader c, ICustomTypeManager typeManager, string subType)
		{
			object data = null;
			if (subType == null)
				data = Activator.CreateInstance(_type);
			else
				data = ReflectionFacade.CreateObject(subType, typeManager);
			Serializer.Deserialize(data, c, typeManager);
			setValue(obj, data);
		}

		internal bool IsSerializeble()
		{
			if (_memberInfo is PropertyInfo)
			{
				var pi = (_memberInfo as PropertyInfo);
				if (!pi.CanRead || !pi.CanWrite)
					return false;
				return true;
			}
			if (_memberInfo is FieldInfo)
			{
				return (_memberInfo as FieldInfo).IsPublic;
			}
			return false;
		}

		internal object createEmpty()
		{
			return Activator.CreateInstance(_type);
		}

		internal bool isSerialize = true;
		public bool IsSerialize()
		{
			//!!!!!!!!1
			return isSerialize;
		}
		internal object defaultValue = null;
		public bool IsDefault(object obj)
		{
			object v = getValue(obj);
			if (v == null)
				return defaultValue == null;
			return (v.Equals(defaultValue));
			//return false;

		}

		public bool IsAttribute()
		{
			return _converter != null;
		}

		public string GetValue(object obj)
		{
			return _converter.ConvertToString(null, CultureInfo.InvariantCulture, getValue(obj));
		}

		public bool IsCollection()
		{
			//!!!!
			if (iCollectionType.IsAssignableFrom(_type))
				return true;
			return false;
		}
		//Dictionary<Type, string> aliases = new Dictionary<Type, string>();
		public IEnumerable<ChildObject> GetChilds(object obj, ICustomTypeManager typeManager)
		{
			List<ChildObject> childs = new List<ChildObject>();
			Object data = GetObject(obj);
			if (data == null)
				return null;
			IList cur = data as IList;
			foreach (var o in cur)
			{
				if (o == null)
					continue;
				childs.Add(new ChildObject(o, getObjectName(o, typeManager)));
			}


			return childs;
		}


		public bool IsObject()
		{
			return !IsAttribute();
		}

		public object GetObject(object obj)
		{
			return getValue(obj);
		}
		public override string ToString()
		{
			return GetName();
		}


		public string GetName(object data, ICustomTypeManager typeManager)
		{

			Type type = data.GetType();
			if (_type == type)
				return GetName();
			return GetName() + "-" + getTypeName(type, typeManager);
		}
		//object defaultValue;
		internal void initDefault(object defaultObject)
		{
			defaultValue = getValue(defaultObject);
		}

		static Type iCollectionType = typeof(ICollection);
		static Type iListType = typeof(IList);
		static Type iDictionaryType = typeof(IDictionary);
	}
}

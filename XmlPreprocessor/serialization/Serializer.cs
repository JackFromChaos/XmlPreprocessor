using dio.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dio.serialization
{
	public class Serializer
	{
		public static void Serialize(IDataWriter writer, Object obj, ICustomTypeManager typeManager = null)
		{
			Serialize(writer, obj, null, typeManager);
		}
		public static void Serialize(IDataWriter writer, Object obj, string name = null, ICustomTypeManager typeManager = null)
		{
			Type t = obj.GetType();
			ISerializer m = ReflectionFacade.GetMetaData(t);
			if (string.IsNullOrEmpty(name))
				name = ReflectionFacade.GetTypeName(t, typeManager);
			m.Serialize(writer, obj, name, typeManager);
		}
		public static T Deserialize<T>(IDataReader reader, ICustomTypeManager typeManager = null)
		{
			return (T)Deserialize(reader, typeof(T), typeManager);
		}
		public static object Deserialize(IDataReader reader, Type type, ICustomTypeManager typeManager = null)
		{
			object ret = Activator.CreateInstance(type);
			Deserialize(ret, reader, typeManager);
			return ret;
		}
		public static object Deserialize(IDataReader reader, ICustomTypeManager typeManager = null)
		{
			string typeName = reader.GetName();
			if (string.IsNullOrEmpty(typeName))
				return null;
			object ret = ReflectionFacade.CreateObject(typeName, typeManager);
			if (ret == null)
				return null;
			Deserialize(ret, reader, typeManager);
			return ret;

		}

		public static void Deserialize(object obj, IDataReader reader, ICustomTypeManager typeManager)
		{
			IDeserializer m = ReflectionFacade.GetMetaData(obj.GetType());
			m.Deserialize(obj, reader, typeManager);
		}

	}
}

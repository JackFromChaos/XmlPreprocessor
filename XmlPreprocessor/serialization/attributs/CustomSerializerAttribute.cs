using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dio.serialization.attributs
{
	public class CustomSerializerAttribute : Attribute
	{
		Type _type;
		public CustomSerializerAttribute(Type t)
		{
			_type = t;
		}
		public ISerializer CreateSerializer()
		{
			return Activator.CreateInstance(_type) as ISerializer;
		}
		public IDeserializer CreateDeserializer()
		{
			return Activator.CreateInstance(_type) as IDeserializer;
		}
	}
}

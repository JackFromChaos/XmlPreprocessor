using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dio.serialization.attributs
{
	public class StringTypeConverterAttribute : Attribute
	{
		Type _type;
		//string _typeName;
		public StringTypeConverterAttribute(Type t)
		{
			_type = t;
		}
		public IStringTypeConverter CreateConverter()
		{
			return Activator.CreateInstance(_type) as IStringTypeConverter;
		}
		/*public StringConverterAttribute(string typeName)
        {
            _typeName = typeName;
        }*/
	}
}

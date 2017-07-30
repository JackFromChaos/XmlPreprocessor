using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dio.serialization.attributs
{
	public class ChildAdderAttribute : Attribute
	{
		Type _type;

		public Type Type
		{
			get { return _type; }
		}
		public ChildAdderAttribute(Type t)
		{
			_type = t;
		}
		public ChildAdderAttribute()
		{
			_type = typeof(object);
		}
	}
}

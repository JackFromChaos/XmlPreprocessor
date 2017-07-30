using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dio.serialization.attributs
{
	public class SerializationGroupAttribute : Attribute
	{
		public string name;
		public SerializationGroupAttribute(string name)
		{
			this.name = name;
		}
	}
}

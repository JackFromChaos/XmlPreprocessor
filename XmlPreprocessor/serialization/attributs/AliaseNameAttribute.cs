using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dio.serialization.attributs
{
	public class AliaseNameAttribute : Attribute
	{
		public string name;
		public AliaseNameAttribute(string name)
		{
			this.name = name;
		}

	}
}

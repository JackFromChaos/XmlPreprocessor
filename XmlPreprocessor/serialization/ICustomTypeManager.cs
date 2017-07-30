using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dio.serialization
{
	public interface ICustomTypeManager
	{
		Type GetType(string name);
		string GetTypeName(Type t);
	}
}

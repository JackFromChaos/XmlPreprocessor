using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dio.serialization
{
	public class SimpleCustomTypeManager : Dictionary<string, Type>, ICustomTypeManager
	{
		/*public void AddType(string name, Type type)
        {
            this[name] = type;
        }*/
		public void Register<T>(string name)
		{
			this[name] = typeof(T);
		}

		public Type GetType(string name)
		{
			Type ret;
			TryGetValue(name, out ret);
			return ret;
		}


		public string GetTypeName(Type t)
		{
			foreach (var k in this)
				if (k.Value == t)
					return k.Key;
			return null;
		}
	}
}

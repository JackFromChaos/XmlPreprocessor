using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace dio.serialization
{
	public static class ReflectionUtils
	{
		public static List<T> getAttributes<T>(MemberInfo mi, bool inherit)
		{
			var arr = mi.GetCustomAttributes(typeof(T), inherit);
			List<T> ret = new List<T>();
			foreach (T a in arr)
				ret.Add(a);
			return ret;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace dio.serialization
{
	internal class MethodAdderField : IChildAdder
	{
		internal MethodAdderField(MethodInfo method, Type type)
		{
			this.method = method;
			this.type = type;
		}
		internal MethodInfo method;
		internal Type type;
		public void AddChild(object parent, object child)
		{
			method.Invoke(parent, new object[] { child });
		}

		public Type GetChildsType()
		{
			return type;
		}
	}
}

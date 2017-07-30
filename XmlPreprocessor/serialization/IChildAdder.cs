using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dio.serialization
{
	public interface IChildAdder
	{
		void AddChild(object parent, object child);
		Type GetChildsType();
	}
}

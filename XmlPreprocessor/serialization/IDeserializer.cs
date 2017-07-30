using dio.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dio.serialization
{
	public interface IDeserializer
	{
		void Deserialize(object obj, IDataReader reader, ICustomTypeManager typeManager);
	}
}

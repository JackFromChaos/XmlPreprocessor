using dio.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dio.serialization
{
	public interface ISerializer
	{
		void Serialize(IDataWriter writer, object obj, string name, ICustomTypeManager typeManager);

		//bool isSerializable();
		//bool isDeserializable();

	}
}

using dio.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dio.serialization
{
	public interface IFieldMetaData
	{
		string GetName();
		void SetValue(object obj, string value, Type type = null);
		void Deserialize(object obj, IDataReader c, ICustomTypeManager typeManager, string subType);

		bool IsSerialize();

		bool IsDefault(object obj);

		bool IsAttribute();

		string GetValue(object obj);

		bool IsCollection();

		IEnumerable<ChildObject> GetChilds(object obj, ICustomTypeManager typeManager);

		bool IsObject();

		object GetObject(object obj);

		string GetName(object data, ICustomTypeManager typeManager);
	}
}

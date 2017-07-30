using dio.data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dio.serialization
{
	public class CollectionFieldMetaData : IFieldMetaData
	{
		//private FieldInfo field;
		private FieldMetaData f;
		private string p;
		private Type type;

		public CollectionFieldMetaData(FieldMetaData f, string p, Type type)
		{
			// TODO: Complete member initialization
			//this.field = field;
			this.f = f;
			this.p = p;
			this.type = type;
		}
		//public CollectionFieldMetaData(MetaD
		public string GetName()
		{
			return p;
		}

		public void SetValue(object obj, string value, Type type = null)
		{
			throw new NotImplementedException();
		}

		public void Deserialize(object obj, IDataReader c, ICustomTypeManager typeManager, string subType)
		{
			object arr = f.getValue(obj);
			if (arr == null)
			{
				arr = f.createEmpty();
				f.setValue(obj, arr);
			}
			IList list = arr as IList;
			Type type = this.type;
			if (subType != null)
				type = ReflectionFacade.GetType(subType, typeManager);
			//Hashtable
			//ArrayList
			object data = Activator.CreateInstance(type);
			Serializer.Deserialize(data, c, typeManager);
			list.Add(data);
		}


		public bool IsSerialize()
		{
			return false;
		}

		public bool IsDefault(object obj)
		{
			throw new NotImplementedException();
		}

		public bool IsAttribute()
		{
			throw new NotImplementedException();
		}

		public string GetValue(object obj)
		{
			throw new NotImplementedException();
		}

		public bool IsCollection()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<ChildObject> GetChilds(object obj, ICustomTypeManager typeManager)
		{
			throw new NotImplementedException();
		}

		public bool IsObject()
		{
			throw new NotImplementedException();
		}

		public object GetObject(object obj)
		{
			throw new NotImplementedException();
		}


		public string GetName(object data, ICustomTypeManager typeManager)
		{
			throw new NotImplementedException();
		}
	}
}

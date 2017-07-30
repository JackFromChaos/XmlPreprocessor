using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dio.serialization
{
	internal class ListAdderField : IChildAdder
	{
		internal FieldMetaData listField;
		internal Type type;
		public ListAdderField(FieldMetaData listField, Type type)
		{
			this.listField = listField;
			this.type = type;
		}

		public Type GetChildsType()
		{
			return type;
		}

		public void AddChild(object parent, object child)
		{
			IList list = listField.getValue(parent) as IList;
			if (list == null)
			{
				list = listField.createEmpty() as IList;
				listField.setValue(parent, list);
			}
			list.Add(child);
		}
	}
}

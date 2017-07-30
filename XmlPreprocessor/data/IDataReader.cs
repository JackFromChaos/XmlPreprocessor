using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dio.data
{
	public interface IDataReader : IEnumerable
	{
		IEnumerable<IDataAttribute> GetAttributes();
		string GetName();
		string GetText();
		string GetAttribute(string p);
	}
}

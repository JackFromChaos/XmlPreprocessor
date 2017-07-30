using System;

namespace dio.data
{
	public interface IDataWriter
	{
		void SetAttribute(string name, string value);
		IDataWriter AddWriter(string name);
	}
}

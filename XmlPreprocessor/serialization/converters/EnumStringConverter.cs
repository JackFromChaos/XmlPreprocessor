using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dio.serialization.converters
{
	public struct EnumStringConverter : IStringTypeConverter
	{
		Type t;
		public EnumStringConverter(Type type)
		{
			t = type;
		}
		public string ConvertToString(System.ComponentModel.ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			return value.ToString();
		}

		public object ConvertFromString(ITypeDescriptorContext context, CultureInfo culture, string value)
		{
			return Enum.Parse(t, value);
		}
	}
}

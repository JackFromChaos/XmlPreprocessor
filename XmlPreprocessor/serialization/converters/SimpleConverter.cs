using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dio.serialization.converters
{
	public struct SimpleConverter : IStringTypeConverter
	{
		public delegate string dConvertToString(object value, CultureInfo culture);
		public delegate object dConvertFromString(string value, CultureInfo culture);
		public dConvertToString toString;
		public dConvertFromString fromString;
		//public string ConvertToString(object value,CultureInfo culture)
		public string ConvertToString(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			return toString(value, culture);
		}

		public object ConvertFromString(ITypeDescriptorContext context, CultureInfo culture, string value)
		{
			return fromString(value, culture);
		}
	}
}

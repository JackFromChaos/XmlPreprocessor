using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dio.serialization.converters
{
	public class ListStringCommaSplitterTypeConverter : IStringTypeConverter
	{

		public string ConvertToString(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			return string.Join(",", ((List<string>)value).ToArray());
		}

		public object ConvertFromString(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, string value)
		{
			var arr = value.Split(',');
			return arr.ToList();
		}
	}
}

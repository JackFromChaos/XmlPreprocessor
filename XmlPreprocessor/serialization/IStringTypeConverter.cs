using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dio.serialization
{
	public interface IStringTypeConverter
	{
		string ConvertToString(ITypeDescriptorContext context, CultureInfo culture, object value);
		object ConvertFromString(ITypeDescriptorContext context, CultureInfo culture, string value);
	}
}

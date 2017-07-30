using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dio.serialization
{
	internal struct CustomTypeDescriptorContext : System.ComponentModel.ITypeDescriptorContext
	{
		internal object _instance;
		public System.ComponentModel.IContainer Container
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public object Instance
		{
			get
			{
				return _instance;
			}
		}

		public void OnComponentChanged()
		{
			throw new NotImplementedException();
		}

		public bool OnComponentChanging()
		{
			throw new NotImplementedException();
		}

		public System.ComponentModel.PropertyDescriptor PropertyDescriptor
		{
			get { throw new NotImplementedException(); }
		}

		public object GetService(Type serviceType)
		{
			throw new NotImplementedException();
		}
	}
}

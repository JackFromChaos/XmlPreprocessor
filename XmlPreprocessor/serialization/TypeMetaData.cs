using dio.data;
using dio.serialization.attributs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace dio.serialization
{
	public class TypeMetaData : IMetaData
	{
		Type type;
		protected object defaultObject;
		protected Dictionary<string, IFieldMetaData> _fields = new Dictionary<string, IFieldMetaData>();

		public TypeMetaData(Type t)
		{
			type = t;
			defaultObject = Activator.CreateInstance(t);
			foreach (var field in t.GetFields())
			{
				FieldMetaData f = new FieldMetaData(field);
				f.initDefault(defaultObject);
				if (f.IsSerializeble())
				{
					_fields[f.GetName()] = f;
					checkFieldAttributes(field, f);
				}
			}
			foreach (var field in t.GetProperties())
			{
				FieldMetaData f = new FieldMetaData(field);
				if (f.IsSerializeble())
				{
					_fields[f.GetName()] = f;
					checkFieldAttributes(field, f);
				}
			}
			foreach (var method in t.GetMethods())
			{
				//method.GetCustomAttributes(
				var childAdderAttributes = ReflectionUtils.getAttributes<ChildAdderAttribute>(method, true);
				foreach (var a in childAdderAttributes)
				{
					childAdders.Insert(0, new MethodAdderField(method, a.Type));

					//_fields[a.ElementName] = new CollectionFieldMetaData(f, a.ElementName, a.Type);
				}
				var completes = ReflectionUtils.getAttributes<SerializerCompleteAttribute>(method, true);
				if (completes.Count > 0)
				{
					if (_completes == null)
						_completes = new List<MethodInfo>();
					_completes.Add(method);
				}
			}
		}
		List<MethodInfo> _completes;
		List<IChildAdder> childAdders = new List<IChildAdder>();
		private void checkFieldAttributes(MemberInfo field, FieldMetaData f)
		{
			var atr = ReflectionUtils.getAttributes<System.Xml.Serialization.XmlElementAttribute>(field, true);
			foreach (var a in atr)
				_fields[a.ElementName] = new CollectionFieldMetaData(f, a.ElementName, a.Type);
			var childAdderAttributes = ReflectionUtils.getAttributes<ChildAdderAttribute>(field, true);
			foreach (var a in childAdderAttributes)
			{
				childAdders.Add(new ListAdderField(f, a.Type));

				//_fields[a.ElementName] = new CollectionFieldMetaData(f, a.ElementName, a.Type);
			}

		}


		public IEnumerable<IFieldMetaData> Fields { get { return _fields.Values; } }

		internal IFieldMetaData GetField(string p)
		{
			IFieldMetaData ret;
			_fields.TryGetValue(p, out ret);
			return ret;
			//return _fields[p];
		}
		internal IChildAdder GetPolimorfField(Type t)
		{
			foreach (var ca in childAdders)
				if (ca.GetChildsType().IsAssignableFrom(t))
					return ca;
			return null;
		}

		public void DeserializeAttributes(object obj, IDataReader reader, ICustomTypeManager typeManager)
		{
			string name;
			string subType = null;
			foreach (var a in reader.GetAttributes())
			{

				calcNameAndType(a.GetName(), out name, out subType);
				IFieldMetaData field = GetField(name);
				//if(name=="autuRuning")
				//Console.WriteLine("!!");
				if (field == null)
					continue;
				if (subType != null)
					field.SetValue(obj, a.GetValue(), ReflectionFacade.GetType(subType, typeManager));
				else
					field.SetValue(obj, a.GetValue());
			}
		}

		public void Deserialize(object obj, IDataReader reader, ICustomTypeManager typeManager)
		{


			DeserializeAttributes(obj, reader, typeManager);

			string name;
			string subType = null;


			foreach (IDataReader c in reader)
			{
				calcNameAndType(c.GetName(), out name, out subType);
				IFieldMetaData field = GetField(name);
				if (field != null)
				{
					field.Deserialize(obj, c, typeManager, subType);
				}
				else
				{
					if (subType == null)
					{

						Type t = ReflectionFacade.GetType(name);
						IChildAdder ca = GetPolimorfField(t);
						if (ca != null)
						{
							var child = Serializer.Deserialize(c, t, typeManager);
							ca.AddChild(obj, child);
							//field.Deserialize(obj, c, typeManager, name);
							continue;
						}
					}
					/*if (subType != null)
                    {
                        Type t = typeManager.GetType(subType);
                    }*/

					//
					///Отдельная история. Надо думать!
				}

			}

			if (_completes != null)
				foreach (var m in _completes)
					m.Invoke(obj, null);
		}

		public void Serialize(IDataWriter writer, object obj, string name, ICustomTypeManager typeManager)
		{
			writer = writer.AddWriter(name);

			foreach (var f in Fields)
			{
				if (!f.IsSerialize())
					continue;
				if (!f.IsCollection() && f.IsDefault(obj))
					continue;
				if (f.IsAttribute())
				{
					writer.SetAttribute(f.GetName(), f.GetValue(obj));
				}
				else if (f.IsCollection())
				{
					IEnumerable<ChildObject> childs = f.GetChilds(obj, typeManager);
					if (childs == null)
						continue;
					foreach (ChildObject child in childs)
					{
						Serializer.Serialize(writer, child.data, child.name, typeManager);
					}
				}
				else if (f.IsObject())
				{
					//IDataWriter childWriter = writer.AddWriter(f.GetName());
					object data = f.GetObject(obj);
					Serialize(writer, data, f.GetName(data, typeManager), typeManager);
				}
			}
		}

		internal static bool calcNameAndType(string name, out string realName, out string subType)
		{

			//string name = a.GetName();
			//string subType=null;
			int k = name.IndexOf('-');
			if (k > 0)
			{
				subType = name.Substring(k + 1);
				realName = name.Substring(0, k);
				return true;
			}
			subType = null;
			realName = name;
			return false;

		}




	}
	public struct ChildObject
	{
		internal object data;
		internal string name;

		public ChildObject(object o, string p)
		{
			// TODO: Complete member initialization
			this.data = o;
			this.name = p;
		}
	}
}

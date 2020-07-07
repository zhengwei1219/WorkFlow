using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Configuration;
using System.Collections;
using System.Xml.Serialization;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 
	/// </summary>
	public static class ConfigurationHelper
	{
		/// <summary>
		/// ʵ����һ������ʵ��
		/// </summary>
		/// <param name="fullName">����ȫ���ƣ����ֿռ�.����,���򼯣�</param>
		/// <returns></returns>
		public static object LoadTypeInstance(string fullName)
		{
			if (string.IsNullOrEmpty(fullName))
				throw new InvalidOperationException("empty type");
			string[] typeInfo = fullName.Split(",".ToCharArray());
			if (typeInfo.Length < 2)
				throw new TypeLoadException(string.Format("Invalid type \"{0}\"", fullName));
			bool hasDefaultConstructor = false;
			Type type = Type.GetType(fullName);
			if (type == null)
				throw new TypeLoadException(string.Format("Invalid type \"{0}\"", fullName));
			//�ж��Ƿ��й������޲ι��캯��
			ConstructorInfo[] cInfos = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
			if (cInfos != null && cInfos.Length > 0)
			{
				foreach (ConstructorInfo c in cInfos)
				{
					if (c.GetParameters().Length == 0)
					{
						hasDefaultConstructor = true;
						break;
					}
				}
			}
			if (!hasDefaultConstructor)
				throw new TypeLoadException(string.Format("type \"{0}\" does not have any No-arg constructor! ", fullName));
			//���䴴��һ��ʵ��
			object obj = Assembly.Load(typeInfo[1]).CreateInstance(typeInfo[0]);
			if (obj == null)
				throw new TypeLoadException(string.Format("Invalid type \"{0}\"", fullName));
			return obj;
		}
		/// <summary>
		/// ��TypeItem��ʵ����һ������
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns></returns>
		public static object LoadType(TypeItem item)
		{
			//����ʵ��
			object obj = LoadTypeInstance(item.Type);
			//���ò���
			if (item.Params != null)
			{
				foreach (ParamItem p in item.Params)
				{
					SetParamPropValue(obj, p);
				}
			}
			//�������Ҫ�ݹ鴴�����������Ը�ֵ
			if (item.Types != null)
			{
				foreach (TypeItem t in item.Types)
				{
					PropertyInfo pInfo = obj.GetType().GetProperty(t.Name);
					if (pInfo == null)
						throw new ApplicationException(string.Format("Property '{0}' can not found, type {1}", t.Name, obj.GetType()));
					pInfo.SetValue(obj, LoadType(t), null);
				}
			}
			return obj;
		}
		/// <summary>
		/// �������������ֵ
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <param name="item">The item.</param>
		public static void SetParamPropValue(object obj, ParamItem item)
		{
			PropertyInfo pInfo = obj.GetType().GetProperty(item.Name);
			if (pInfo == null)
				throw new ApplicationException(string.Format("Property '{0}' can not found, type {1}", item.Name, obj.GetType()));
			if (pInfo.PropertyType.GetInterface(typeof(IConvertible).Name) == null)
				throw new ApplicationException(string.Format("property's type \"{0}\" does not implement interface IConvertable , so it cannot be a param element!", pInfo.PropertyType.FullName));
			object param = Convert.ChangeType(item.Value, pInfo.PropertyType);
			if (pInfo != null)
				pInfo.SetValue(obj, param, null);
		}
	}
	/// <summary>
	/// �������л����Ͳ�������
	/// </summary>
	[XmlRoot(ElementName = "type")]
	public class TypeItem
	{
		/// <summary>
		/// 
		/// </summary>
		[XmlAttribute("name")]
		public string Name;
		/// <summary>
		/// 
		/// </summary>
		[XmlAttribute("type")]
		public string Type;
		/// <summary>
		/// 
		/// </summary>
		[XmlElement("type")]
		public TypeItem[] Types;
		/// <summary>
		/// 
		/// </summary>
		[XmlElement("param")]
		public ParamItem[] Params;
	}
	/// <summary>
	/// ����ʵ�������ݲ�������
	/// </summary>
	[XmlRoot(ElementName = "param")]
	public class ParamItem
	{
		/// <summary>
		/// 
		/// </summary>
		[XmlAttribute("name")]
		public string Name;
		/// <summary>
		/// 
		/// </summary>
		[XmlAttribute("value")]
		public string Value;
	}
}
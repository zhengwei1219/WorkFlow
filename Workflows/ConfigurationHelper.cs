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
		/// 实例化一个类型实例
		/// </summary>
		/// <param name="fullName">类型全名称（名字空间.名称,程序集）</param>
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
			//判断是否有公共的无参构造函数
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
			//反射创建一个实例
			object obj = Assembly.Load(typeInfo[1]).CreateInstance(typeInfo[0]);
			if (obj == null)
				throw new TypeLoadException(string.Format("Invalid type \"{0}\"", fullName));
			return obj;
		}
		/// <summary>
		/// 从TypeItem中实例化一个对象
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns></returns>
		public static object LoadType(TypeItem item)
		{
			//创建实例
			object obj = LoadTypeInstance(item.Type);
			//设置参数
			if (item.Params != null)
			{
				foreach (ParamItem p in item.Params)
				{
					SetParamPropValue(obj, p);
				}
			}
			//对象参数要递归创建对象后给属性赋值
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
		/// 给对象添加属性值
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
	/// 用来序列化类型参数的类
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
	/// 用来实例化数据参数的类
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
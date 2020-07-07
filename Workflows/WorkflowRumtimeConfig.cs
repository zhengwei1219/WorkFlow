using System;
using System.Data;
using System.Configuration;
using System.Configuration.Provider;
using System.Collections.Specialized;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.XPath;
using System.IO;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 工作流运行时的配置
	/// </summary>
	internal class WorkflowRuntimeConfig : IConfigurationSectionHandler
	{
		#region IConfigurationSectionHandler Members

		public object Create(object parent, object configContext, XmlNode section)
		{
			string xml = section.InnerXml;
			XmlDocument xDoc = new XmlDocument();
			xDoc.LoadXml(xml);
			XmlNodeList nodes = xDoc.SelectNodes("//services/type");
			WorkflowRuntime runtime = new WorkflowRuntime();
			foreach (XmlNode node in nodes)
			{
				if (node.NodeType == XmlNodeType.Element)
				{
					StringReader sr = new StringReader(node.OuterXml);
					using (XmlTextReader xr = new XmlTextReader(sr))
					{
						XmlSerializer serializer = new XmlSerializer(typeof(TypeItem));
						TypeItem item = (TypeItem)serializer.Deserialize(xr);
						runtime.AddService(ConfigurationHelper.LoadType(item));
					}
				}
			}
			runtime.StartRuntime();
			return runtime;
		}

		#endregion
	}
}

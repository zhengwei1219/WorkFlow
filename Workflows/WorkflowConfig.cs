using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace OilDigital.Workflows
{
	/// <summary>
	/// ������������
	/// </summary>
	[Serializable]
	public class WorkflowConfig
	{
		private WorkflowApplication[] applications;
		/// <summary>
		/// Gets or sets the applications.
		/// </summary>
		/// <value>The applications.</value>
		[XmlElement("application")]
		public WorkflowApplication[] Applications
		{
			get { return applications; }
			set { applications = value; }
		}
		/// <summary>
		/// ��Ӧ�����Ʋ��ҹ�����Ӧ������.
		/// </summary>
		/// <param name="applicationName">Name of the application.</param>
		/// <returns></returns>
		public WorkflowApplication Get(string applicationName)
		{
			if (string.IsNullOrEmpty(applicationName))
				throw new ArgumentNullException("applicationName");
			foreach (WorkflowApplication application in Applications)
			{
				if (application.Name.ToUpper() == applicationName.ToUpper())
					return application;
			}
			return null;
		}

		/// <summary>
		/// �����������Ʋ��ҹ�����Ӧ������.
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <returns></returns>
		public WorkflowApplication FindByWorkFlowName(string workflowName)
		{
			if (string.IsNullOrEmpty(workflowName))
				throw new ArgumentNullException("workflowName");

			foreach (WorkflowApplication application in Applications)
			{
				foreach (WorkflowDefineConfig one in application.Defines)
				{
					if (one.Name.Equals(workflowName)) return application;
				}
			}
			return null;
		}
	}

	/// <summary>
	/// ������Ӧ����,ÿ��Ӧ�ó�������ж��������Ӧ��.
	/// </summary>
	[Serializable]
	public class WorkflowApplication
	{

		private string name;
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		[XmlAttribute("name")]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		private string root;
		/// <summary>
		/// Gets or sets Root(��ʼ)����������
		/// </summary>
		/// <value>The root.</value>
		[XmlAttribute("root")]
		public string Root
		{
			get { return root; }
			set { root = value; }
		}

		private List<WorkflowDefineConfig> defines;
		/// <summary>
		/// Gets or sets the defines.
		/// </summary>
		/// <value>The defines.</value>
		[XmlElement("define")]
		public List<WorkflowDefineConfig> Defines
		{
			get { return defines; }
			set { defines = value; }
		}
	}

	/// <summary>
	/// ��������������
	/// </summary>
	[Serializable]
	public class WorkflowDefineConfig
	{
		private string name;

		/// <summary>
		/// ��������������
		/// </summary>
		[XmlAttribute("name")]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		private string file;

		/// <summary>
		/// ��Ӧ��xml�ļ�
		/// </summary>
		[XmlAttribute("file")]
		public string File
		{
			get { return file; }
			set { file = value; }
		}

		private bool enabled;

		/// <summary>
		/// �Ƿ����
		/// </summary>
		[XmlAttribute("enabled")]
		public bool Enabled
		{
			get { return enabled; }
			set { enabled = value; }
		}
	}
}

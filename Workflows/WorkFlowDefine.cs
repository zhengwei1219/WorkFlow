using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class WorkFlowDefine
	{
		private Guid id;
		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		[XmlAttribute()]
		public Guid Id
		{
			get { return id; }
			set { id = value; }
		}

		private string name;
		/// <summary>
		/// ����
		/// </summary>
		[XmlAttribute()]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		private string description;
		/// <summary>
		/// ����
		/// </summary>
		[XmlAttribute()]
		public string Description
		{
			get { return description; }
			set { description = value; }
		}

		private WorkflowApplication application;

		/// <summary>
		/// Gets or sets �ù�������Ӧ������.
		/// </summary>
		/// <value>The application.</value>
		public WorkflowApplication Application
		{
			get { return application; }
			set { application = value; }
		}

	}
}

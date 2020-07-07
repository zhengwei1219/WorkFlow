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
		/// 名称
		/// </summary>
		[XmlAttribute()]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		private string description;
		/// <summary>
		/// 描述
		/// </summary>
		[XmlAttribute()]
		public string Description
		{
			get { return description; }
			set { description = value; }
		}

		private WorkflowApplication application;

		/// <summary>
		/// Gets or sets 该工作流对应的引用.
		/// </summary>
		/// <value>The application.</value>
		public WorkflowApplication Application
		{
			get { return application; }
			set { application = value; }
		}

	}
}

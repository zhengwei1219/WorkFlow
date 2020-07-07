using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 触发时间角色类
	/// </summary>
    [Serializable]
    public class EventRole
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="EventRole"/> class.
		/// </summary>
		public EventRole()
		{ }

		private string name;
		/// <summary>
		/// 角色名称
		/// </summary>
		[XmlAttribute()]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		private string taskName;
		/// <summary>
		/// 任务名称
		/// </summary>
		[XmlAttribute()]
		public string TaskName
		{
			get { return taskName; }
			set { taskName = value; }
		}

		private string linkPage;
		/// <summary>
		/// 任务办理页面
		/// </summary>
		[XmlAttribute()]
		public string LinkPage
		{
			get { return linkPage; }
			set { linkPage = value; }
		}

		private TimeSpan timeSpan;

		/// <summary>
		/// 任务过期时间
		/// </summary>
		[XmlIgnore()]
		public TimeSpan ExceedTime
		{
			get { return timeSpan; }
			set { timeSpan = value; }
		}

		/// <summary>
		/// 任务过期时间字符串表示（用于序列化TimeSpan）
		/// </summary>
		[XmlAttribute("ExceedTime")]
		public string ExceedTimeString
		{
			get { return timeSpan.ToString(); }
			set { timeSpan = TimeSpan.Parse(value); }
		}
    }
}
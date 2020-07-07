using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 审批事件类
	/// </summary>
	[Serializable]
	public class ApprovalEvent
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ApprovalEvent"/> class.
		/// </summary>
		public ApprovalEvent(){}

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

		private List<EventRole> roles;
		/// <summary>
		/// 触发事件的角色集合
		/// </summary>
		[XmlArray("Roles")]
		[XmlArrayItem("EventRole")]
		public List<EventRole> Roles
		{
			get { return roles; }
			set { roles = value; }
		}

		private string authorization = "All";
		/// <summary>
		/// 允许触发该事件的权限限定
		/// </summary>
		[XmlAttribute()]
		public string Authorization
		{
			get { return authorization; }
			set { authorization = value; }
		}

		private string bindActivity;
		/// <summary>
		/// Gets or sets the bind activity.
		/// </summary>
		/// <value>The bind activity.</value>
		[XmlAttribute()]
		public string BindActivity
		{
			get { return bindActivity; }
			set { bindActivity = value; }
		}
	
		/// <summary>
		/// 分号分隔的下一步状态列表
		/// </summary>
		[XmlAttribute("NextStateNames")]
		public string NextStateNamesString
		{
			get 
			{
				StringBuilder sb = new StringBuilder();
				foreach (string stateName in this.nextStateNames)
				{
					sb.Append(stateName + ";");
				}
				return sb.ToString().Trim(';');
			}
			set 
			{
				if (value == null)
					throw new ArgumentNullException("NextStateNamesString");
				nextStateNames = value.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				if (nextStateNames.Length == 0)
					nextStateNames = new string[1] { "" };
			}
		}

		private string[] nextStateNames;
		/// <summary>
		/// 下一步状态名称集合
		/// </summary>
		[XmlIgnore()]
		public string[] NextStateNames
		{
			get { return nextStateNames; }
		}

		private ApprovalEventParameter parameter = ApprovalEventParameter.Empty;
		/// <summary>
		/// Gets or sets 和此审批事件相关的外部配置参数
		/// </summary>
		/// <value>The prameter.</value>
		[XmlElement("Parameter")]
		public ApprovalEventParameter Parameter
		{
			get { return parameter; }
			set { parameter = value; }
		}
	}

	/// <summary>
	/// 时间参数类
	/// </summary>
	[Serializable()]
	public class ApprovalEventParameter
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ApprovalEventParameter"/> class.
		/// </summary>
		public ApprovalEventParameter() { }
		private bool needCheckValid = false;
		/// <summary>
		/// Gets or sets 是否在执行动作之前先校验立项的有效性
		/// </summary>
		/// <value><c>true</c> if [need check valid]; otherwise, <c>false</c>.</value>
		[XmlAttribute("NeedCheckValid")]
		public bool NeedCheckValid
		{
			get { return needCheckValid; }
			set { needCheckValid = value; }
		}

		private bool emptySolutionAllowed = true;
		/// <summary>
		/// Gets or sets 是否允许不输入办理的意见,default=true
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [empty solution allowed]; otherwise, <c>false</c>.
		/// </value>
		[XmlAttribute("EmptySolutionAllowed")]
		public bool EmptySolutionAllowed
		{
			get { return emptySolutionAllowed; }
			set { emptySolutionAllowed = value; }
		}

		private string defaultSolutionInfo = "";
		/// <summary>
		/// Gets or sets 在意见输入框中缺省输入的意见.
		/// </summary>
		/// <value>The default solution info.</value>
		[XmlAttribute("DefaultSolutionInfo")]
		public string DefaultSolutionInfo
		{
			get { return defaultSolutionInfo; }
			set { defaultSolutionInfo = value; }
		}

		private MsgRemind reminder = MsgRemind.None;
		/// <summary>
		/// Gets or sets 短信提醒册方式
		/// </summary>
		/// <value>The reminder.</value>
		[XmlAttribute("Reminder")]
		public MsgRemind Reminder
		{
			get { return reminder; }
			set { reminder = value; }
		}

		private static ApprovalEventParameter empty = new ApprovalEventParameter();
		/// <summary>
		/// Gets the empty.
		/// </summary>
		/// <value>The empty.</value>
		public static ApprovalEventParameter Empty
		{
			get { return empty; }
		}
	}
	/// <summary>
	/// 短消息提醒方式
	/// </summary>
	[Flags]
	public enum MsgRemind
	{
		/// <summary>
		/// 否
		/// </summary>
		None = 0,
		/// <summary>
		/// 只有所有者
		/// </summary>
		Owner = 1,
		/// <summary>
		/// 只有操作者
		/// </summary>
		Processor = 2,
		/// <summary>
		/// 所有
		/// </summary>
		All = 3
	}
}

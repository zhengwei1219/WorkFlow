using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Configuration;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 审批服务配置.
	/// </summary>
	public class ApprovalServiceConfig
	{
		private ApplicationConfig[] applications;
		/// <summary>
		/// Gets or sets the applications.
		/// </summary>
		/// <value>The applications.</value>
		[XmlElement("application")]
		public ApplicationConfig[] Applications
		{
			get { return applications; }
			set { applications = value; }
		}
	}

	/// <summary>
	/// 应用配置类,定义每个具体工作流的呈现方式,处理方式
	/// </summary>
	public class ApplicationConfig
	{
		/// <summary>
		/// 应用名称
		/// </summary>
		[XmlAttribute("name")]
		public string Name;

		/// <summary>
		/// 访问审批对象的服务的配置
		/// </summary>
		[XmlElement("approvalObjectService")]
		public TypeItem ApprovalObjectService;

	
		/// <summary>
		/// 获取待办配置
		/// </summary>
		[XmlElement("currentlyService")]
		public ServiceConfig CurrentlyService;
		/// <summary>
		/// 获取允许撤销的任务列表时的各种参数
		/// </summary>
		[XmlElement("allowedCancelService")]
		public ServiceConfig AllowedCancelService;
		/// <summary>
		/// 
		/// </summary>
		[XmlElement("proceedService")]
		public ServiceConfig ProceedService;
		/// <summary>
		/// 
		/// </summary>
		[XmlElement("completedService")]
		public ServiceConfig CompletedService;
		/// <summary>
		/// 
		/// </summary>
		[XmlElement("processingService")]
		public ServiceConfig ProcessingService;
		/// <summary>
		/// 
		/// </summary>
		[XmlElement("agentService")]
		public ServiceConfig AgentService;
		/// <summary>
		/// 
		/// </summary>
		[XmlElement("agentConfig")]
		public AgentConfig AgentConfigInfo;
		/// <summary>
		/// 
		/// </summary>
		[XmlArray("customServices")]
		[XmlArrayItem("one")]
		public ServiceConfig[] Services;

		private TypeItem[] solutionFilters;

		/// <summary>
		/// Gets or sets 审批意见的过滤器
		/// </summary>
		/// <value>The solution filters.</value>
		[XmlArray("solutionFilter")]
		[XmlArrayItem("one")]
		public TypeItem[] SolutionFilters
		{
			get { return solutionFilters; }
			set { solutionFilters = value; }
		}


		private TypeItem[] recordFilters;

		/// <summary>
		/// Gets or sets 审批记录的过滤器配置
		/// </summary>
		/// <value>The solution filters.</value>
		[XmlArray("recordFilter")]
		[XmlArrayItem("one")]
		public TypeItem[] RecordFilters
		{
			get { return recordFilters; }
			set { recordFilters = value; }
		}

		/// <summary>
		/// 其他类型.
		/// </summary>
		[XmlElement("type")]
		public TypeItem[] Types;

	}

	/// <summary>
	/// 具体角色配置类,可配置为每个不同审批角色进行独立配置
	/// </summary>
	public class ServiceConfig
	{
		/// <summary>
		/// 配置名
		/// </summary>
		[XmlAttribute("name")]
		public string Name;
		/// <summary>
		/// <summary>
		/// 设定任务列列表的名称.在形成一个或多个任务列表时,是否制定任务列表的名称,如果为null,那么将使用任务列表自己的名称.
		/// 一般在合共所有角色时配置使用.
		/// </summary>
		/// </summary>
		[XmlAttribute("taskName")]
		public string TaskName;
		/// <summary>
		/// 如果用户具有多个审批角色,是否将所有角色的任务列表合并显示
		/// </summary>
		[XmlAttribute("allMerge")]
		public bool AllMerge;
		/// <summary>
		/// 缺省配置项目，在没有配置具体角色项目时使用。
		/// </summary>
		[XmlElement("Default")]
		public RoleConfigItem DefaultItem;
		/// <summary>
		///  每个角色的特定配置项目
		/// </summary>
		[XmlElement("role")]
		public RoleConfigItem[] Items;
	}

	/// <summary>
	/// 角色配置类项目
	/// </summary>
	public class RoleConfigItem
	{
		/// <summary>
		/// 角色名称
		/// </summary>
		[XmlAttribute("name")]
		public string Name;
		/// <summary>
		/// 各类提取器
		/// </summary>
		[XmlElement("distiller")]
		public TypeItem[] Distillers;
		/// <summary>
		/// 各类过滤器
		/// </summary>
		[XmlElement("filter")]
		public TypeItem[] Filters;
		/// <summary>
		/// 动作提取器
		/// </summary>
		[XmlElement("itemDistiller")]
		public TypeItem TaskItemDistiller;
		/// <summary>
		/// 列表呈现
		/// </summary>
		[XmlElement("render")]
		public TypeItem TaskRender;
	}
	/// <summary>
	/// 授权情况的配置类
	/// </summary>
	[Serializable()]
	public class AgentConfig
	{
		private AgentOption[] options;
		/// <summary>
		/// 授权情况配置项目集合
		/// </summary>
		/// <value>The options.</value>
		[XmlElement("option")]
		public AgentOption[] Options
		{
			get { return options; }
			set
			{
				options = value;
				optionsDic = new Dictionary<string, AgentOption>();
				foreach (AgentOption option in options)
				{
					if (optionsDic.ContainsKey(option.Name))
						throw new ConfigurationErrorsException(string.Format("配置文件有误,不能重复添加name为{0}的配置", option.Name));
					optionsDic.Add(option.Name, option);
				}
			}
		}
		private Dictionary<string, AgentOption> optionsDic;
		/// <summary>
		/// 用户或角色名称获取授权相关信息
		/// </summary>
		/// <param name="name">用户Id或角色</param>
		/// <returns></returns>
		public AgentOption GetOption(string name)
		{
			if (!optionsDic.ContainsKey(name))
				return null;
			return optionsDic[name];
		}
		/// <summary>
		/// Checks this instance.
		/// </summary>
		public void Check(string applicationName)
		{
			List<string> roles = new List<string>(WorkflowUtility.GetAllRoles(applicationName));
			foreach (string key in optionsDic.Keys)
			{
				string[] roleNames = optionsDic[key].RoleNames;
				if (roleNames != null && roleNames.Length < 0)
				{
					foreach (string roleName in roleNames)
					{
						if (!roles.Contains(roleName))
							throw new ConfigurationErrorsException(string.Format("AgentConfig 配置错误！ role '{0}' 不存在于工作流中！ ", roleName));
					}
				}
			}
		}
	}
	/// <summary>
	/// 代理配置类
	/// </summary>
	[Serializable()]
	public class AgentOption
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AgentOption"/> class.
		/// </summary>
		public AgentOption() { }
		private bool matchUnit = true;
		/// <summary>
		///是否只匹配相同单位人员
		/// </summary>
		/// <value><c>true</c> if [match unit]; otherwise, <c>false</c>.</value>
		[XmlAttribute("matchUnit")]
		public bool MatchUnit
		{
			get { return matchUnit; }
			set { matchUnit = value; }
		}

		private string name;
		/// <summary>
		/// 用户Id或者角色名称
		/// </summary>
		/// <value>The name.</value>
		[XmlAttribute("name")]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		private bool includeOther = true;
		/// <summary>
		/// 是否包含除定制人员外的其他所有人员
		/// </summary>
		[XmlAttribute("includeOther")]
		public bool IncludeOther
		{
			get { return includeOther; }
			set { includeOther = value; }
		}

		private string users;
		/// <summary>
		/// Gets or sets the users.
		/// </summary>
		/// <value>The users.</value>
		[XmlElement("user")]
		public string Users
		{
			get { return users; }
			set
			{
				users = value;
				this.userIds = users.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			}
		}

		private string[] userIds;
		/// <summary>
		/// Gets the user ids.
		/// </summary>
		/// <value>The user ids.</value>
		[XmlIgnore()]
		public string[] UserIds
		{
			get
			{
				if (userIds == null)
					return new List<string>().ToArray();
				return this.userIds;
			}
		}

		private string roles;
		/// <summary>
		/// Gets or sets the roles.
		/// </summary>
		/// <value>The roles.</value>
		[XmlElement("role")]
		public string Roles
		{
			get { return roles; }
			set
			{
				roles = value;
				this.roleNames = roles.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			}
		}

		private string[] roleNames;
		/// <summary>
		/// Gets the role names.
		/// </summary>
		/// <value>The role names.</value>
		[XmlIgnore()]
		public string[] RoleNames
		{
			get
			{
				if (roleNames == null)
					return new List<string>().ToArray();
				return this.roleNames;
			}
		}
	}
}

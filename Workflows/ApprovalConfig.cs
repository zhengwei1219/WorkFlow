using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Configuration;

namespace OilDigital.Workflows
{
	/// <summary>
	/// ������������.
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
	/// Ӧ��������,����ÿ�����幤�����ĳ��ַ�ʽ,����ʽ
	/// </summary>
	public class ApplicationConfig
	{
		/// <summary>
		/// Ӧ������
		/// </summary>
		[XmlAttribute("name")]
		public string Name;

		/// <summary>
		/// ������������ķ��������
		/// </summary>
		[XmlElement("approvalObjectService")]
		public TypeItem ApprovalObjectService;

	
		/// <summary>
		/// ��ȡ��������
		/// </summary>
		[XmlElement("currentlyService")]
		public ServiceConfig CurrentlyService;
		/// <summary>
		/// ��ȡ�������������б�ʱ�ĸ��ֲ���
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
		/// Gets or sets ��������Ĺ�����
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
		/// Gets or sets ������¼�Ĺ���������
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
		/// ��������.
		/// </summary>
		[XmlElement("type")]
		public TypeItem[] Types;

	}

	/// <summary>
	/// �����ɫ������,������Ϊÿ����ͬ������ɫ���ж�������
	/// </summary>
	public class ServiceConfig
	{
		/// <summary>
		/// ������
		/// </summary>
		[XmlAttribute("name")]
		public string Name;
		/// <summary>
		/// <summary>
		/// �趨�������б������.���γ�һ�����������б�ʱ,�Ƿ��ƶ������б������,���Ϊnull,��ô��ʹ�������б��Լ�������.
		/// һ���ںϹ����н�ɫʱ����ʹ��.
		/// </summary>
		/// </summary>
		[XmlAttribute("taskName")]
		public string TaskName;
		/// <summary>
		/// ����û����ж��������ɫ,�Ƿ����н�ɫ�������б�ϲ���ʾ
		/// </summary>
		[XmlAttribute("allMerge")]
		public bool AllMerge;
		/// <summary>
		/// ȱʡ������Ŀ����û�����þ����ɫ��Ŀʱʹ�á�
		/// </summary>
		[XmlElement("Default")]
		public RoleConfigItem DefaultItem;
		/// <summary>
		///  ÿ����ɫ���ض�������Ŀ
		/// </summary>
		[XmlElement("role")]
		public RoleConfigItem[] Items;
	}

	/// <summary>
	/// ��ɫ��������Ŀ
	/// </summary>
	public class RoleConfigItem
	{
		/// <summary>
		/// ��ɫ����
		/// </summary>
		[XmlAttribute("name")]
		public string Name;
		/// <summary>
		/// ������ȡ��
		/// </summary>
		[XmlElement("distiller")]
		public TypeItem[] Distillers;
		/// <summary>
		/// ���������
		/// </summary>
		[XmlElement("filter")]
		public TypeItem[] Filters;
		/// <summary>
		/// ������ȡ��
		/// </summary>
		[XmlElement("itemDistiller")]
		public TypeItem TaskItemDistiller;
		/// <summary>
		/// �б����
		/// </summary>
		[XmlElement("render")]
		public TypeItem TaskRender;
	}
	/// <summary>
	/// ��Ȩ�����������
	/// </summary>
	[Serializable()]
	public class AgentConfig
	{
		private AgentOption[] options;
		/// <summary>
		/// ��Ȩ���������Ŀ����
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
						throw new ConfigurationErrorsException(string.Format("�����ļ�����,�����ظ����nameΪ{0}������", option.Name));
					optionsDic.Add(option.Name, option);
				}
			}
		}
		private Dictionary<string, AgentOption> optionsDic;
		/// <summary>
		/// �û����ɫ���ƻ�ȡ��Ȩ�����Ϣ
		/// </summary>
		/// <param name="name">�û�Id���ɫ</param>
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
							throw new ConfigurationErrorsException(string.Format("AgentConfig ���ô��� role '{0}' �������ڹ������У� ", roleName));
					}
				}
			}
		}
	}
	/// <summary>
	/// ����������
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
		///�Ƿ�ֻƥ����ͬ��λ��Ա
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
		/// �û�Id���߽�ɫ����
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
		/// �Ƿ������������Ա�������������Ա
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

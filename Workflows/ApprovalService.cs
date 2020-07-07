using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.IO;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace OilDigital.Workflows
{
	/// <summary>
	/// �����������弯��
	/// </summary>
	public sealed class ApprovalServicesCollection:List<ApprovalService>
	{
		/// <summary>
		/// ��Ӧ�ó�������ƻ�ȡ���� <see cref="OilDigital.Workflows.ApprovalService"/> 
		/// </summary>
		/// <value></value>
		public ApprovalService this[string name]
		{
			get 
			{
				foreach (ApprovalService service in this)
				{
					if (service.ApplicationName.Equals(name, StringComparison.OrdinalIgnoreCase))
						return service;
				}
				return null;
			}
		}

		/// <summary>
		/// ���չ�������ʵ������������ڴ����ApprovalService
		/// </summary>
		/// <param name="instance">������ʵ������</param>
		/// <returns></returns>
		public ApprovalService GetByInstance(WorkflowInstance instance)
		{
			WorkflowApplication app = instance.Workflow.Application;
			foreach (ApprovalService service in this)
			{
				if (service.ApplicationName.Equals(app.Name, StringComparison.OrdinalIgnoreCase))
					return service;
			}
			return null;
		}


		#region ��ȡ���������б���
		/// <summary>
		/// ajax���õĻ�ȡ��ǰ�û��Ĵ��������б�
		/// </summary>
		/// <param name="userId">�û���Id</param>
		/// <returns>���������б����ɵ�HTML�ַ���</returns>
		public string GetTaskList(string userId)
		{
			StringBuilder builders = new StringBuilder();
			foreach (ApprovalService service in this)
			{
				builders.Append(service.CurrentlyService.Distill(userId));
			}
			return builders.ToString();
		}

		/// <summary>
		/// ��ȡ��ǰ��¼�û��Ŀɳ��������б�
		/// </summary>
		/// <returns></returns>
		public string GetAllowedCancelTaskList(string userId)
		{
			StringBuilder builders = new StringBuilder();
			foreach (ApprovalService service in this)
			{
				builders.Append(service.AllowedCancelService.Distill(userId));
			}
			return builders.ToString();
		}

		/// <summary>
		/// ��ȡ���������������б�
		/// </summary>
		/// <param name="startDate">����ʱ�䣨�ӣ�</param>
		/// <param name="endDate">����ʱ�䣨����</param>
		/// <returns></returns>
		public string GetProcessedTaskList(string userId, DateTime startDate, DateTime endDate)
		{
			StringBuilder builders = new StringBuilder();
			foreach (ApprovalService service in this)
			{
				builders.Append(service.ProceedService.Distill(userId, startDate, endDate));
			}
			return builders.ToString();
		}
		/// <summary>
		/// ��ȡ���������������б�
		/// </summary>
		/// <param name="startDate">����ʱ�䣨�ӣ�</param>
		/// <param name="endDate">����ʱ�䣨����</param>
		public string GetProcessingTaskList(string userId, DateTime startDate, DateTime endDate)
		{
			StringBuilder builders = new StringBuilder();
			foreach (ApprovalService service in this)
			{
				builders.Append(service.ProcessingService.Distill(userId, startDate, endDate));
			}
			return builders.ToString();
		}
		/// <summary>
		/// ��ȡ���������������б�
		/// </summary>
		/// <param name="startDate">����ʱ�䣨�ӣ�</param>
		/// <param name="endDate">����ʱ�䣨����</param>
		/// <returns></returns>
		public string GetCompletedTaskList(string userId, DateTime startDate, DateTime endDate)
		{
			StringBuilder builders = new StringBuilder();
			foreach (ApprovalService service in this)
			{
				builders.Append(service.CompletedService.Distill(userId, startDate, endDate));
			}
			return builders.ToString();
		}
		/// <summary>
		/// ��ȡ�Ѵ���������б�
		/// </summary>
		/// <param name="taskName">��������</param>
		/// <param name="agentId">��Ȩ�����¼��Id</param>
		/// <returns></returns>
		public string GetAgentProceedTask(string taskName, int agentId)
		{
			ApprovalAgent agentInfo = WorkflowRuntime.Current.SaveService.GetAgentInfoById(agentId);
			if(agentInfo==null) return string.Empty;
			StringBuilder builders = new StringBuilder();
			foreach (ApprovalService service in this)
			{
				builders.Append(service.AgentService.Distill(agentInfo.ToUserId, agentInfo.BeginDate, agentInfo.EndDate));
			}
			return builders.ToString();
		}
		#endregion
	}
	/// <summary>
	/// ����������ط������࣬�����ɻ�ȡ��ȡ��������Ŀ���Ѱ�����Ŀ�ȷ���
	/// </summary>
	public class ApprovalService
	{
		internal ApprovalService(ApplicationConfig applicationConfig)
		{
			this.applicationName = applicationConfig.Name;
			if (applicationConfig.CurrentlyService == null)
				throw new ConfigurationErrorsException("cann't found config section 'CurrentlyService'");
			if (applicationConfig.AllowedCancelService == null)
				throw new ConfigurationErrorsException("cann't found config section 'AllowedCancelService'");
			if (applicationConfig.ProceedService == null)
				throw new ConfigurationErrorsException("cann't found config section 'ProceedService'");
			if (applicationConfig.CompletedService == null)
				throw new ConfigurationErrorsException("cann't found config section 'CompletedService'");
			if (applicationConfig.ProcessingService == null)
				throw new ConfigurationErrorsException("cann't found config section 'ProcessingService'");
			if (applicationConfig.AgentService == null)
				throw new ConfigurationErrorsException("cann't found config section 'AgentService'");

			if (applicationConfig.ApprovalObjectService != null)
				this.approvalObjectService = (IApprovalObjectService)ConfigurationHelper.LoadType(applicationConfig.ApprovalObjectService);

			//�����������������漰�ķ���
			applicationConfig.CurrentlyService.Name = "currentlyService";
			applicationConfig.AllowedCancelService.Name = "allowedCancelService";
			applicationConfig.CompletedService.Name = "completedService";
			applicationConfig.ProceedService.Name = "proceedService";
			applicationConfig.ProcessingService.Name = "processingService";
			applicationConfig.AgentService.Name = "agentService";
			currentlyService = new TaskService(applicationConfig.CurrentlyService, applicationConfig.Name);
			allowedCancelService = new TaskService(applicationConfig.AllowedCancelService, applicationConfig.Name);
			proceedService = new TaskService(applicationConfig.ProceedService, applicationConfig.Name);
			completedService = new TaskService(applicationConfig.CompletedService, applicationConfig.Name);
			processingService = new TaskService(applicationConfig.ProcessingService, applicationConfig.Name);
			agentService = new TaskService(applicationConfig.AgentService, applicationConfig.Name);

			//���������еķ����б��еķ���
			if (applicationConfig.Services != null && applicationConfig.Services.Length != 0)
			{
				services.Clear();
				foreach (ServiceConfig o in applicationConfig.Services)
				{
					TaskService service = new TaskService(o, applicationConfig.Name);
					if (services.ContainsKey(o.Name))
						throw new ConfigurationErrorsException(string.Format("service \"{0}\" already added��",o.Name));
					services.Add(service.Name, service);
				}
			}
			//���������е������б��е�����
			if (applicationConfig.Types != null && applicationConfig.Types.Length != 0)
			{
				types.Clear();
				foreach (TypeItem item in applicationConfig.Types)
				{
					object o = ConfigurationHelper.LoadType(item);
					if (o.GetType().GetInterface(typeof(ICloneable).Name) == null)
						throw new TypeLoadException(string.Format("\"{0}\" doesn't implement Interface ICloneable!", o.GetType().FullName));
					AddType((ICloneable)o);
				}
			}
			agentConfig = applicationConfig.AgentConfigInfo;
			agentConfig.Check(applicationConfig.Name);

			//��ʼ��������¼��������.
			if (applicationConfig.RecordFilters != null && applicationConfig.RecordFilters.Length > 0)
			{
				this.recordsFilters = new List<IApprovalRecordFilter>();
				foreach (TypeItem one in applicationConfig.RecordFilters)
				{
					IApprovalRecordFilter obj = ConfigurationHelper.LoadType(one) as IApprovalRecordFilter;
					if (one == null) throw new ConfigurationErrorsException(string.Format("������¼�������ô��� \"{0}\" ",one.Name));
					this.recordsFilters.Add(obj);
				}
			}

			//��ʼ�����������������.
			if (applicationConfig.SolutionFilters != null && applicationConfig.SolutionFilters.Length > 0)
			{
				this.solutionFilters = new List<IApprovalSolutionFilter>();
				foreach (TypeItem one in applicationConfig.SolutionFilters)
				{
					IApprovalSolutionFilter obj = ConfigurationHelper.LoadType(one) as IApprovalSolutionFilter;
					if (one == null) throw new ConfigurationErrorsException(string.Format("������¼�������ô��� \"{0}\" ", one.Name));
					this.solutionFilters.Add(obj);
				}
			}
		}
		private string applicationName;
		/// <summary>
		/// Gets the name of the application.
		/// </summary>
		/// <value>The name of the application.</value>
		public string ApplicationName
		{
			get { return this.applicationName; }
		}
		
		
		private AgentConfig agentConfig;
		/// <summary>
		/// ��Ȩ���ñ�
		/// </summary>
		public AgentConfig AgentConfigInfo
		{
			get
			{
				return agentConfig;
			}
		}
		

		private IApprovalObjectService approvalObjectService;

		/// <summary>
		/// ��ȡ���ʱ���������ķ���.
		/// </summary>
		/// <value>The approval object service.</value>
		public IApprovalObjectService ApprovalObjectService
		{
			get
			{
				if (approvalObjectService == null)
					throw new ConfigurationErrorsException("cann't found config section 'approvalObjectService'");
				return approvalObjectService;
			}
		}


		private ITaskActionProcessor taskActionProcessor;
		/// <summary>
		/// ��������������
		/// </summary>
		public ITaskActionProcessor ActionProcessor
		{
			get
			{
				if (taskActionProcessor == null)
					throw new ConfigurationErrorsException("cann't found config section 'TaskActionProcessor'");
				return taskActionProcessor;
			}
		}
		private ITaskApprovalService taskApprovalService;
		/// <summary>
		/// ����������ط���
		/// </summary>
		public ITaskApprovalService TaskApprovalService
		{
			get
			{
				if (taskApprovalService == null)
					throw new ConfigurationErrorsException("cann't found config section 'TaskApprovalService'");
				return taskApprovalService;
			}
		}
		private TaskService currentlyService;
		/// <summary>
		/// �����������
		/// </summary>
		/// <value>The currently service.</value>
		public TaskService CurrentlyService
		{
			get
			{
				return (TaskService)currentlyService.Clone();
			}
		}
		private TaskService allowedCancelService;
		/// <summary>
		/// �Ѵ������������������
		/// </summary>
		/// <value>The allowed cancel service.</value>
		public TaskService AllowedCancelService
		{
			get
			{
				return (TaskService)allowedCancelService.Clone();
			}
		}
		private TaskService proceedService;
		/// <summary>
		/// �Ѵ�������������
		/// </summary>
		/// <value>The proceed service.</value>
		public TaskService ProceedService
		{
			get
			{
				return (TaskService)proceedService.Clone();
			}
		}
		private TaskService completedService;
		/// <summary>
		/// Gets the completed service.
		/// </summary>
		/// <value>The completed service.</value>
		public TaskService CompletedService
		{
			get
			{
				return (TaskService)completedService.Clone();
			}
		}
		private TaskService processingService;
		/// <summary>
		/// ���ڴ����е��������
		/// </summary>
		/// <value>The processing service.</value>
		public TaskService ProcessingService
		{
			get
			{
				return (TaskService)processingService.Clone();
			}
		}
		private TaskService agentService;
		/// <summary>
		/// ���������Ŀ����
		/// </summary>
		/// <value>The agent service.</value>
		public TaskService AgentService
		{
			get
			{
				return (TaskService)agentService.Clone();
			}
		}

		/// <summary>
		/// ��ȡ�������
		/// </summary>
		/// <param name="eaid">�����������Id</param>
		/// <returns></returns>
		public List<ApprovalSolution> GetApprovalSolutions(int eaid)
		{
			DrillApprovalSolution drill = new DrillApprovalSolution();

			string userId = WorkflowRuntime.Current.GetService<IIdentityService>().GetUserIdentity().GetUserId();
			string rootName = WorkflowRuntime.Current.DefineService.GetRoot(this.applicationName);
			List<ApprovalRole> roles = WorkflowUtility.GetUserRoles(rootName, userId);
			if (roles == null || roles.Count == 0)
				//û�н�ɫ�û�,���ܿ��κμ�¼
				return new List<ApprovalSolution>();

			if (this.solutionFilters != null && this.solutionFilters.Count >= 0)
			{
				ApprovalRole role = roles[0];
				foreach (IApprovalSolutionFilter one in this.solutionFilters)
				{
					one.SetValues(rootName, role, userId);
					drill.Filters.Add(one);
				}
			}
			return drill.GetInfo(this.applicationName, eaid);
		}

		/// <summary>
		/// ��ȡ������¼
		/// </summary>
		/// <param name="eaid">�����������Id</param>
		/// <returns></returns>
		public List<ApprovalRecord> GetApprovalRecords(int eaid)
		{
			DrillApprovalRecord drill = new DrillApprovalRecord();
			string userId = WorkflowRuntime.Current.GetService<IIdentityService>().GetUserIdentity().GetUserId();
			string rootName = WorkflowRuntime.Current.DefineService.GetRoot(this.applicationName);
			List<ApprovalRole> roles = WorkflowUtility.GetUserRoles(rootName, userId);

			if (roles == null || roles.Count == 0)
				//û�н�ɫ�û�,���ܿ��κμ�¼
				return new List<ApprovalRecord>();

			if (this.recordsFilters != null && this.recordsFilters.Count >= 0)
			{
				ApprovalRole role = roles[0];
				foreach (IApprovalRecordFilter one in this.recordsFilters)
				{
					one.SetValues(rootName, role, userId);
					drill.Filters.Add(one);
				}
			}
			return drill.GetInfo(this.applicationName, eaid);
		}

		/// <summary>
		/// ������¼������
		/// </summary>
		private List<IApprovalRecordFilter> recordsFilters;


		/// <summary>
		/// �������������
		/// </summary>
		private List<IApprovalSolutionFilter> solutionFilters;

		private Dictionary<string, TaskService> services = new Dictionary<string, TaskService>();
		/// <summary>
		/// ���ݷ������ƻ�ȡ����
		/// </summary>
		/// <param name="serviceName">Name of the service.</param>
		/// <returns></returns>
		public TaskService GetService(string serviceName)
		{
			if (!services.ContainsKey(serviceName))
				return null;
			return services[serviceName].Clone() as TaskService;
		}
		private Dictionary<Type, List<ICloneable>> types = new Dictionary<Type, List<ICloneable>>();
		/// <summary>
		/// ���ʵ��
		/// </summary>
		private void AddType(ICloneable obj)
		{
			if (obj == null)
				throw new ArgumentException("service");
			if (GetAllTypes(obj.GetType()).Count > 0)
				throw new InvalidOperationException(string.Format("Can't Add Service Twice!\"{0}\"", obj.GetType()));
			Type key = obj.GetType();
			foreach (Type type2 in key.GetInterfaces())
			{
				if (!types.ContainsKey(type2))
					types.Add(type2, new List<ICloneable>());
				types[type2].Add(obj);
			}
			while (key != null)
			{
				if (!types.ContainsKey(key))
					types.Add(key, new List<ICloneable>());
				types[key].Add(obj);
				key = key.BaseType;
			}
		}
		private ReadOnlyCollection<ICloneable> GetAllTypes(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			List<ICloneable> list = new List<ICloneable>();
			if (types.ContainsKey(type))
				list.AddRange(types[type]);
			return new ReadOnlyCollection<ICloneable>(list);
		}
		/// <summary>
		/// ��ȡ���ص�ʵ��
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetTypeInstance<T>()
		{
			return (T)GetTypeInstance(typeof(T), false);
		}

		/// <summary>
		/// ��ȡ���ص�ʵ�����Ե�����ʽ��ȡʵ����ִ��Clone����ֱ�ӷ������ж���
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="isSingleton">�Ƿ��Ե�����ʽ����</param>
		/// <returns></returns>
		public T GetTypeInstance<T>(bool isSingleton)
		{
			return (T)GetTypeInstance(typeof(T), isSingleton);
		}

		private object GetTypeInstance(Type type, bool isSingleton)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			ICloneable obj = null;
			if (types.ContainsKey(type))
			{
				if (types[type].Count > 1)
					throw new InvalidOperationException(string.Format("more than one {0} types", type));
				if (types[type].Count == 1)
					obj = types[type][0];
			}
			if (obj != null)
			{
				if (isSingleton)
					return obj;
				return obj.Clone();
			}
			return null;
		}
	}
}

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
	/// 工作流服务定义集合
	/// </summary>
	public sealed class ApprovalServicesCollection:List<ApprovalService>
	{
		/// <summary>
		/// 按应用程序的名称获取服务 <see cref="OilDigital.Workflows.ApprovalService"/> 
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
		/// 按照工作流的实例对象查找用于处理的ApprovalService
		/// </summary>
		/// <param name="instance">工作流实例对象</param>
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


		#region 获取各种任务列表方法
		/// <summary>
		/// ajax调用的获取当前用户的待办任务列表
		/// </summary>
		/// <param name="userId">用户的Id</param>
		/// <returns>返回任务列表生成的HTML字符串</returns>
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
		/// 获取当前登录用户的可撤销任务列表
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
		/// 获取已审批过的立项列表
		/// </summary>
		/// <param name="startDate">办理时间（从）</param>
		/// <param name="endDate">办理时间（到）</param>
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
		/// 获取正在审批中立项列表
		/// </summary>
		/// <param name="startDate">办理时间（从）</param>
		/// <param name="endDate">办理时间（到）</param>
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
		/// 获取审批结束的立项列表
		/// </summary>
		/// <param name="startDate">办理时间（从）</param>
		/// <param name="endDate">办理时间（到）</param>
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
		/// 获取已待办的任务列表
		/// </summary>
		/// <param name="taskName">任务名称</param>
		/// <param name="agentId">授权代理记录的Id</param>
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
	/// 所有审批相关服务总类，包括可获取获取需办理的项目，已办理项目等服务
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

			//加载六种审批任务涉及的服务
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

			//加载配置中的服务列表中的服务
			if (applicationConfig.Services != null && applicationConfig.Services.Length != 0)
			{
				services.Clear();
				foreach (ServiceConfig o in applicationConfig.Services)
				{
					TaskService service = new TaskService(o, applicationConfig.Name);
					if (services.ContainsKey(o.Name))
						throw new ConfigurationErrorsException(string.Format("service \"{0}\" already added！",o.Name));
					services.Add(service.Name, service);
				}
			}
			//加载配置中的类型列表中的类型
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

			//初始化审批记录过滤配置.
			if (applicationConfig.RecordFilters != null && applicationConfig.RecordFilters.Length > 0)
			{
				this.recordsFilters = new List<IApprovalRecordFilter>();
				foreach (TypeItem one in applicationConfig.RecordFilters)
				{
					IApprovalRecordFilter obj = ConfigurationHelper.LoadType(one) as IApprovalRecordFilter;
					if (one == null) throw new ConfigurationErrorsException(string.Format("审批记录过滤配置错误 \"{0}\" ",one.Name));
					this.recordsFilters.Add(obj);
				}
			}

			//初始化审批意见过滤配置.
			if (applicationConfig.SolutionFilters != null && applicationConfig.SolutionFilters.Length > 0)
			{
				this.solutionFilters = new List<IApprovalSolutionFilter>();
				foreach (TypeItem one in applicationConfig.SolutionFilters)
				{
					IApprovalSolutionFilter obj = ConfigurationHelper.LoadType(one) as IApprovalSolutionFilter;
					if (one == null) throw new ConfigurationErrorsException(string.Format("审批记录过滤配置错误 \"{0}\" ", one.Name));
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
		/// 授权配置表
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
		/// 获取访问被审批对象的服务.
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
		/// 处理审批动作类
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
		/// 其他审批相关服务
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
		/// 代办任务服务
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
		/// 已处理且允许撤销任务服务
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
		/// 已处理过的任务服务
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
		/// 正在处理中的任务服务
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
		/// 代理代办项目服务
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
		/// 获取审批意见
		/// </summary>
		/// <param name="eaid">被审批对象的Id</param>
		/// <returns></returns>
		public List<ApprovalSolution> GetApprovalSolutions(int eaid)
		{
			DrillApprovalSolution drill = new DrillApprovalSolution();

			string userId = WorkflowRuntime.Current.GetService<IIdentityService>().GetUserIdentity().GetUserId();
			string rootName = WorkflowRuntime.Current.DefineService.GetRoot(this.applicationName);
			List<ApprovalRole> roles = WorkflowUtility.GetUserRoles(rootName, userId);
			if (roles == null || roles.Count == 0)
				//没有角色用户,不能看任何记录
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
		/// 获取审批记录
		/// </summary>
		/// <param name="eaid">被审批对象的Id</param>
		/// <returns></returns>
		public List<ApprovalRecord> GetApprovalRecords(int eaid)
		{
			DrillApprovalRecord drill = new DrillApprovalRecord();
			string userId = WorkflowRuntime.Current.GetService<IIdentityService>().GetUserIdentity().GetUserId();
			string rootName = WorkflowRuntime.Current.DefineService.GetRoot(this.applicationName);
			List<ApprovalRole> roles = WorkflowUtility.GetUserRoles(rootName, userId);

			if (roles == null || roles.Count == 0)
				//没有角色用户,不能看任何记录
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
		/// 审批记录过滤器
		/// </summary>
		private List<IApprovalRecordFilter> recordsFilters;


		/// <summary>
		/// 审批意见过滤器
		/// </summary>
		private List<IApprovalSolutionFilter> solutionFilters;

		private Dictionary<string, TaskService> services = new Dictionary<string, TaskService>();
		/// <summary>
		/// 根据服务名称获取服务
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
		/// 添加实例
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
		/// 获取加载的实例
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetTypeInstance<T>()
		{
			return (T)GetTypeInstance(typeof(T), false);
		}

		/// <summary>
		/// 获取加载的实例（以单件方式获取实例不执行Clone方法直接返回已有对象）
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="isSingleton">是否以单件方式加载</param>
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

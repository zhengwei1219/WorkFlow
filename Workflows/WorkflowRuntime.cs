using System;
using System.Data;
using System.Configuration;
using System.Xml.Serialization;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Collections.ObjectModel;

namespace OilDigital.Workflows
{
	/// <summary>
	/// ���������л�������,Singletonģʽʵ�ֵĵ�һ����,�����ó����ʼ��ʱ��Ҫ������г�ʼ������
	/// </summary>
	public class WorkflowRuntime : IServiceProvider
	{
		private Dictionary<Type, List<object>> services = new Dictionary<Type, List<object>>();
		private static readonly object syncRoot = new object();
		private static WorkflowRuntime runtime = null;
		internal WorkflowRuntime() { }
		/// <summary>
		/// ��������ʱ
		/// </summary>
		public void StartRuntime()
		{
			if (!isStarted)
			{
				if (GetService(typeof(IWorkFlowDefinePersistService)) == null)
					throw new ApplicationException("no IWorkFlowDefinePersistService type loaded!");
				if (GetService(typeof(IWorkflowPersistService)) == null)
					throw new ApplicationException("no IWorkflowPersistService type loaded!");
				this.persistService = (IWorkflowPersistService)GetService(typeof(IWorkflowPersistService));
				if (GetService(typeof(IApprovalSaveService)) == null)
					throw new ApplicationException("no IApprovalSaveService type loaded!");
				this.saveService = (IApprovalSaveService)GetService(typeof(IApprovalSaveService));
				if (GetService(typeof(IIdentityService)) == null)
					throw new ApplicationException("no IIdentityService type loaded!");
				if (GetService(typeof(IWorkflowSecurityService)) == null)
					throw new ApplicationException("no IWorkflowSecurityService type loaded!");
				if (GetService(typeof(ITransactionService)) == null)
					throw new ApplicationException("no ITransactionService type loaded!");
				if (RuntimeStart != null)
					RuntimeStart(this, new EventArgs());
				isStarted = true;
			}
		}
		/// <summary>
		/// Stops the runtime.
		/// </summary>
		public void StopRuntime()
		{
			runtime = null;
		}
		private bool isStarted = false;
		/// <summary>
		/// ��ǰ������Runtime
		/// </summary>
		public static WorkflowRuntime Current
		{
			get
			{
				if (runtime == null)
				{
					lock (syncRoot)
					{
						if (runtime == null)
						{
							if (ConfigurationManager.GetSection("WorkflowRuntime") == null)
								runtime = new WorkflowRuntime();
							else
								runtime = (WorkflowRuntime)ConfigurationManager.GetSection("WorkflowRuntime");
						}
					}
				}
				return runtime;
			}
		}

		private IWorkflowPersistService persistService;
		/// <summary>
		/// Gets the persist service.
		/// </summary>
		/// <value>The persist service.</value>
		internal IWorkflowPersistService PersistService
		{
			get 
			{
				if (persistService == null)
					persistService = (IWorkflowPersistService)GetService(typeof(IWorkflowPersistService));
				return persistService; 
			}
		}
		private IApprovalSaveService saveService;
		/// <summary>
		/// Gets the save service.
		/// </summary>
		/// <value>The save service.</value>
		public IApprovalSaveService SaveService
		{
			get
			{
				if (saveService == null)
					saveService = (IApprovalSaveService)GetService(typeof(IApprovalSaveService));
				return saveService;
			}
		}

		private IWorkFlowDefinePersistService defineService;
		/// <summary>
		/// Gets the define service.
		/// </summary>
		/// <value>The define service.</value>
		public IWorkFlowDefinePersistService DefineService
		{
			get 
			{
				if (defineService == null)
					defineService = (IWorkFlowDefinePersistService)GetService(typeof(IWorkFlowDefinePersistService));
				return defineService;
			}
		}
		
		/// <summary>
		/// ��ӷ���
		/// </summary>
		/// <param name="service">The service.</param>
		public void AddService(object service)
		{
			if (service == null)
				throw new ArgumentException("service");
			if (this.GetAllServices(service.GetType()).Count > 0)
				throw new InvalidOperationException(string.Format("Can't Add Service Twice!\"{0}\"", service.GetType()));
			Type key = service.GetType();
			foreach (Type type2 in key.GetInterfaces())
			{
				if (!this.services.ContainsKey(type2))
					this.services.Add(type2, new List<object>());
				this.services[type2].Add(service);
			}
			while (key != null)
			{
				if (!this.services.ContainsKey(key))
					this.services.Add(key, new List<object>());
				this.services[key].Add(service);
				key = key.BaseType;
			}
		}

		private ReadOnlyCollection<object> GetAllServices(Type serviceType)
		{
			if (serviceType == null)
				throw new ArgumentNullException("serviceType");
			List<object> list = new List<object>();
			if (this.services.ContainsKey(serviceType))
				list.AddRange(this.services[serviceType]);
			return new ReadOnlyCollection<object>(list);
		}

		/// <summary>
		/// ��ȡ�ض����͵ķ������
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetService<T>()
		{
			return (T)GetService(typeof(T));
		}

		#region IServiceProvider Members

		/// <summary>
		/// Gets the service object of the specified type.
		/// </summary>
		/// <param name="serviceType">An object that specifies the type of service object to get.</param>
		/// <returns>
		/// A service object of type <paramref name="serviceType"/>.-or- null if there is no service object of type <paramref name="serviceType"/>.
		/// </returns>
		public object GetService(Type serviceType)
		{
			if (serviceType == null)
				throw new ArgumentNullException("serviceType");
			object obj = null;
			if (services.ContainsKey(serviceType))
			{
				if (services[serviceType].Count > 1)
					throw new InvalidOperationException(string.Format("more than one {0} services", serviceType));
				if (services[serviceType].Count == 1)
					obj = services[serviceType][0];
			}
			return obj;
		}

		#endregion

		#region ManapulateInstance

		/// <summary>
		/// Creates the instance.
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="eaId">The ea id.</param>
		/// <returns></returns>
		public WorkflowInstance CreateInstance(string workflowName, int eaId)
		{
			return new StateMachineWorkflowInstance(workflowName, eaId);
		}
		/// <summary>
		/// ����Id��ȡ������ʵ��
		/// </summary>
		/// <param name="id">ʵ����Id</param>
		/// <returns></returns>
		public WorkflowInstance GetInstance(Guid id)
		{
			return PersistService.GetWorkflowInstance(id, true);
		}

		/// <summary>
		/// ����Id��ȡ������ʵ��
		/// </summary>
		/// <param name="id">ʵ����Id</param>
		/// <param name="attachMore">if set to <c>true</c> [attach more].</param>
		/// <returns></returns>
		public WorkflowInstance GetInstance(Guid id, bool attachMore)
		{
			return GetInstance(id);
		}
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <param name="applicationName">Name of the application.</param>
		/// <param name="eaId">The ea id.</param>
		/// <param name="attachMore">if set to <c>true</c> [attach more].</param>
		/// <returns></returns>
		public List<StateMachineWorkflowInstance> GetInstance(string applicationName, int eaId, bool attachMore)
		{
			string[] names = GetService<IWorkFlowDefinePersistService>().GetAllWorkflowDefineName(applicationName);
			List<StateMachineWorkflowInstance> instances = new List<StateMachineWorkflowInstance>();
			foreach (string workflowName in names)
			{
				instances.AddRange(PersistService.GetWorkflowInstance(workflowName, eaId, attachMore));
			}
			return instances;
		}

		
		/// <summary>
		/// Gets the root.
		/// </summary>
		/// <param name="applicationName">Name of the application.</param>
		/// <param name="eaId">The ea id.</param>
		/// <returns></returns>
		public StateMachineWorkflowInstance GetRoot(string applicationName, int eaId)
		{
			return GetRoot(applicationName, eaId, true);
		}
		/// <summary>
		/// Gets the root.
		/// </summary>
		/// <param name="applicationName">Name of the application.</param>
		/// <param name="eaId">The ea id.</param>
		/// <param name="attachMore">�Ƿ���ع����������Ķ�����,Activity�б�</param>
		/// <returns></returns>
		public StateMachineWorkflowInstance GetRoot(string applicationName, int eaId,bool attachMore)
		{
			string[] names = DefineService.GetAllWorkflowDefineName(applicationName);
			List<StateMachineWorkflowInstance> instances = new List<StateMachineWorkflowInstance>();
			foreach (string name in names)
			{
				instances.AddRange(PersistService.GetWorkflowInstance(name, eaId, attachMore));
			}
			if (instances.Count == 0)
				throw new ArgumentException(string.Format("Cann't find the root instance! eaid=\"{0}\"", eaId));
			foreach (StateMachineWorkflowInstance instance in instances)
			{
				if (instance.ParentId == Guid.Empty)
					return instance;
			}
			return null;
		}
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="startDate">The start date.</param>
		/// <param name="endDate">The end date.</param>
		/// <param name="stateNames">The state names.</param>
		/// <param name="unitCode">The unit code.</param>
		/// <returns></returns>
		public List<StateMachineWorkflowInstance> GetInstance(string workflowName, DateTime startDate, DateTime endDate, string[] stateNames, string unitCode)
		{
			List<StateMachineWorkflowInstance> instances = PersistService.GetWorkflowInstance(workflowName, startDate, endDate, stateNames, unitCode);
			return instances;
		}
		/// <summary>
		/// ��ȡ��ǰ�û���������ĳ״̬�Ĺ�����ʵ��
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="stateNames">The state names.</param>
		/// <param name="userId">The user id.</param>
		/// <param name="unitCode">The unit code.</param>
		/// <returns></returns>
		public List<StateMachineWorkflowInstance> GetOwnerList(string workflowName, string[] stateNames, string userId, string unitCode)
		{
			return PersistService.GetWorkflowInstance(workflowName, stateNames, userId, unitCode);
		}
		/// <summary>
		/// ��ȡ��ǰ�û����ڵ�λ��ĳ״̬�Ĺ�����ʵ��
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="stateNames">״̬����</param>
		/// <param name="unitCode">The unit code.</param>
		/// <returns></returns>
		public List<StateMachineWorkflowInstance> GetUnitList(string workflowName, string[] stateNames, string unitCode)
		{
			return PersistService.GetWorkflowInstance(workflowName, stateNames, unitCode);
		}
		/// <summary>
		/// ��ȡĳ״̬�µĹ�����ʵ��
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="stateNames">״̬����</param>
		/// <returns></returns>
		public List<StateMachineWorkflowInstance> GetListByState(string workflowName, string[] stateNames)
		{
			return PersistService.GetWorkflowInstance(workflowName, stateNames, typeof(StateMachineWorkflowInstance));
		}
		/// <summary>
		/// Deletes the instance.
		/// </summary>
		/// <param name="instance">The instance.</param>
		public void DeleteInstance(WorkflowInstance instance)
		{
			PersistService.DeleteWorkflowInstance(instance);
		}

		/// <summary>
		/// Persists the specified instance.
		/// </summary>
		/// <param name="instance">The instance.</param>
		public void Persist(WorkflowInstance instance)
		{
			instance.PersistTime = DateTime.Now;
			persistService.SaveWorkflowInstance((StateMachineWorkflowInstance)instance);
		}

		/// <summary>
		/// ��ȡ���й�����������Ϣ.
		/// </summary>
		/// <value>The config.</value>
		public WorkflowConfig Config
		{
			get { return this.DefineService.GetConfig(); }
		}

		/// <summary>
		/// Executes the specified instance.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="activity">The activity.</param>
		public void Execute(WorkflowInstance instance, ActivityBase activity)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");
			if (activity == null)
				throw new ArgumentNullException("activity");
			if (string.IsNullOrEmpty(activity.UserId))
				activity.UserId = GetService<IIdentityService>().GetUserIdentity().GetUserId();
			IWorkflowSecurityService service = GetService<IWorkflowSecurityService>();
			ApprovalRole role = service.GetActionRole(instance, activity.UserId, activity.EventName);
			activity.UserApprovalRole = role.Name;
			if (service.CanDoAction(instance, activity.EventName))
			{
				ITransactionService transactionService = GetService<ITransactionService>();
				transactionService.BeginTransaction();
				try
				{
					instance.InternalExecute(activity);
					transactionService.CommitTransaction();
				}
				catch (Exception ep)
				{
					transactionService.RollbackTransaction();
					throw ep;
				}
			}
			else
				throw new WorkflowExecuteExeception(string.Format("����ִ�б��β���:���̵�ǰ״̬\"{0}\"���ܽ���\"{1}\"����,�������߱�ִ�иò�����Ȩ��.", ((StateMachineWorkflowInstance)instance).CurrentState.Description));
		}

		/// <summary>
		/// Undoes the specified instance.
		/// </summary>
		/// <param name="instance">The instance.</param>
		public void Undo(WorkflowInstance instance)
		{
			ITransactionService transactionService = GetService<ITransactionService>();
			IWorkflowSecurityService security = GetService<IWorkflowSecurityService>();
			if (security.CanCancel(instance))
			{
				transactionService.BeginTransaction();
				try
				{
					instance.InternalUndo();
				}
				catch (Exception ep)
				{
					transactionService.RollbackTransaction();
					throw ep;
				}
			}
			else
				throw new WorkflowExecuteExeception("��ǰ״̬�����������г������������㲻�Ǹ���Ŀ�ϴ�����������ִ���ߣ�û��Ȩ�޽��г���������ֻ��ִ���߱��˲���ִ�г���");
		}

		#endregion

		#region EventHandlers

		/// <summary>
		/// Occurs when [runtime start].
		/// </summary>
		public event EventHandler<EventArgs> RuntimeStart;
		/// <summary>
		/// ĳ��������ʵ������ʱ����.
		/// </summary>
		public event EventHandler<WorkflowEventArgs> WorkflowCompleted;
		/// <summary>
		/// ������ִ��Activity����ʱ����
		/// </summary>
		public event EventHandler<WorkflowEventArgs> WorkflowExecuted;
		/// <summary>
		/// ������������һ��ʱ����
		/// </summary>
		public event EventHandler<WorkflowEventArgs> WorkflowCanceled;
		/// <summary>
		/// ������������ֹʱ����
		/// </summary>
		public event EventHandler<WorkflowEventArgs> WorkflowTerminated;
		/// <summary>
		/// ĳ��������ʵ������ʱ����.
		/// </summary>
		/// <param name="args">The <see cref="OilDigital.Workflows.WorkflowEventArgs"/> instance containing the event data.</param>
		internal void OnWorkflowCompleted(WorkflowEventArgs args)
		{
			if (WorkflowCompleted != null)
				WorkflowCompleted(this, args);
		}
		/// <summary>
		/// ������ִ��Activity����ʱ����.
		/// </summary>
		internal void OnWorkflowExecuted(WorkflowEventArgs args)
		{
			if (WorkflowExecuted != null)
				WorkflowExecuted(this, args);
		}
		/// <summary>
		/// ������������һ��ʱ����
		/// </summary>
		internal void OnWorkflowCanceled(WorkflowEventArgs args)
		{
			if (WorkflowCanceled != null)
				WorkflowCanceled(this, args);
		}
		/// <summary>
		/// ��������ֹʱ����
		/// </summary>
		internal void OnWorkflowTerminated(WorkflowEventArgs args)
		{
			if (WorkflowTerminated != null)
				WorkflowTerminated(this, args);
		}

		#endregion
	}
}
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Collections.Specialized;
using System.Xml;
using System.Web.Security;
using OilDigital.Common;

namespace OilDigital.Workflows
{
	public class OnActivityExecutedArg : EventArgs
	{

		protected ApprovalActivity activity;

		/// <summary>
		/// 将要执行的动作
		/// </summary>
		public ApprovalActivity Activity
		{
			get { return activity; }
			set { activity = value; }
		}

		public OnActivityExecutedArg():this(null)
		{
		}

		public OnActivityExecutedArg(ApprovalActivity activity)
		{
			this.activity = activity;
		}
	}
	/// <summary>
	/// 动作执行前事件参数
	/// </summary>
	public class OnActivityExecutingArg : OnActivityExecutedArg
	{
		private bool cancel;

		/// <summary>
		/// 是否需要被撤销
		/// </summary>
		public bool Cancel
		{
			get { return cancel; }
			set { cancel = value; }
		}
	
		public OnActivityExecutingArg(bool cancel,ApprovalActivity activity)
		{
			this.cancel = cancel;
			this.activity = activity;
		}


		/// <summary>
		/// 构造一个动作执行前事件参数对象,cancel设置为false,activity=null
		/// </summary>
		public OnActivityExecutingArg()
			: this(false, null)
		{
		}

		/// <summary>
		/// 构造一个空Activity的事件参数
		/// </summary>
		/// <param name="cancel"></param>
		public OnActivityExecutingArg(bool cancel):this(cancel,null)
		{

		}
	}

	/// <summary>
	/// 状态机工作流实例类
	/// </summary>
	[Serializable]
	public class StateMachineWorkflowInstance : WorkflowInstance
	{
		/// <summary>
		/// 动作执行前事件处理
		/// </summary>
		public event EventHandler<OnActivityExecutingArg> OnActivityExecuting;

		/// <summary>
		/// 动作执行完毕以后的事件处理
		/// </summary>
		public event EventHandler<OnActivityExecutedArg> OnActivityExecuted;

		/// <summary>
		/// 无参构造函数
		/// </summary>
		public StateMachineWorkflowInstance()
		{
			this.executedActivities = new List<ApprovalActivity>();
			this.stateRecord = new List<ApprovalState>();
			this.properties = new NameValueCollection();
		}

		/// <summary>
		/// 根据工作流名称创建一个新的工作流实例,生成新实例以后,必须调用Start方法来启动工作流
		/// </summary>
		/// <param name="workflowName">状态机工作流定义</param>
		public StateMachineWorkflowInstance(string workflowName, int eaId)
			: this()
		{
			this.id = Guid.NewGuid();
			this.eaId = eaId;
			this.Workflow = WorkflowRuntime.Current.GetService<IWorkFlowDefinePersistService>().GetWorkflowDefine(workflowName);
		}

		/// <summary>
		/// 根据工作流名称创建一个新的工作流实例,生成新实例以后,同时调用Start方法来启动工作流
		/// </summary>
		/// <param name="workflowName">状态机工作流定义</param>
		/// <param name="eaid">项目Id号</param>
		public static StateMachineWorkflowInstance CreateNewInstanceAndStart(string workflowName, int eaid)
		{
			StateMachineWorkflowInstance instance = new StateMachineWorkflowInstance(workflowName, eaid);
			instance.Start();
			return instance;
		}

		private Guid id;

		/// <summary>
		/// 工作流Guid
		/// </summary>
		[XmlIgnore()]
		public override Guid Id
		{
			get { return id; }
			set { id = value; }
		}

		private int eaId;
		/// <summary>
		/// 工作流关联的对象的ID
		/// </summary>
		
		[XmlAttribute()]
		public override int EaId
		{
			get { return eaId; }
			set { eaId = value; }
		}

		private Guid parentId;
		/// <summary>
		/// 父工作流实例Id
		/// </summary>
		public override Guid ParentId
		{
			get { return parentId; }
			set { parentId = value; }
		}

		private List<Guid> childrenId;
		/// <summary>
		/// 子工作流实例Id集合
		/// </summary>
		public override List<Guid> ChildrenId
		{
			get { return childrenId; }
			set { childrenId = value; }
		}
	
		/// <summary>
		/// 当前工作流实例对应的工作流定义的名称
		/// </summary>
		[XmlIgnore()]
		public override string WorkflowName
		{
			get { return workflow.Name; }
		}

		private StateMachineWorkflow workflow;

		/// <summary>
		/// 当前工作流实例对应的工作流定义
		/// </summary>
		[XmlIgnore()]
		public override WorkFlowDefine Workflow
		{
			get { return workflow; }
			set
			{
				workflow = (StateMachineWorkflow)value;
			}
		}

		private NameValueCollection properties;

		/// <summary>
		/// 和工作流实例相关的其他属性集合
		/// </summary>
		public override NameValueCollection Properties
		{
			get { return properties; }
			set { properties = value; }
		}

		/// <summary>
		/// 工作流的当前执行状态
		/// </summary>
		public override WorkflowExecutionStatus CurrentStatus
		{
			get { return CurrentState.Name == this.workflow.EndState ? WorkflowExecutionStatus.Closed : WorkflowExecutionStatus.Executing; }
		}

		/// <summary>
		/// 当前状态名字
		/// </summary>
		public string StateName
		{
			get { return CurrentState.Name; }
		}

		private ApprovalState currentState;

		/// <summary>
		/// 当前状态
		/// </summary>
		[XmlIgnore()]
		public ApprovalState CurrentState
		{
			get { return currentState; }
			set
			{
				currentState = value;
			}
		}

		private List<ApprovalState> stateRecord;
		/// <summary>
		/// 工作流历史状态(即在当前状态之前的过往状态记录)
		/// </summary>
		[XmlArray("StateRecord")]
		[XmlArrayItem("ApprovalState")]
		public List<ApprovalState> StateRecord
		{
			get { return stateRecord; }
			set { stateRecord = value; }
		}


		private List<ApprovalActivity> executedActivities;

		/// <summary>
		/// 工作流截止到当前状态为止,所有执行过的动作列表
		/// </summary>
		[XmlIgnore()]
		public List<ApprovalActivity> ExecutedActivities
		{
			get { return executedActivities; }
			set { executedActivities = value; }
		}

		/// <summary>
		/// 最后一步执行的动作
		/// </summary>
		public ApprovalActivity LastActivity
		{
			get
			{
				if (executedActivities == null || executedActivities.Count == 0)
					return null;
				else
					return executedActivities[executedActivities.Count - 1];
			}
		}

		
		private DateTime persistTime;
		/// <summary>
		/// 最后一次持久化的时间
		/// </summary>
		[XmlAttribute()]
		public DateTime PersistTime
		{
			get { return persistTime; }
			set { persistTime = value; }
		}

		/// <summary>
		/// 初始化工作流实例
		/// </summary>
		public override void Init()
		{
			//this.Workflow = new XmlWorkFlowDefinePersistService("").GetWorkflowDefine(string.Empty);
			//this.CurrentState = workflow.GetStateByName(state);
		}
		/// <summary>
		/// 
		/// </summary>
		public override void Terminate()
		{
			if (childrenId != null && childrenId.Count > 0)
			{
				foreach (Guid oneChild in childrenId)
				{
					StateMachineWorkflowInstance childInstance = WorkflowRuntime.Current.GetService<IApprovalSaveService>().GetWorkflowInstance(oneChild);
					childInstance.Terminate();
					WorkflowRuntime.Current.GetService<IApprovalSaveService>().DeleteWorkflowInstance(childInstance);
				}
			}
		}

		/// <summary>
		/// 设置实例的当前状态
		/// </summary>
		/// <param name="stateName"></param>
		public void SetCurrentState(string stateName)
		{
			Check.Require(!string.IsNullOrEmpty(stateName), "stateName can not be null");
			this.CurrentState = this.workflow.GetStateByName(stateName);
			Check.Ensure(this.currentState != null, "current state can not be null");
		}
		/// <summary>
		/// 设置实例的历史记录状态
		/// </summary>
		/// <param name="stateNames"></param>
		public void SetStateRecord(string[] stateNames)
		{
			Check.Require(stateNames != null, "stateNames can not be null");
			for (int i = 0; i < stateNames.Length; i++)
			{
				stateRecord.Add(this.workflow.GetStateByName(stateNames[i]));
			}
		}
		/// <summary>
		/// 启动工作流
		/// </summary>
		public override void Start()
		{
			ApprovalState initState = workflow.GetStateByName(workflow.InitState);
			this.CurrentState = initState;
			this.persistTime = DateTime.Now;
			//RecordStart();
			WorkflowRuntime.Current.GetService<IApprovalSaveService>().InsertWorkflowInstance(this);
		}

		/// <summary>
		/// 当前的事务服务接口
		/// </summary>
		public ITransactionService TransactionService
		{
			get { return WorkflowRuntime.Current.GetService<ITransactionService>(); }
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="activity"></param>
		public override void Execute(ApprovalActivity activity)
		{
			Check.Require(activity != null, "activity can not be null");

			if (CanDoApprovalAction(activity))
			{
				TransactionService.BeginTransaction();
				try
				{
					//触发执行前时间
					OnActivityExecutingArg e = new OnActivityExecutingArg(false, activity);
					// Copy to a temporary variable to be thread-safe.
					EventHandler<OnActivityExecutingArg> temp = this.OnActivityExecuting;
					if (temp != null)	temp(this, e);

					//如果时间被撤销那么返回
					if (e.Cancel) return;

					//执行当前动作
					activity.Execute(this);

					//触发执行完毕时间
					EventHandler<OnActivityExecutedArg> temp2 = this.OnActivityExecuted;
					if(this.OnActivityExecuted!=null)
					{
						OnActivityExecutedArg ee=new OnActivityExecutedArg(activity);
						temp2(this, ee);
					}

					//记录操作记录
					RecordActivityExecute(activity);



					//如果工作流动作完成,那么移动到下一步状态
					if (activity.ExecutionStatus == ActivityExecutionStatus.Closed)
					{
						//添加当前动作到动作列表中
						executedActivities.Add(activity);

						if (activity is IBranchAction)
						{
							//分支动作,需要获取分支执行后选定的状态
							IBranchAction branch = (IBranchAction)activity;
							MoveOn(branch.GetSelectedState());
						}
						else
						{
							//顺序动作,从动作针对的事件名称中获取状态
							ApprovalEvent currentEvent = currentState.GetEventByName(activity.EventName);
							MoveOn(currentEvent.NextStateNames[0]);
						}
					}

					//保存
					Save();

					TransactionService.CommitTransaction();
				}
				catch (Exception ep)
				{
					TransactionService.RollbackTransaction();
					throw ep;
				}
			}
			
		}
		/// <summary>
		/// 
		/// </summary>
		public override void Undo()
		{
			if (CanDoCancelAction(LastActivity))
			{
				TransactionService.BeginTransaction();
				try
				{
					//撤销动作
					LastActivity.Undo(this);

					//生成操作日志
					RecordUndo(LastActivity);

					//返回到上一状态
					MoveBack();

					//在动作列表中删除被撤销的动作
					executedActivities.RemoveAt(executedActivities.Count - 1);
					executedActivities.TrimExcess();

					if (childrenId != null && childrenId.Count > 0)
					{
						foreach (Guid oneChild in childrenId)
						{
							StateMachineWorkflowInstance childInstance = WorkflowRuntime.Current.GetService<IApprovalSaveService>().GetWorkflowInstance(oneChild);
							childInstance.Terminate();
							WorkflowRuntime.Current.GetService<IApprovalSaveService>().DeleteWorkflowInstance(childInstance);
						}
						this.childrenId = null;
					}
					//保存实例
					Save();
				}
				catch (Exception ep)
				{
					TransactionService.RollbackTransaction();
					throw ep;
				}
			}
			else
			{
				throw new WorkflowExecuteExeception("你不是该项目上次审批动作的执行者，没有权限进行撤销操作！只有执行者本人才能执行撤销");
			}
		}

		/// <summary>
		/// 保存工作流实例
		/// </summary>
		private void Save()
		{
			this.persistTime = DateTime.Now;
			WorkflowRuntime.Current.GetService<IApprovalSaveService>().SaveWorkflowInstance(this);
		}

		/// <summary>
		/// 
		/// </summary>
		private void MoveOn(string stateName)
		{
			ApprovalState nextState = workflow.GetStateByName(stateName);
			Check.Ensure(nextState!=null,"工作流定义中没有找到名称: '"+stateName+"' 对应的状态");
			stateRecord.Add(this.currentState);
			this.currentState = nextState;
			if (this.CurrentStatus == WorkflowExecutionStatus.Closed && parentId != null && parentId != Guid.Empty)
			{
				StateMachineWorkflowInstance parentInstance = WorkflowRuntime.Current.GetService<IApprovalSaveService>().GetWorkflowInstance(parentId);
				parentInstance.OnChildFinished(this);
			}
		}

		private void OnChildFinished(StateMachineWorkflowInstance instance)
		{
			bool isAllFinished = true;
			if (childrenId != null)
			{
				foreach (Guid oneChild in childrenId)
				{
					StateMachineWorkflowInstance childInstance = WorkflowRuntime.Current.GetService<IApprovalSaveService>().GetWorkflowInstance(oneChild);
					if (childInstance.id == instance.id)
						continue;
					if (childInstance.CurrentStatus != WorkflowExecutionStatus.Closed)
					{
						isAllFinished = false;
					}
				}
				if (isAllFinished)
				{
					//顺序动作,从动作针对的事件名称中获取状态
					MoveOn(CurrentState.Events[0].NextStateNames[0]);
					childrenId = null;
					Save();
				}
			}
		}

		/// <summary>
		/// 撤销操作
		/// </summary>
		private void MoveBack()
		{
			ApprovalState lastState = stateRecord[stateRecord.Count - 1];
			this.CurrentState = lastState;
			this.stateRecord.RemoveAt(stateRecord.Count - 1);
		}
		/// <summary>
		/// 记录动作的执行情况
		/// </summary>
		private void RecordActivityExecute(ApprovalActivity executeActivity)
		{
			ApprovalRecord record = new ApprovalRecord();
			IUserIdentity userInfo = GetUserIdentity();
			record.OperatorTime = DateTime.Now;
			record.WorkflowInstanceId = this.id;
			record.OperatorId = userInfo.GetUserId();
			record.OperatorName = userInfo.GetUserName();
			record.OperatorRole = executeActivity.UserApprovalRole;
			record.EaId = this.eaId;
			record.ApprovalType = currentState.GetEventByName(executeActivity.EventName).Description;
			record.StateName = currentState.Description;
			WorkflowRuntime.Current.GetService<IApprovalSaveService>().InsertApprovalRecord(record);
		}

		/// <summary>
		/// 记录实例的撤销情况
		/// </summary>
		private void RecordUndo(ApprovalActivity lastActivity)
		{
			ApprovalRecord record = new ApprovalRecord();
			IUserIdentity userInfo = GetUserIdentity();
			record.OperatorTime = DateTime.Now;
			record.WorkflowInstanceId = this.id;
			record.OperatorId = userInfo.GetUserId();
			record.OperatorName = userInfo.GetUserName();
			record.OperatorRole = lastActivity.UserApprovalRole;
			record.EaId = this.eaId;
			record.ApprovalType = "撤销操作";
			record.StateName = currentState.Description;	
			WorkflowRuntime.Current.GetService<IApprovalSaveService>().InsertApprovalRecord(record);
		}

		private void RecordStart()
		{
			ApprovalRecord record = new ApprovalRecord();
			IUserIdentity userInfo = GetUserIdentity();
			record.OperatorTime = DateTime.Now;
			record.WorkflowInstanceId = this.id;
			record.OperatorId = userInfo.GetUserId();
			record.OperatorName = userInfo.GetUserName();
			record.EaId = this.eaId;
			record.ApprovalType = "新建立项";
			record.StateName = currentState.Description;
			WorkflowRuntime.Current.GetService<IApprovalSaveService>().InsertApprovalRecord(record);
		}

		/// <summary>
		/// 获取用户身份
		/// </summary>
		/// <returns></returns>
		private IUserIdentity GetUserIdentity()
		{
			IIdentityService service = WorkflowRuntime.Current.GetService<IIdentityService>();
			if (service == null) throw new WorkflowExecuteExeception("身份信息提供服务为空");
			return service.GetUserIdentity();
		}
		/// <summary>
		/// 是否可以执行当前工作流步骤
		/// </summary>
		/// <param name="executeActivity">当前执行的活动</param>
		/// <returns></returns>
		private bool CanDoApprovalAction(ApprovalActivity executeActivity)
		{
			string eventName = executeActivity.EventName;
			IApprovalSaveService approvalService=WorkflowRuntime.Current.GetService<IApprovalSaveService>();
			if (currentState.GetEventByName(eventName) != null)
			{
				ApprovalEvent approvalEvent = currentState.GetEventByName(eventName);
				IUserIdentity identity = GetUserIdentity();
				foreach (EventRole eventRole in approvalEvent.Roles)
				{
					//如果当前操作用户在事件所允许操作角色列表中，返回true
					if (Roles.IsUserInRole(identity.GetUserId(), eventRole.Name))
					{
						return true;
					}
					else
					{
						//如果当前用户的被授权列表中有用户Id满足
						List<ApprovalAgent> agentList = approvalService.GetValidAgentInfoByToUser(identity.GetUserId());
						if (agentList != null && agentList.Count > 0)
						{
							foreach (ApprovalAgent agentInfo in agentList)
							{
								if (Roles.IsUserInRole(agentInfo.SetUserId, eventRole.Name))
								{
									return true;
								}
							}
						}
					}
				}
			}
			return false;
		}
		/// <summary>
		/// 校验是否可进行撤销操作
		/// </summary>
		/// <returns></returns>
		private bool CanDoCancelAction(ApprovalActivity lastActivity)
		{
			if (LastActivity == null)
				throw new WorkflowExecuteExeception("没有步骤可供撤销");
			else
			{
				//只有活动执行者有撤销权限
				IUserIdentity useIdentity=GetUserIdentity();
				if (lastActivity.UserId == useIdentity.GetUserId())
				{
					return true;
				}
			}
			return false;
		}
	}
}

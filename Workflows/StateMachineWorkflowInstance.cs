using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Collections.Specialized;
using System.Xml;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 状态机工作流实例类
	/// </summary>
	[Serializable]
	public class StateMachineWorkflowInstance : WorkflowInstance
	{
		/// <summary>
		/// 无参构造函数
		/// </summary>
		public StateMachineWorkflowInstance()
		{
			this.executedActivities = new List<Activity>();
			this.properties = new NameValueCollection();
			this.childrenId = new List<Guid>();
		}

		/// <summary>
		/// 根据工作流名称创建一个新的工作流实例,生成新实例以后,必须调用Start方法来启动工作流
		/// </summary>
		/// <param name="workflowName">状态机工作流定义</param>
		/// <param name="eaId">The ea id.</param>
		internal StateMachineWorkflowInstance(string workflowName, int eaId)
			: this()
		{
			this.id = Guid.NewGuid();
			this.eaId = eaId;
			this.workflowName = workflowName;
			this.workflow = (StateMachineWorkflow)WorkflowRuntime.Current.GetService<IWorkFlowDefinePersistService>().GetWorkflowDefine(workflowName);
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

		private ICanBeApproval externalObj;
		/// <summary>
		/// Gets or sets the external obj.
		/// </summary>
		/// <value>The external obj.</value>
		[XmlIgnore()]
		public override ICanBeApproval ExternalObj
		{
			get { return externalObj; }
			set { externalObj = value; }
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
		private string workflowName;
		/// <summary>
		/// Gets the name of the workflow.
		/// </summary>
		/// <value>The name of the workflow.</value>
		[XmlIgnore()]
		public override string WorkflowName
		{
			get { return workflowName; }
			set { this.workflowName = value; }
		}

		private StateMachineWorkflow workflow;

		/// <summary>
		/// 当前工作流实例对应的工作流定义
		/// </summary>
		[XmlIgnore()]
		public override WorkFlowDefine Workflow
		{
			get 
			{
				if (workflow == null)
					workflow = (StateMachineWorkflow)WorkflowRuntime.Current.GetService<IWorkFlowDefinePersistService>().GetWorkflowDefine(workflowName);
				return workflow; 
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
			get 
			{
				StateMachineWorkflow o = (StateMachineWorkflow)this.Workflow;
				if (stateName == o.EndState)
					return WorkflowExecutionStatus.Closed;
				else if (stateName == o.TerminateState)
					return WorkflowExecutionStatus.Aborted;
				else
					return WorkflowExecutionStatus.Executing;
			}
		}

		private string stateName;
		/// <summary>
		/// 当前状态名字
		/// </summary>
		public string StateName
		{
			get { return this.stateName; }
			set { this.stateName = value; }
		}

		/// <summary>
		/// 当前状态
		/// </summary>
		[XmlIgnore()]
		public ApprovalState CurrentState
		{
			get
			{
				return ((StateMachineWorkflow)Workflow).GetStateByName(stateName);
			}
		}


		private List<string> stateRecordNames = new List<string>();
		/// <summary>
		/// Gets or sets the state record names.
		/// </summary>
		/// <value>The state record names.</value>
		public List<string> StateRecordNames
		{
			get { return this.stateRecordNames; }
			set { this.stateRecordNames = value; }
		}

		private List<Activity> executedActivities;
		/// <summary>
		/// 工作流截止到当前状态为止,所有执行过的动作列表
		/// </summary>
		[XmlIgnore()]
		public List<Activity> ExecutedActivities
		{
			get { return executedActivities; }
			set { executedActivities = value; }
		}

		/// <summary>
		/// 最后一步执行的动作
		/// </summary>
		public Activity LastActivity
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
		public override DateTime PersistTime
		{
			get { return persistTime; }
			set { persistTime = value; }
		}

		/// <summary>
		/// Determines whether this instance is terminated.
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if this instance is terminated; otherwise, <c>false</c>.
		/// </returns>
		public bool IsTerminated()
		{
			return this.CurrentState.Name == ((StateMachineWorkflow)this.Workflow).TerminateState;
		}
		/// <summary>
		/// Determines whether this instance is end.
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if this instance is end; otherwise, <c>false</c>.
		/// </returns>
		public bool IsEnd()
		{
			return this.CurrentStatus == WorkflowExecutionStatus.Closed;
		}
		/// <summary>
		/// 启动工作流
		/// </summary>
		public override void Start()
		{
			StateMachineWorkflow o = (StateMachineWorkflow)this.Workflow;
			ApprovalState initState = o.GetStateByName(o.InitState);
			this.stateName = initState.Name;
			this.persistTime = DateTime.Now;
			WorkflowRuntime.Current.GetService<IWorkflowPersistService>().InsertWorkflowInstance(this);
		}

		/// <summary>
		/// 当前的事务服务接口
		/// </summary>
		public ITransactionService TransactionService
		{
			get { return WorkflowRuntime.Current.GetService<ITransactionService>(); }
		}
		/// <summary>
		/// Executes the specified activity.
		/// </summary>
		/// <param name="activity">The activity.</param>
		protected override void ExecuteActivity(Activity activity)
		{
			activity.OnInitialize(this);
			//执行当前动作
			activity.OnExecute(this);
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
					ApprovalEvent currentEvent = CurrentState.GetEventByName(activity.EventName);
					MoveOn(currentEvent.NextStateNames[0]);
				}
			}
			//保存
			Save();
			if (this.stateRecordNames.Count > 0)
				WorkflowRuntime.Current.OnWorkflowExecuted(new StateChangedEventArgs(this, this.stateRecordNames[stateRecordNames.Count - 1], stateName));
			//如果是子流程的执行完成,那么还有判断其父流程是否也已经执行完毕,如果执行完毕,那么设置其父流程的下一步状态
			if (this.CurrentStatus == WorkflowExecutionStatus.Closed && parentId != null && parentId != Guid.Empty)
			{
				StateMachineWorkflowInstance parentInstance = (StateMachineWorkflowInstance)WorkflowRuntime.Current.GetInstance(parentId);
				parentInstance.OnChildFinished(this);
			}
			else if (this.CurrentStatus == WorkflowExecutionStatus.Aborted)
			{
				WorkflowRuntime.Current.OnWorkflowTerminated(new StateChangedEventArgs(this, this.stateRecordNames[stateRecordNames.Count - 1], stateName));
			}
			if (IsEnd())
				WorkflowRuntime.Current.OnWorkflowCompleted(new StateChangedEventArgs(this, this.stateRecordNames[stateRecordNames.Count - 1], stateName));
		}
		/// <summary>
		/// Undoes this instance.
		/// </summary>
		protected override void Undo()
		{
			if (LastActivity == null)
				throw new WorkflowExecuteExeception("没有步骤可供撤销");
			//撤销动作
			LastActivity.OnUndo(this);
			if (this.stateRecordNames.Count > 0)
				WorkflowRuntime.Current.OnWorkflowCanceled(new StateChangedEventArgs(this, this.stateName, this.stateRecordNames[stateRecordNames.Count - 1]));
			//返回到上一状态
			MoveBack();
			//在动作列表中删除被撤销的动作
			executedActivities.RemoveAt(executedActivities.Count - 1);
			executedActivities.TrimExcess();

			RemoveChildren();
			//保存实例
			Save();
		}

		/// <summary>
		/// Removes the children.
		/// </summary>
		public void RemoveChildren()
		{
			if (childrenId != null && childrenId.Count > 0)
			{
				foreach (Guid oneChild in childrenId)
				{
					StateMachineWorkflowInstance childInstance = WorkflowRuntime.Current.GetService<IWorkflowPersistService>().GetWorkflowInstance(oneChild);
					childInstance.RemoveChildren();
					WorkflowRuntime.Current.GetService<IWorkflowPersistService>().DeleteWorkflowInstance(childInstance);
				}
				this.childrenId = null;
			}
		}

		/// <summary>
		/// 保存工作流实例
		/// </summary>
		public void Save()
		{
			this.persistTime = DateTime.Now;
			WorkflowRuntime.Current.GetService<IWorkflowPersistService>().SaveWorkflowInstance(this);
		}

		/// <summary>
		/// Moves the on.
		/// </summary>
		/// <param name="stateName">Name of the state.</param>
		public void MoveOn(string stateName)
		{
			ApprovalState nextState = ((StateMachineWorkflow)Workflow).GetStateByName(stateName);
			this.stateRecordNames.Add(this.stateName);
			this.stateName = nextState.Name;
		}

		private void OnChildFinished(StateMachineWorkflowInstance instance)
		{
			bool isAllFinished = true;
			if (childrenId != null)
			{
				foreach (Guid oneChild in childrenId)
				{
					StateMachineWorkflowInstance childInstance = WorkflowRuntime.Current.GetService<IWorkflowPersistService>().GetWorkflowInstance(oneChild);
					if (childInstance.id == instance.id)
						continue;
					if (childInstance.CurrentStatus != WorkflowExecutionStatus.Closed)
					{
						isAllFinished = false;
					}
				}
				if (isAllFinished)
				{
					if (CurrentState is InvokeWorkflowState)
					{
						MoveOn(((InvokeWorkflowState)CurrentState).CompletedTransitionState);
					}
					else
					{
						//顺序动作,从动作针对的事件名称中获取状态
						MoveOn(CurrentState.Events[0].NextStateNames[0]);
					}
					childrenId = null;
					Save();
				}
			}
		}

		/// <summary>
		/// 撤销操作
		/// </summary>
		public void MoveBack()
		{
			string lastState = stateRecordNames[stateRecordNames.Count - 1];
			this.stateName = lastState;
			this.stateRecordNames.RemoveAt(stateRecordNames.Count - 1);
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
		/// <returns>
		/// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		/// <exception cref="T:System.NullReferenceException">
		/// The <paramref name="obj"/> parameter is null.
		/// </exception>
		public override bool Equals(object obj)
		{
			if (obj is StateMachineWorkflowInstance)
			{
				StateMachineWorkflowInstance o = (StateMachineWorkflowInstance)obj;
				if (o.id == this.id)
					return true;
				return false;
			}
			return false;
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode()
		{
			return id.GetHashCode();
		}
	}
}

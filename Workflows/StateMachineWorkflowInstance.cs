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
	/// ״̬��������ʵ����
	/// </summary>
	[Serializable]
	public class StateMachineWorkflowInstance : WorkflowInstance
	{
		/// <summary>
		/// �޲ι��캯��
		/// </summary>
		public StateMachineWorkflowInstance()
		{
			this.executedActivities = new List<Activity>();
			this.properties = new NameValueCollection();
			this.childrenId = new List<Guid>();
		}

		/// <summary>
		/// ���ݹ��������ƴ���һ���µĹ�����ʵ��,������ʵ���Ժ�,�������Start����������������
		/// </summary>
		/// <param name="workflowName">״̬������������</param>
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
		/// ������Guid
		/// </summary>
		[XmlIgnore()]
		public override Guid Id
		{
			get { return id; }
			set { id = value; }
		}

		private int eaId;
		/// <summary>
		/// �����������Ķ����ID
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
		/// ��������ʵ��Id
		/// </summary>
		public override Guid ParentId
		{
			get { return parentId; }
			set { parentId = value; }
		}

		private List<Guid> childrenId;
		/// <summary>
		/// �ӹ�����ʵ��Id����
		/// </summary>
		public override List<Guid> ChildrenId
		{
			get { return childrenId; }
			set { childrenId = value; }
		}

		/// <summary>
		/// ��ǰ������ʵ����Ӧ�Ĺ��������������
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
		/// ��ǰ������ʵ����Ӧ�Ĺ���������
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
		/// �͹�����ʵ����ص��������Լ���
		/// </summary>
		public override NameValueCollection Properties
		{
			get { return properties; }
			set { properties = value; }
		}

		/// <summary>
		/// �������ĵ�ǰִ��״̬
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
		/// ��ǰ״̬����
		/// </summary>
		public string StateName
		{
			get { return this.stateName; }
			set { this.stateName = value; }
		}

		/// <summary>
		/// ��ǰ״̬
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
		/// ��������ֹ����ǰ״̬Ϊֹ,����ִ�й��Ķ����б�
		/// </summary>
		[XmlIgnore()]
		public List<Activity> ExecutedActivities
		{
			get { return executedActivities; }
			set { executedActivities = value; }
		}

		/// <summary>
		/// ���һ��ִ�еĶ���
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
		/// ���һ�γ־û���ʱ��
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
		/// ����������
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
		/// ��ǰ���������ӿ�
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
			//ִ�е�ǰ����
			activity.OnExecute(this);
			//����������������,��ô�ƶ�����һ��״̬
			if (activity.ExecutionStatus == ActivityExecutionStatus.Closed)
			{
				//��ӵ�ǰ�����������б���
				executedActivities.Add(activity);
				if (activity is IBranchAction)
				{
					//��֧����,��Ҫ��ȡ��ִ֧�к�ѡ����״̬
					IBranchAction branch = (IBranchAction)activity;
					MoveOn(branch.GetSelectedState());
				}
				else
				{
					//˳����,�Ӷ�����Ե��¼������л�ȡ״̬
					ApprovalEvent currentEvent = CurrentState.GetEventByName(activity.EventName);
					MoveOn(currentEvent.NextStateNames[0]);
				}
			}
			//����
			Save();
			if (this.stateRecordNames.Count > 0)
				WorkflowRuntime.Current.OnWorkflowExecuted(new StateChangedEventArgs(this, this.stateRecordNames[stateRecordNames.Count - 1], stateName));
			//����������̵�ִ�����,��ô�����ж��丸�����Ƿ�Ҳ�Ѿ�ִ�����,���ִ�����,��ô�����丸���̵���һ��״̬
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
				throw new WorkflowExecuteExeception("û�в���ɹ�����");
			//��������
			LastActivity.OnUndo(this);
			if (this.stateRecordNames.Count > 0)
				WorkflowRuntime.Current.OnWorkflowCanceled(new StateChangedEventArgs(this, this.stateName, this.stateRecordNames[stateRecordNames.Count - 1]));
			//���ص���һ״̬
			MoveBack();
			//�ڶ����б���ɾ���������Ķ���
			executedActivities.RemoveAt(executedActivities.Count - 1);
			executedActivities.TrimExcess();

			RemoveChildren();
			//����ʵ��
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
		/// ���湤����ʵ��
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
						//˳����,�Ӷ�����Ե��¼������л�ȡ״̬
						MoveOn(CurrentState.Events[0].NextStateNames[0]);
					}
					childrenId = null;
					Save();
				}
			}
		}

		/// <summary>
		/// ��������
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

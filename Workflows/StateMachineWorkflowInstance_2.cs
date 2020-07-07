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
		/// ��Ҫִ�еĶ���
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
	/// ����ִ��ǰ�¼�����
	/// </summary>
	public class OnActivityExecutingArg : OnActivityExecutedArg
	{
		private bool cancel;

		/// <summary>
		/// �Ƿ���Ҫ������
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
		/// ����һ������ִ��ǰ�¼���������,cancel����Ϊfalse,activity=null
		/// </summary>
		public OnActivityExecutingArg()
			: this(false, null)
		{
		}

		/// <summary>
		/// ����һ����Activity���¼�����
		/// </summary>
		/// <param name="cancel"></param>
		public OnActivityExecutingArg(bool cancel):this(cancel,null)
		{

		}
	}

	/// <summary>
	/// ״̬��������ʵ����
	/// </summary>
	[Serializable]
	public class StateMachineWorkflowInstance : WorkflowInstance
	{
		/// <summary>
		/// ����ִ��ǰ�¼�����
		/// </summary>
		public event EventHandler<OnActivityExecutingArg> OnActivityExecuting;

		/// <summary>
		/// ����ִ������Ժ���¼�����
		/// </summary>
		public event EventHandler<OnActivityExecutedArg> OnActivityExecuted;

		/// <summary>
		/// �޲ι��캯��
		/// </summary>
		public StateMachineWorkflowInstance()
		{
			this.executedActivities = new List<ApprovalActivity>();
			this.stateRecord = new List<ApprovalState>();
			this.properties = new NameValueCollection();
		}

		/// <summary>
		/// ���ݹ��������ƴ���һ���µĹ�����ʵ��,������ʵ���Ժ�,�������Start����������������
		/// </summary>
		/// <param name="workflowName">״̬������������</param>
		public StateMachineWorkflowInstance(string workflowName, int eaId)
			: this()
		{
			this.id = Guid.NewGuid();
			this.eaId = eaId;
			this.Workflow = WorkflowRuntime.Current.GetService<IWorkFlowDefinePersistService>().GetWorkflowDefine(workflowName);
		}

		/// <summary>
		/// ���ݹ��������ƴ���һ���µĹ�����ʵ��,������ʵ���Ժ�,ͬʱ����Start����������������
		/// </summary>
		/// <param name="workflowName">״̬������������</param>
		/// <param name="eaid">��ĿId��</param>
		public static StateMachineWorkflowInstance CreateNewInstanceAndStart(string workflowName, int eaid)
		{
			StateMachineWorkflowInstance instance = new StateMachineWorkflowInstance(workflowName, eaid);
			instance.Start();
			return instance;
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
		[XmlIgnore()]
		public override string WorkflowName
		{
			get { return workflow.Name; }
		}

		private StateMachineWorkflow workflow;

		/// <summary>
		/// ��ǰ������ʵ����Ӧ�Ĺ���������
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
			get { return CurrentState.Name == this.workflow.EndState ? WorkflowExecutionStatus.Closed : WorkflowExecutionStatus.Executing; }
		}

		/// <summary>
		/// ��ǰ״̬����
		/// </summary>
		public string StateName
		{
			get { return CurrentState.Name; }
		}

		private ApprovalState currentState;

		/// <summary>
		/// ��ǰ״̬
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
		/// ��������ʷ״̬(���ڵ�ǰ״̬֮ǰ�Ĺ���״̬��¼)
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
		/// ��������ֹ����ǰ״̬Ϊֹ,����ִ�й��Ķ����б�
		/// </summary>
		[XmlIgnore()]
		public List<ApprovalActivity> ExecutedActivities
		{
			get { return executedActivities; }
			set { executedActivities = value; }
		}

		/// <summary>
		/// ���һ��ִ�еĶ���
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
		/// ���һ�γ־û���ʱ��
		/// </summary>
		[XmlAttribute()]
		public DateTime PersistTime
		{
			get { return persistTime; }
			set { persistTime = value; }
		}

		/// <summary>
		/// ��ʼ��������ʵ��
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
		/// ����ʵ���ĵ�ǰ״̬
		/// </summary>
		/// <param name="stateName"></param>
		public void SetCurrentState(string stateName)
		{
			Check.Require(!string.IsNullOrEmpty(stateName), "stateName can not be null");
			this.CurrentState = this.workflow.GetStateByName(stateName);
			Check.Ensure(this.currentState != null, "current state can not be null");
		}
		/// <summary>
		/// ����ʵ������ʷ��¼״̬
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
		/// ����������
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
		/// ��ǰ���������ӿ�
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
					//����ִ��ǰʱ��
					OnActivityExecutingArg e = new OnActivityExecutingArg(false, activity);
					// Copy to a temporary variable to be thread-safe.
					EventHandler<OnActivityExecutingArg> temp = this.OnActivityExecuting;
					if (temp != null)	temp(this, e);

					//���ʱ�䱻������ô����
					if (e.Cancel) return;

					//ִ�е�ǰ����
					activity.Execute(this);

					//����ִ�����ʱ��
					EventHandler<OnActivityExecutedArg> temp2 = this.OnActivityExecuted;
					if(this.OnActivityExecuted!=null)
					{
						OnActivityExecutedArg ee=new OnActivityExecutedArg(activity);
						temp2(this, ee);
					}

					//��¼������¼
					RecordActivityExecute(activity);



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
							ApprovalEvent currentEvent = currentState.GetEventByName(activity.EventName);
							MoveOn(currentEvent.NextStateNames[0]);
						}
					}

					//����
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
					//��������
					LastActivity.Undo(this);

					//���ɲ�����־
					RecordUndo(LastActivity);

					//���ص���һ״̬
					MoveBack();

					//�ڶ����б���ɾ���������Ķ���
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
					//����ʵ��
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
				throw new WorkflowExecuteExeception("�㲻�Ǹ���Ŀ�ϴ�����������ִ���ߣ�û��Ȩ�޽��г���������ֻ��ִ���߱��˲���ִ�г���");
			}
		}

		/// <summary>
		/// ���湤����ʵ��
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
			Check.Ensure(nextState!=null,"������������û���ҵ�����: '"+stateName+"' ��Ӧ��״̬");
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
					//˳����,�Ӷ�����Ե��¼������л�ȡ״̬
					MoveOn(CurrentState.Events[0].NextStateNames[0]);
					childrenId = null;
					Save();
				}
			}
		}

		/// <summary>
		/// ��������
		/// </summary>
		private void MoveBack()
		{
			ApprovalState lastState = stateRecord[stateRecord.Count - 1];
			this.CurrentState = lastState;
			this.stateRecord.RemoveAt(stateRecord.Count - 1);
		}
		/// <summary>
		/// ��¼������ִ�����
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
		/// ��¼ʵ���ĳ������
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
			record.ApprovalType = "��������";
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
			record.ApprovalType = "�½�����";
			record.StateName = currentState.Description;
			WorkflowRuntime.Current.GetService<IApprovalSaveService>().InsertApprovalRecord(record);
		}

		/// <summary>
		/// ��ȡ�û����
		/// </summary>
		/// <returns></returns>
		private IUserIdentity GetUserIdentity()
		{
			IIdentityService service = WorkflowRuntime.Current.GetService<IIdentityService>();
			if (service == null) throw new WorkflowExecuteExeception("�����Ϣ�ṩ����Ϊ��");
			return service.GetUserIdentity();
		}
		/// <summary>
		/// �Ƿ����ִ�е�ǰ����������
		/// </summary>
		/// <param name="executeActivity">��ǰִ�еĻ</param>
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
					//�����ǰ�����û����¼������������ɫ�б��У�����true
					if (Roles.IsUserInRole(identity.GetUserId(), eventRole.Name))
					{
						return true;
					}
					else
					{
						//�����ǰ�û��ı���Ȩ�б������û�Id����
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
		/// У���Ƿ�ɽ��г�������
		/// </summary>
		/// <returns></returns>
		private bool CanDoCancelAction(ApprovalActivity lastActivity)
		{
			if (LastActivity == null)
				throw new WorkflowExecuteExeception("û�в���ɹ�����");
			else
			{
				//ֻ�лִ�����г���Ȩ��
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

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace OilDigital.Workflows
{
	/// <summary>
	/// �����Activity��,���л�Ļ���
	/// </summary>
	public abstract class Activity
	{
		/// <summary>
		/// ��������ΨһID
		/// </summary>
		/// <value>The id.</value>
		public abstract long Id { get; set; }
		/// <summary>
		/// ��������Ե��¼�����
		/// </summary>
		/// <value>The name of the event.</value>
		public abstract string EventName { get; set; }
		/// <summary>
		/// ִ�л�û�Id
		/// </summary>
		/// <value>The user id.</value>
		public abstract string UserId { get; set; }
		/// <summary>
		/// �û���ִ�иö���ʱ���ݵ�������ɫ
		/// </summary>
		/// <value>The user approval role.</value>
		public abstract string UserApprovalRole { get; set; }
		/// <summary>
		/// Called when [initialize].
		/// </summary>
		/// <param name="instance">The instance.</param>
		internal abstract void OnInitialize(WorkflowInstance instance);
		/// <summary>
		/// ��ʼ��������Դ
		/// </summary>
		protected abstract void Initialize(WorkflowInstance instance);
		/// <summary>
		/// Called when [execute].
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <returns></returns>
		internal abstract ActivityExecutionResult OnExecute(WorkflowInstance instance);
		/// <summary>
		/// ִ�ж���
		/// </summary>
		/// <param name="instance">��ǰ������ԵĹ�����ʵ��</param>
		/// <returns>����ִ�еĽ��(��ʱ����)</returns>
		protected abstract ActivityExecutionResult Execute(WorkflowInstance instance);
		/// <summary>
		/// Called when [undo].
		/// </summary>
		/// <param name="instance">The instance.</param>
		internal abstract void OnUndo(WorkflowInstance instance);
		/// <summary>
		/// ��������.�ڹ�����ʵ����,����û�ִ�г�������,��ôϵͳ��ִ�й���������ĳ�������֮ǰ,����Ҫִ���ض������ĳ�������.
		/// </summary>
		/// <param name="instance">The instance.</param>
		protected abstract void Undo(WorkflowInstance instance);
		/// <summary>
		/// 	<seealso cref="ActivityExecutionStatus"/>ö�����͵�ʵ��,��ʾ������ִ��״̬,һ����п��ֶܷ��ִ��(����һ����������λ�����,�ظ���������Ҫ��������λ��Ҫ���ִ��,ֻ�����е�λ�Ļظ���
		/// ����,������ǩ�ظ��ſ��Խ���
		/// </summary>
		/// <value>The execution status.</value>
		public abstract ActivityExecutionStatus ExecutionStatus { get; }
	}

	/// <summary>
	/// ���������Ļ���,ʵ�ֳ�����<seealso cref="Activity"/>�Ľӿ�,���ǲ�ִ���κζ���,�Զ��嶯�������Դ���Ϊ����.
	/// </summary>
	public abstract class ActivityBase : Activity
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ActivityBase"/> class.
		/// </summary>
		public ActivityBase() { }
		private long id = 0;
		/// <summary>
		/// ��������ΨһID
		/// </summary>
		/// <value>The id.</value>
		public override long Id
		{
			get { return this.id; }
			set { this.id = value; }
		}

		private string eventName;
		/// <summary>
		/// ��������Ե��¼�����
		/// </summary>
		public override string EventName
		{
			get { return eventName; }
			set { eventName = value; }
		}

		private string userId;
		/// <summary>
		/// �ִ�����û�Id
		/// </summary>
		public override string UserId
		{
			get { return userId; }
			set { userId = value; }
		}

		private string userApprovalRole;
		/// <summary>
		/// �û���ִ�иö���ʱ���ݵ�������ɫ
		/// </summary>
		public override string UserApprovalRole
		{
			get { return userApprovalRole; }
			set { userApprovalRole = value; }
		}
		private int recordId;
		/// <summary>
		/// ������¼Id
		/// </summary>
		public int RecordId
		{
			get { return recordId; }
			set { recordId = value; }
		}

		private string solutionInfo;
		/// <summary>
		/// �������
		/// </summary>
		public string SolutionInfo
		{
			get { return solutionInfo; }
			set { solutionInfo = value; }
		}

		/// <summary>
		/// ����ִ������Ժ���¼�����
		/// </summary>
		public event EventHandler<ActivityExecutedArg> OnActivityExecuted;
		/// <summary>
		/// ��ʼ���ִ��ǰ��һЩ��Դ
		/// </summary>
		/// <param name="instance">The instance.</param>
		internal override void OnInitialize(WorkflowInstance instance)
		{
			Initialize(instance);
		}
		/// <summary>
		/// ��ִ��Execute����Ǯ����ʼ����������Ϣ
		/// </summary>
		/// <param name="instance"></param>
		protected override void Initialize(WorkflowInstance instance) { }
		private string activityName;

		/// <summary>
		/// Gets or sets the name of the activity.
		/// </summary>
		/// <value>The name of the activity.</value>
		public string ActivityName
		{
			get { return activityName; }
			set { activityName = value; }
		}
		/// <summary>
		/// �������͵Ķ���ִ�н��<seealso cref="ActivityExecutionResult"/>ö�����͵�ʵ��,���ʼֵΪActivityExecutionResult.Succeeded;
		/// </summary>
		protected ActivityExecutionResult ExecutionResult = ActivityExecutionResult.Succeeded;
		internal override ActivityExecutionResult OnExecute(WorkflowInstance instance)
		{
			ActivityExecutionResult result = Execute(instance);
			TrackExecute(instance);
			if(this.OnActivityExecuted!=null)
				this.OnActivityExecuted(this,new ActivityExecutedArg(instance,result));
			return result;
		}
		/// <summary>
		/// ִ����������ʱ���е����������¼����,���Ҫ�ò�ͬ�ļ�¼��������д�÷���
		/// </summary>
		/// <param name="instance">The instance.</param>
		protected virtual void TrackExecute(WorkflowInstance instance)
		{
			ApprovalRecord record = new ApprovalRecord();
			IIdentityService service = WorkflowRuntime.Current.GetService<IIdentityService>();
			if (service == null)
				throw new WorkflowExecuteExeception("�����Ϣ�ṩ����Ϊ��");
			IUserIdentity userInfo = service.GetUserIdentity();
			record.OperatorTime = DateTime.Now;
			record.WorkflowInstanceId = instance.Id;
			string currentUserId = userInfo.GetUserId();
			record.OperatorId = userId;
			record.OperatorName = userInfo.GetUserName();
			record.OperatorUnitCode = userInfo.GetUserUnitCode();

			ResetRecordAndUserId(record);
			
			record.OperatorRole = this.UserApprovalRole;
			record.EaId = instance.EaId;
			StateMachineWorkflowInstance stateMachine = instance as StateMachineWorkflowInstance;

			//���û�ж���������,��ô��ʱ���������ȡ����
			if (string.IsNullOrEmpty(this.activityName))
				this.activityName = stateMachine.CurrentState.GetEventByName(eventName).Description;
			record.ApprovalType = this.ActivityName;
			record.StateName = stateMachine.CurrentState.Description;
			record.SolutionInfo = solutionInfo;
			WorkflowRuntime.Current.GetService<IApprovalSaveService>().InsertRecord(record);
			this.recordId = record.Id;
		}

		/// <summary>
		/// �ڼ�¼������־ǰ,��Ҫ�жϴ�������������ò�����¼,����Ǵ�����ô�����û�������Ϊ��ʽ***(��***),
		/// ͬʱ��Ҫ���õ�ǰ������UserIdΪ��ǰ����ִ�иĸö������û�,�������ִ���߿��Գ�������,�÷�����������
		/// </summary>
		/// <param name="record">The record.</param>
		protected virtual void ResetRecordAndUserId(ApprovalRecord record)
		{
			IIdentityService service = WorkflowRuntime.Current.GetService<IIdentityService>();
			IUserIdentity userInfo = service.GetUserIdentity();
			string currentUserId = userInfo.GetUserId();
			if (currentUserId != userId)
			{
				IUserIdentity userIdentity = service.GetUserIdentity(userId);
				record.OperatorName = userInfo.GetUserName() + "(��" + userIdentity.GetUserName() + ")";
				record.OperatorUnitCode = userIdentity.GetUserUnitCode();
				userId = currentUserId;
			}
		}
		/// <summary>
		/// ִ�ж���
		/// </summary>
		/// <param name="instance">��ǰ������ԵĹ�����ʵ��</param>
		/// <returns>����ִ�еĽ��(��ʱ����)</returns>
		protected override ActivityExecutionResult Execute(WorkflowInstance instance)
		{
			return ActivityExecutionResult.Succeeded;
		}
		/// <summary>
		/// �����û��ִ�еķ���
		/// </summary>
		/// <param name="instance">The instance.</param>
		internal override void OnUndo(WorkflowInstance instance)
		{
			Undo(instance);
			TrackUndo(instance);
		}

		/// <summary>
		/// ��������ʱ���е����������¼����,���Ҫ�ò�ͬ�ļ�¼��������д�÷���
		/// </summary>
		/// <param name="instance">The instance.</param>
		protected virtual void TrackUndo(WorkflowInstance instance)
		{
			ApprovalRecord record = new ApprovalRecord();
			IIdentityService service = WorkflowRuntime.Current.GetService<IIdentityService>();
			IApprovalSaveService approvalService = WorkflowRuntime.Current.GetService<IApprovalSaveService>();
			if (service == null)
				throw new WorkflowExecuteExeception("�����Ϣ�ṩ����Ϊ��");
			IUserIdentity userInfo = service.GetUserIdentity();
			record.OperatorTime = DateTime.Now;
			record.WorkflowInstanceId = instance.Id;
			record.OperatorId = userInfo.GetUserId();
			record.OperatorName = userInfo.GetUserName();
			record.OperatorUnitCode = userInfo.GetUserUnitCode();
			record.OperatorRole = this.UserApprovalRole;
			record.EaId = instance.EaId;

			StateMachineWorkflowInstance stateMachine = instance as StateMachineWorkflowInstance;
			record.ApprovalType = GetUndoName(instance);
			record.StateName = stateMachine.CurrentState.Description;
			if (recordId != 0)
			{
				ApprovalRecord historyRecord = approvalService.GetRecordById(recordId);
				historyRecord.IsCanceled = true;
				approvalService.SaveRecord(historyRecord);
			}
			WorkflowRuntime.Current.GetService<IApprovalSaveService>().InsertRecord(record);
		}
		/// <summary>
		/// ��������.�ڹ�����ʵ����,����û�ִ�г�������,��ôϵͳ��ִ�й���������ĳ�������֮ǰ,����Ҫִ���ض������ĳ�������.
		/// </summary>
		/// <param name="instance">The instance.</param>
		protected override void Undo(WorkflowInstance instance) { }
		/// <summary>
		/// �ڳ�������ʱ,��¼������(���ڼ�¼��������־��)
		/// </summary>
		/// <param name="instance">������ʵ��</param>
		/// <returns></returns>
		protected virtual string GetUndoName(WorkflowInstance instance)
		{
			return string.IsNullOrEmpty(this.ActivityName) ? "��������" : string.Format("������{0}������", this.ActivityName);
		}
		/// <summary>
		/// <seealso cref="ActivityExecutionStatus"/>ö�����͵�ʵ��,���ʼֵΪActivityExecutionStatus.Closed;
		/// </summary>
		private ActivityExecutionStatus executionStatus = ActivityExecutionStatus.Closed;
		/// <summary>
		/// 	<seealso cref="ActivityExecutionStatus"/>ö�����͵�ʵ��,��ʾ������ִ��״̬,һ����п��ֶܷ��ִ��(����һ����������λ�����,�ظ���������Ҫ��������λ��Ҫ���ִ��,ֻ�����е�λ�Ļظ���
		/// ����,������ǩ�ظ��ſ��Խ���
		/// </summary>
		/// <value>The execution status.</value>
		public override ActivityExecutionStatus ExecutionStatus
		{
			get { return executionStatus; }
		}
		/// <summary>
		/// ���ö���ִ�е�״̬
		/// </summary>
		/// <param name="executionStatus"></param>
		protected void SetExecutionStatus(ActivityExecutionStatus executionStatus)
		{
			this.executionStatus = executionStatus;
		}
	}
}

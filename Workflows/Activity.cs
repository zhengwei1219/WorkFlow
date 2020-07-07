using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 抽象的Activity类,所有活动的基类
	/// </summary>
	public abstract class Activity
	{
		/// <summary>
		/// 代表动作的唯一ID
		/// </summary>
		/// <value>The id.</value>
		public abstract long Id { get; set; }
		/// <summary>
		/// 工作流针对的事件对象
		/// </summary>
		/// <value>The name of the event.</value>
		public abstract string EventName { get; set; }
		/// <summary>
		/// 执行活动用户Id
		/// </summary>
		/// <value>The user id.</value>
		public abstract string UserId { get; set; }
		/// <summary>
		/// 用户在执行该动作时扮演的审批角色
		/// </summary>
		/// <value>The user approval role.</value>
		public abstract string UserApprovalRole { get; set; }
		/// <summary>
		/// Called when [initialize].
		/// </summary>
		/// <param name="instance">The instance.</param>
		internal abstract void OnInitialize(WorkflowInstance instance);
		/// <summary>
		/// 初始化所用资源
		/// </summary>
		protected abstract void Initialize(WorkflowInstance instance);
		/// <summary>
		/// Called when [execute].
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <returns></returns>
		internal abstract ActivityExecutionResult OnExecute(WorkflowInstance instance);
		/// <summary>
		/// 执行动作
		/// </summary>
		/// <param name="instance">当前动作针对的工作流实例</param>
		/// <returns>返回执行的结果(暂时不用)</returns>
		protected abstract ActivityExecutionResult Execute(WorkflowInstance instance);
		/// <summary>
		/// Called when [undo].
		/// </summary>
		/// <param name="instance">The instance.</param>
		internal abstract void OnUndo(WorkflowInstance instance);
		/// <summary>
		/// 撤销动作.在工作流实例中,如果用户执行撤销操作,那么系统在执行工作流本身的撤销操作之前,还需要执行特定动作的撤销操作.
		/// </summary>
		/// <param name="instance">The instance.</param>
		protected abstract void Undo(WorkflowInstance instance);
		/// <summary>
		/// 	<seealso cref="ActivityExecutionStatus"/>枚举类型的实例,表示工作流执行状态,一个活动有可能分多次执行(比如一次征求多个单位的意见,回复操作可能要被各个单位需要多次执行,只有所有单位的回复都
		/// 答复了,整个会签回复才可以结束
		/// </summary>
		/// <value>The execution status.</value>
		public abstract ActivityExecutionStatus ExecutionStatus { get; }
	}

	/// <summary>
	/// 审批动作的基类,实现抽象类<seealso cref="Activity"/>的接口,但是不执行任何动作,自定义动作可以以此作为基类.
	/// </summary>
	public abstract class ActivityBase : Activity
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ActivityBase"/> class.
		/// </summary>
		public ActivityBase() { }
		private long id = 0;
		/// <summary>
		/// 代表动作的唯一ID
		/// </summary>
		/// <value>The id.</value>
		public override long Id
		{
			get { return this.id; }
			set { this.id = value; }
		}

		private string eventName;
		/// <summary>
		/// 工作流针对的事件对象
		/// </summary>
		public override string EventName
		{
			get { return eventName; }
			set { eventName = value; }
		}

		private string userId;
		/// <summary>
		/// 活动执行者用户Id
		/// </summary>
		public override string UserId
		{
			get { return userId; }
			set { userId = value; }
		}

		private string userApprovalRole;
		/// <summary>
		/// 用户在执行该动作时扮演的审批角色
		/// </summary>
		public override string UserApprovalRole
		{
			get { return userApprovalRole; }
			set { userApprovalRole = value; }
		}
		private int recordId;
		/// <summary>
		/// 审批记录Id
		/// </summary>
		public int RecordId
		{
			get { return recordId; }
			set { recordId = value; }
		}

		private string solutionInfo;
		/// <summary>
		/// 审批意见
		/// </summary>
		public string SolutionInfo
		{
			get { return solutionInfo; }
			set { solutionInfo = value; }
		}

		/// <summary>
		/// 动作执行完毕以后的事件处理
		/// </summary>
		public event EventHandler<ActivityExecutedArg> OnActivityExecuted;
		/// <summary>
		/// 初始化活动执行前的一些资源
		/// </summary>
		/// <param name="instance">The instance.</param>
		internal override void OnInitialize(WorkflowInstance instance)
		{
			Initialize(instance);
		}
		/// <summary>
		/// 在执行Execute方法钱，初始化活动的相关信息
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
		/// 保护类型的动作执行结果<seealso cref="ActivityExecutionResult"/>枚举类型的实例,其初始值为ActivityExecutionResult.Succeeded;
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
		/// 执行审批操作时进行的相关审批记录操作,如果要用不同的记录，必须重写该方法
		/// </summary>
		/// <param name="instance">The instance.</param>
		protected virtual void TrackExecute(WorkflowInstance instance)
		{
			ApprovalRecord record = new ApprovalRecord();
			IIdentityService service = WorkflowRuntime.Current.GetService<IIdentityService>();
			if (service == null)
				throw new WorkflowExecuteExeception("身份信息提供服务为空");
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

			//如果没有定义活动的名称,那么从时间的描述中取出来
			if (string.IsNullOrEmpty(this.activityName))
				this.activityName = stateMachine.CurrentState.GetEventByName(eventName).Description;
			record.ApprovalType = this.ActivityName;
			record.StateName = stateMachine.CurrentState.Description;
			record.SolutionInfo = solutionInfo;
			WorkflowRuntime.Current.GetService<IApprovalSaveService>().InsertRecord(record);
			this.recordId = record.Id;
		}

		/// <summary>
		/// 在记录操作日志前,需要判断代办情况重新设置操作记录,如果是代办那么操作用户名设置为格式***(代***),
		/// 同时需要设置当前动作的UserId为当前真正执行改该动作的用户,以允许该执行者可以撤销操作,该方法允许被重载
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
				record.OperatorName = userInfo.GetUserName() + "(代" + userIdentity.GetUserName() + ")";
				record.OperatorUnitCode = userIdentity.GetUserUnitCode();
				userId = currentUserId;
			}
		}
		/// <summary>
		/// 执行动作
		/// </summary>
		/// <param name="instance">当前动作针对的工作流实例</param>
		/// <returns>返回执行的结果(暂时不用)</returns>
		protected override ActivityExecutionResult Execute(WorkflowInstance instance)
		{
			return ActivityExecutionResult.Succeeded;
		}
		/// <summary>
		/// 撤销该活动是执行的方法
		/// </summary>
		/// <param name="instance">The instance.</param>
		internal override void OnUndo(WorkflowInstance instance)
		{
			Undo(instance);
			TrackUndo(instance);
		}

		/// <summary>
		/// 撤销操作时进行的相关审批记录操作,如果要用不同的记录，必须重写该方法
		/// </summary>
		/// <param name="instance">The instance.</param>
		protected virtual void TrackUndo(WorkflowInstance instance)
		{
			ApprovalRecord record = new ApprovalRecord();
			IIdentityService service = WorkflowRuntime.Current.GetService<IIdentityService>();
			IApprovalSaveService approvalService = WorkflowRuntime.Current.GetService<IApprovalSaveService>();
			if (service == null)
				throw new WorkflowExecuteExeception("身份信息提供服务为空");
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
		/// 撤销动作.在工作流实例中,如果用户执行撤销操作,那么系统在执行工作流本身的撤销操作之前,还需要执行特定动作的撤销操作.
		/// </summary>
		/// <param name="instance">The instance.</param>
		protected override void Undo(WorkflowInstance instance) { }
		/// <summary>
		/// 在撤销操作时,记录的名称(用于记录到操作日志中)
		/// </summary>
		/// <param name="instance">工作流实例</param>
		/// <returns></returns>
		protected virtual string GetUndoName(WorkflowInstance instance)
		{
			return string.IsNullOrEmpty(this.ActivityName) ? "撤销操作" : string.Format("撤销“{0}”操作", this.ActivityName);
		}
		/// <summary>
		/// <seealso cref="ActivityExecutionStatus"/>枚举类型的实例,其初始值为ActivityExecutionStatus.Closed;
		/// </summary>
		private ActivityExecutionStatus executionStatus = ActivityExecutionStatus.Closed;
		/// <summary>
		/// 	<seealso cref="ActivityExecutionStatus"/>枚举类型的实例,表示工作流执行状态,一个活动有可能分多次执行(比如一次征求多个单位的意见,回复操作可能要被各个单位需要多次执行,只有所有单位的回复都
		/// 答复了,整个会签回复才可以结束
		/// </summary>
		/// <value>The execution status.</value>
		public override ActivityExecutionStatus ExecutionStatus
		{
			get { return executionStatus; }
		}
		/// <summary>
		/// 设置动作执行的状态
		/// </summary>
		/// <param name="executionStatus"></param>
		protected void SetExecutionStatus(ActivityExecutionStatus executionStatus)
		{
			this.executionStatus = executionStatus;
		}
	}
}

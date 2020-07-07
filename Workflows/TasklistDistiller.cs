using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 获取用户的任务列表类,
	/// </summary>
	public abstract class TasklistDistiller : ITasklistDistiller
	{
		/// <summary>
		/// 构造一个当前用户的用于获取任务列表的对象(distiller提取者)
		/// </summary>
		/// <param name="applicationName">Name of the application.</param>
		public TasklistDistiller(string applicationName)
			: this(WorkflowRuntime.Current.GetService<IIdentityService>().GetUserIdentity(), applicationName)
		{ }
		/// <summary>
		/// 构造一个某用户的用于获取任务列表的对象(distiller提取者)
		/// </summary>
		/// <param name="userIdentity">用户的身份信息</param>
		/// <param name="applicationName">Name of the application.</param>
		public TasklistDistiller(IUserIdentity userIdentity,string applicationName)
		{
			if(userIdentity == null)
				throw new ArgumentNullException("userIdentity");
			this.userIdentity = userIdentity;
			this.applicationName = applicationName;
			workflowNames = WorkflowRuntime.Current.GetService<IWorkFlowDefinePersistService>().GetAllWorkflowDefineName(applicationName);
		}
		private string applicationName;
		/// <summary>
		/// Gets the name of the application.
		/// </summary>
		/// <value>The name of the application.</value>
		protected string ApplicationName
		{
			get { return applicationName; }
		}
		private IUserIdentity userIdentity;
		/// <summary>
		/// 用户的身份信息
		/// </summary>
		protected IUserIdentity UserIdentity
		{
			get { return this.userIdentity; }
		}
		private string[] workflowNames;
		/// <summary>
		/// 所有工作流流程名称集合
		/// </summary>
		protected string[] WorkflowNames
		{
			get { return this.workflowNames; }
		}
		/// <summary>
		/// 返回一个可获取实例提取者ApprovalProcess对象
		/// </summary>
		/// <param name="workflowName">工作流名称</param>
		/// <param name="role">审批角色</param>
		/// <returns></returns>
		protected abstract ApprovalProcess CreateProcess(string workflowName, ApprovalRole role);
		/// <summary>
		/// 获取或设置当前的任务列表转换成任务项目的处理器
		/// </summary>
		/// <returns></returns>
		public abstract TaskItemProcessor ItemProcessor { get;set;}
		/// <summary>
		/// 获取流程所有的任务名称
		/// </summary>
		/// <returns></returns>
		public string[] GetTaskNames()
		{
			List<string> taskNameList = new List<string>();
			foreach (string workflowName in workflowNames)
			{
				string[] taskList = GetTaskNames(workflowName);
				foreach (string taskName in taskList)
				{
					if (!taskNameList.Contains(taskName))
						taskNameList.Add(taskName);
				}
			}
			taskNameList.Sort();
			return taskNameList.ToArray();
		}
		/// <summary>
		/// 获取某流程的所有任务的名称
		/// </summary>
		/// <param name="workflowName">流程的名称</param>
		/// <returns></returns>
		public string[] GetTaskNames(string workflowName)
		{
			List<ApprovalRole> roles = WorkflowUtility.GetUserRoles(workflowName, userIdentity.GetUserId());
			List<string> taskNameList = new List<string>();
			foreach (ApprovalRole role in roles)
			{
				List<string> taskList = CreateProcess(workflowName, role).GetTaskNameList();
				foreach (string taskName in taskList)
				{
					if (!taskNameList.Contains(taskName))
						taskNameList.Add(taskName);
				}
			}
			taskNameList.Sort();
			return taskNameList.ToArray();
		}
		/// <summary>
		/// 获取待办任务列表
		/// </summary>
		/// <returns></returns>
		public TaskList[] GetTasklists()
		{
			List<TaskList> result = new List<TaskList>();
			string[] taskNames = GetTaskNames();
			foreach (string taskName in taskNames)
			{
				result.Add(GetTask(taskName));
			}
			return result.ToArray();
		}
		/// <summary>
		/// 获取用户待办信息字符串描述
		/// </summary>
		/// <returns></returns>
		public abstract string GetTaskListInfoString();
		/// <summary>
		/// 按任务名称获取一个该任务的任务列表
		/// </summary>
		/// <param name="taskName">Name of the task.</param>
		/// <returns></returns>
		public TaskList GetTask(string taskName)
		{
			List<ITaskItem> items = new List<ITaskItem>();
			foreach (string workflowName in workflowNames)
			{
				items.AddRange(GetTask(workflowName, taskName).Items);
			}
			return new TaskList(taskName, items);
		}
		/// <summary>
		/// 获取某工作流程的某任务列表
		/// </summary>
		/// <param name="workflowName">工作流名称</param>
		/// <param name="taskName">任务名称</param>
		/// <returns></returns>
		public TaskList GetTask(string workflowName, string taskName)
		{
			List<ApprovalRole> roles = WorkflowUtility.GetUserRoles(workflowName, userIdentity.GetUserId());
			List<StateMachineWorkflowInstance> instanceList = new List<StateMachineWorkflowInstance>();
			foreach (ApprovalRole role in roles)
			{
				instanceList.AddRange(CreateProcess(workflowName, role).GetInstanceList(taskName));
			}
			return new TaskList(taskName, ItemProcessor.GenerateTaskTable(instanceList));
		}
		/// <summary>
		/// 获取某流程的所有任务列表
		/// </summary>
		/// <param name="workflowName">流程名称</param>
		/// <returns></returns>
		public TaskList[] GetTaskListsByWorkflow(string workflowName)
		{
			List<TaskList> result = new List<TaskList>();
			List<ApprovalRole> roles = WorkflowUtility.GetUserRoles(workflowName, userIdentity.GetUserId());
			string[] taskNames = GetTaskNames(workflowName);
			foreach (string taskName in taskNames)
			{
				result.Add(GetTask(workflowName, taskName));
			}
			return result.ToArray();
		}
		/// <summary>
		/// 获取可以撤销操作的任务列表
		/// </summary>
		/// <param name="taskName">Name of the task.</param>
		/// <returns></returns>
		public TaskList GetAllowedCancelTask(string taskName)
		{
			List<ITaskItem> taskItemsList = new List<ITaskItem>();
			//遍历每一个工作流获取任务
			foreach (string workflowName in workflowNames)
			{
				taskItemsList.AddRange(GetAllowedCancelTask(workflowName, taskName).Items);
			}
			TaskList list = new TaskList("a." + taskName, taskItemsList);
			return list;
		}
		/// <summary>
		/// 获取某流程的可以撤销操作的任务列表
		/// </summary>
		/// <returns></returns>
		public TaskList GetAllowedCancelTask(string workflowName, string taskName)
		{
			List<ITaskItem> taskItemsList = new List<ITaskItem>();
			List<ApprovalRole> roles = WorkflowUtility.GetUserRoles(workflowName, userIdentity.GetUserId());
			List<StateMachineWorkflowInstance> instanceList = new List<StateMachineWorkflowInstance>();
			foreach (ApprovalRole role in roles)
			{
				if (!string.IsNullOrEmpty(role.Name))
				{
					instanceList.AddRange(CreateProcess(workflowName, role).GetCancelAllowedTaskList());
				}
			}
			TaskList list = new TaskList("a." + taskName, ItemProcessor.GenerateTaskTable(instanceList));
			return list;
		}
		/// <summary>
		/// 获取某用户的代理办理项目列表
		/// </summary>
		/// <param name="taskName">任务名称</param>
		/// <param name="agentInfoId">待办记录的Id</param>
		/// <returns></returns>
		public virtual TaskList GetAgentProceedList(string taskName, int agentInfoId)
		{
			IApprovalSaveService service = WorkflowRuntime.Current.GetService<IApprovalSaveService>();
			ApprovalAgent agentInfo = service.GetAgentInfoById(agentInfoId);
			string agentUserId = agentInfo.ToUserId;
			string agentUserName = agentInfo.ToUserName;
			IUserIdentity setAgentUserIdentity = WorkflowRuntime.Current.GetService<IIdentityService>().GetUserIdentity(agentInfo.SetUserId);
			string setAgentUserName = setAgentUserIdentity.GetUserName();
			string agentRecordUserInfo = agentUserName + "(代" + setAgentUserName + ")";
			List<ApprovalRecord> recordList = new List<ApprovalRecord>();
			string[] workflowNames = WorkflowRuntime.Current.DefineService.GetAllWorkflowDefineName(applicationName);
			foreach (string workflowName in workflowNames)
			{
				WorkflowRuntime.Current.GetService<IApprovalSaveService>().GetRecord(workflowName, agentInfo.BeginDate, agentInfo.EndDate, agentInfo.SetUserId);
			}
			List<int> eaIds = new List<int>();
			foreach (ApprovalRecord record in recordList)
			{
				if (record.OperatorName == agentRecordUserInfo)
				{
					if (!eaIds.Contains(record.EaId))
						eaIds.Add(record.EaId);
				}
			}
			List<StateMachineWorkflowInstance> instanceList = new List<StateMachineWorkflowInstance>();
			foreach (int eaid in eaIds)
			{
				List<StateMachineWorkflowInstance> instances = WorkflowRuntime.Current.GetInstance(applicationName, eaid, false);
				foreach (StateMachineWorkflowInstance instance in instances)
				{
					if (instance.ParentId == null || instance.ParentId == Guid.Empty)
					{
						instanceList.Add(instance);
						break;
					}
				}
			}
			return new TaskList("a." + taskName, ItemProcessor.GenerateTaskTable(instanceList, true));
		}
		/// <summary>
		/// Gets the proceed list.
		/// </summary>
		/// <param name="startDate">The start date.</param>
		/// <param name="endDate">The end date.</param>
		/// <param name="userId">The user id.</param>
		/// <returns></returns>
		protected List<StateMachineWorkflowInstance> GetProceedList(DateTime startDate, DateTime endDate, string userId)
		{
			List<ApprovalRecord> recordList = new List<ApprovalRecord>();
			string[] workflowNames = WorkflowRuntime.Current.DefineService.GetAllWorkflowDefineName(applicationName);
			foreach (string workflowName in workflowNames)
			{
				WorkflowRuntime.Current.GetService<IApprovalSaveService>().GetRecord(workflowName, startDate, endDate, userId);
			}
			
			List<int> eaIds = new List<int>();
			foreach (ApprovalRecord record in recordList)
			{
				if (!eaIds.Contains(record.EaId))
					eaIds.Add(record.EaId);
			}
			List<StateMachineWorkflowInstance> instanceList = new List<StateMachineWorkflowInstance>();
			foreach (int eaid in eaIds)
			{
				List<StateMachineWorkflowInstance> instances = WorkflowRuntime.Current.GetInstance(applicationName, eaid, false);
				foreach (StateMachineWorkflowInstance instance in instances)
				{
					StateMachineWorkflow workflow = (StateMachineWorkflow)instance.Workflow;
					if (instance.ParentId == null || instance.ParentId == Guid.Empty)
					{
						instanceList.Add(instance);
						break;
					}
				}
			}
			return instanceList;
		}
		/// <summary>
		/// 按最后操作时间范围获取已处理过的任务列表
		/// </summary>
		/// <param name="taskName">审批过的任务名称</param>
		/// <param name="startDate">最后操作时间范围起始时间</param>
		/// <param name="endDate">最后操作时间范围终了时间</param>
		/// <returns></returns>
		public virtual TaskList GetProcessedList(string taskName, DateTime startDate, DateTime endDate)
		{
			List<StateMachineWorkflowInstance> instanceList = GetProceedList(startDate, endDate, userIdentity.GetUserId());
			for (int i = instanceList.Count - 1; i >= 0; i--)
			{
				if (instanceList[i].IsEnd() || instanceList[i].IsTerminated())
					instanceList.RemoveAt(i);
			}
			return new TaskList("a." + taskName, ItemProcessor.GenerateTaskTable(instanceList, true));
		}
		/// <summary>
		/// 按最后操作时间范围获取正在审批中的所有项目任务列表
		/// </summary>
		/// <param name="mainWorkflowName">Name of the main workflow.</param>
		/// <param name="isByUnit">if set to <c>true</c> [is by unit].</param>
		/// <returns></returns>
		public virtual TaskList[] GetProcessingLists(string mainWorkflowName, bool isByUnit)
		{
			string unitCode = "";
			if (isByUnit)
				unitCode = userIdentity.GetUserUnitCode();
			List<TaskList> result = new List<TaskList>();
			Dictionary<string, string> stateNames = new Dictionary<string, string>();
			string[] workflows = WorkflowRuntime.Current.GetService<IWorkFlowDefinePersistService>().GetAllWorkflowDefineName(applicationName);
			StateMachineWorkflow stateMachine = WorkflowRuntime.Current.GetService<IWorkFlowDefinePersistService>().GetWorkflowDefine(mainWorkflowName) as StateMachineWorkflow;
			AddApprovalStates(mainWorkflowName, stateNames);
			foreach (string stateDes in stateNames.Keys)
			{
				string stateName = stateNames[stateDes];
				List<StateMachineWorkflowInstance> instanceList = new List<StateMachineWorkflowInstance>();
				instanceList.AddRange(WorkflowRuntime.Current.GetInstance(mainWorkflowName, DateTime.Now.AddYears(-5), DateTime.Now.AddDays(1), new string[] { stateName }, unitCode));
				result.Add(new TaskList("a." + stateDes, ItemProcessor.GenerateTaskTable(instanceList, true)));
			}
			return result.ToArray();
		}
		/// <summary>
		/// 递归获取所有流程的所有审批状态
		/// </summary>
		/// <param name="workflowName">流程名称</param>
		/// <param name="dicStates">状态字典</param>
		protected void AddApprovalStates(string workflowName, Dictionary<string, string> dicStates)
		{
			StateMachineWorkflow stateMachine = WorkflowRuntime.Current.GetService<IWorkFlowDefinePersistService>().GetWorkflowDefine(workflowName) as StateMachineWorkflow;
			foreach (ApprovalState state in stateMachine.States)
			{
				if (state.IsApprovalState && !dicStates.ContainsKey(state.Description))
					dicStates.Add(state.Description, state.Name);
				else
					continue;
			}
		}
		/// <summary>
		/// 按最后操作时间范围获取已审批结束的立项列表
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="taskName">Name of the task.</param>
		/// <param name="startDate">The start date.</param>
		/// <param name="endDate">The end date.</param>
		/// <param name="isByUnit">if set to <c>true</c> [is by unit].</param>
		/// <returns></returns>
		public virtual TaskList GetCompletedList(string workflowName, string taskName, DateTime startDate, DateTime endDate, bool isByUnit)
		{
			string unitCode = "";
			if (isByUnit)
				unitCode = userIdentity.GetUserUnitCode();
			string endStateName = ((StateMachineWorkflow)WorkflowRuntime.Current.GetService<IWorkFlowDefinePersistService>().GetWorkflowDefine(workflowName)).EndState;
			List<StateMachineWorkflowInstance> instanceList = WorkflowRuntime.Current.GetInstance(workflowName, startDate, endDate, new string[] { endStateName }, unitCode);
			return new TaskList("a." + taskName, ItemProcessor.GenerateTaskTable(instanceList, true));
		}
		/// <summary>
		/// Gets the batch process list.
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="batchProcessType">Type of the batch process.</param>
		/// <returns></returns>
		public virtual TaskList GetBatchProcessList(string workflowName, BatchProcessType batchProcessType)
		{
			StateMachineWorkflow workflow = (StateMachineWorkflow)WorkflowRuntime.Current.GetService<IWorkFlowDefinePersistService>().GetWorkflowDefine(workflowName);
			string stateName = string.Empty;
			ApprovalRole role = null;
			if (batchProcessType == BatchProcessType.Receive)
			{
				//获取操作为“接收”所在的状态名称
				foreach (ApprovalState state in workflow.States)
				{
					foreach (ApprovalEvent approvalEvent in state.Events)
					{
						if (approvalEvent.Name == "接收")
						{
							role = WorkflowUtility.GetUserRoleByName(workflowName, approvalEvent.Roles[0].Name);
							stateName = state.Name;
						}
					}
				}
			}
			else if (batchProcessType == BatchProcessType.Approve)
			{
				//遍历工作流状态，找到下一状态为工作流终止状态的状态名
				foreach (ApprovalState state in workflow.States)
				{
					foreach (ApprovalEvent approvalEvent in state.Events)
					{
						if (approvalEvent.NextStateNames != null && approvalEvent.NextStateNames.Length > 0 && approvalEvent.NextStateNames[0] == workflow.EndState)
						{
							role = WorkflowUtility.GetUserRoleByName(workflowName, approvalEvent.Roles[0].Name);
							stateName = state.Name;
						}
					}
				}
			}
			else
				throw new ArgumentException("batchProcessType");
			ApprovalProcess process = CreateProcess(workflowName, role);
			//获取所有在该状态下的工作流实例 
			List<StateMachineWorkflowInstance> instanceList = process.GetList(new string[] { stateName });
			return new TaskList("a." + batchProcessType.ToString(), new List<ITaskItem>(ItemProcessor.GenerateTaskTable(instanceList)));
		}
	}

	/// <summary>
	/// 获取任务列表的接口
	/// </summary>
	public interface ITasklistDistiller
	{
		/// <summary>
		/// Gets the task names.
		/// </summary>
		/// <returns></returns>
		string[] GetTaskNames();
		/// <summary>
		/// Gets the task names.
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <returns></returns>
		string[] GetTaskNames(string workflowName);

		/// <summary>
		/// Gets the tasklists.
		/// </summary>
		/// <returns></returns>
		TaskList[] GetTasklists();
		/// <summary>
		/// Gets the task lists by workflow.
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <returns></returns>
		TaskList[] GetTaskListsByWorkflow(string workflowName);
		/// <summary>
		/// Gets the task.
		/// </summary>
		/// <param name="taskName">Name of the task.</param>
		/// <returns></returns>
		TaskList GetTask(string taskName);
		/// <summary>
		/// Gets the task.
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="taskName">Name of the task.</param>
		/// <returns></returns>
		TaskList GetTask(string workflowName, string taskName);

		/// <summary>
		/// Gets the allowed cancel task.
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="taskName">Name of the task.</param>
		/// <returns></returns>
		TaskList GetAllowedCancelTask(string workflowName, string taskName);
		/// <summary>
		/// Gets the allowed cancel task.
		/// </summary>
		/// <param name="taskName">Name of the task.</param>
		/// <returns></returns>
		TaskList GetAllowedCancelTask(string taskName);

		/// <summary>
		/// Gets the completed list.
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="taskName">Name of the task.</param>
		/// <param name="startDate">The start date.</param>
		/// <param name="endDate">The end date.</param>
		/// <param name="isByUnit">if set to <c>true</c> [is by unit].</param>
		/// <returns></returns>
		TaskList GetCompletedList(string workflowName, string taskName, DateTime startDate, DateTime endDate, bool isByUnit);
		/// <summary>
		/// Gets the processed list.
		/// </summary>
		/// <param name="taskName">Name of the task.</param>
		/// <param name="startDate">The start date.</param>
		/// <param name="endDate">The end date.</param>
		/// <returns></returns>
		TaskList GetProcessedList(string taskName, DateTime startDate, DateTime endDate);
		/// <summary>
		/// Gets the processing lists.
		/// </summary>
		/// <param name="mainWorkflowName">Name of the main workflow.</param>
		/// <param name="isByUnit">if set to <c>true</c> [is by unit].</param>
		/// <returns></returns>
		TaskList[] GetProcessingLists(string mainWorkflowName, bool isByUnit);

		/// <summary>
		/// Gets the batch process list.
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="batchProcessType">Type of the batch process.</param>
		/// <returns></returns>
		TaskList GetBatchProcessList(string workflowName, BatchProcessType batchProcessType);
	}

	/// <summary>
	/// 
	/// </summary>
	public enum BatchProcessType
	{
		/// <summary>
		/// 
		/// </summary>
		Approve,
		/// <summary>
		/// 
		/// </summary>
		Receive
	}

	
}

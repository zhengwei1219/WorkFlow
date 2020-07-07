using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace OilDigital.Workflows
{
	/// <summary>
	/// ��ȡ�û��������б���,
	/// </summary>
	public abstract class TasklistDistiller : ITasklistDistiller
	{
		/// <summary>
		/// ����һ����ǰ�û������ڻ�ȡ�����б�Ķ���(distiller��ȡ��)
		/// </summary>
		/// <param name="applicationName">Name of the application.</param>
		public TasklistDistiller(string applicationName)
			: this(WorkflowRuntime.Current.GetService<IIdentityService>().GetUserIdentity(), applicationName)
		{ }
		/// <summary>
		/// ����һ��ĳ�û������ڻ�ȡ�����б�Ķ���(distiller��ȡ��)
		/// </summary>
		/// <param name="userIdentity">�û��������Ϣ</param>
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
		/// �û��������Ϣ
		/// </summary>
		protected IUserIdentity UserIdentity
		{
			get { return this.userIdentity; }
		}
		private string[] workflowNames;
		/// <summary>
		/// ���й������������Ƽ���
		/// </summary>
		protected string[] WorkflowNames
		{
			get { return this.workflowNames; }
		}
		/// <summary>
		/// ����һ���ɻ�ȡʵ����ȡ��ApprovalProcess����
		/// </summary>
		/// <param name="workflowName">����������</param>
		/// <param name="role">������ɫ</param>
		/// <returns></returns>
		protected abstract ApprovalProcess CreateProcess(string workflowName, ApprovalRole role);
		/// <summary>
		/// ��ȡ�����õ�ǰ�������б�ת����������Ŀ�Ĵ�����
		/// </summary>
		/// <returns></returns>
		public abstract TaskItemProcessor ItemProcessor { get;set;}
		/// <summary>
		/// ��ȡ�������е���������
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
		/// ��ȡĳ���̵��������������
		/// </summary>
		/// <param name="workflowName">���̵�����</param>
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
		/// ��ȡ���������б�
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
		/// ��ȡ�û�������Ϣ�ַ�������
		/// </summary>
		/// <returns></returns>
		public abstract string GetTaskListInfoString();
		/// <summary>
		/// ���������ƻ�ȡһ��������������б�
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
		/// ��ȡĳ�������̵�ĳ�����б�
		/// </summary>
		/// <param name="workflowName">����������</param>
		/// <param name="taskName">��������</param>
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
		/// ��ȡĳ���̵����������б�
		/// </summary>
		/// <param name="workflowName">��������</param>
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
		/// ��ȡ���Գ��������������б�
		/// </summary>
		/// <param name="taskName">Name of the task.</param>
		/// <returns></returns>
		public TaskList GetAllowedCancelTask(string taskName)
		{
			List<ITaskItem> taskItemsList = new List<ITaskItem>();
			//����ÿһ����������ȡ����
			foreach (string workflowName in workflowNames)
			{
				taskItemsList.AddRange(GetAllowedCancelTask(workflowName, taskName).Items);
			}
			TaskList list = new TaskList("a." + taskName, taskItemsList);
			return list;
		}
		/// <summary>
		/// ��ȡĳ���̵Ŀ��Գ��������������б�
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
		/// ��ȡĳ�û��Ĵ��������Ŀ�б�
		/// </summary>
		/// <param name="taskName">��������</param>
		/// <param name="agentInfoId">�����¼��Id</param>
		/// <returns></returns>
		public virtual TaskList GetAgentProceedList(string taskName, int agentInfoId)
		{
			IApprovalSaveService service = WorkflowRuntime.Current.GetService<IApprovalSaveService>();
			ApprovalAgent agentInfo = service.GetAgentInfoById(agentInfoId);
			string agentUserId = agentInfo.ToUserId;
			string agentUserName = agentInfo.ToUserName;
			IUserIdentity setAgentUserIdentity = WorkflowRuntime.Current.GetService<IIdentityService>().GetUserIdentity(agentInfo.SetUserId);
			string setAgentUserName = setAgentUserIdentity.GetUserName();
			string agentRecordUserInfo = agentUserName + "(��" + setAgentUserName + ")";
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
		/// ��������ʱ�䷶Χ��ȡ�Ѵ�����������б�
		/// </summary>
		/// <param name="taskName">����������������</param>
		/// <param name="startDate">������ʱ�䷶Χ��ʼʱ��</param>
		/// <param name="endDate">������ʱ�䷶Χ����ʱ��</param>
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
		/// ��������ʱ�䷶Χ��ȡ���������е�������Ŀ�����б�
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
		/// �ݹ��ȡ�������̵���������״̬
		/// </summary>
		/// <param name="workflowName">��������</param>
		/// <param name="dicStates">״̬�ֵ�</param>
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
		/// ��������ʱ�䷶Χ��ȡ�����������������б�
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
				//��ȡ����Ϊ�����ա����ڵ�״̬����
				foreach (ApprovalState state in workflow.States)
				{
					foreach (ApprovalEvent approvalEvent in state.Events)
					{
						if (approvalEvent.Name == "����")
						{
							role = WorkflowUtility.GetUserRoleByName(workflowName, approvalEvent.Roles[0].Name);
							stateName = state.Name;
						}
					}
				}
			}
			else if (batchProcessType == BatchProcessType.Approve)
			{
				//����������״̬���ҵ���һ״̬Ϊ��������ֹ״̬��״̬��
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
			//��ȡ�����ڸ�״̬�µĹ�����ʵ�� 
			List<StateMachineWorkflowInstance> instanceList = process.GetList(new string[] { stateName });
			return new TaskList("a." + batchProcessType.ToString(), new List<ITaskItem>(ItemProcessor.GenerateTaskTable(instanceList)));
		}
	}

	/// <summary>
	/// ��ȡ�����б�Ľӿ�
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

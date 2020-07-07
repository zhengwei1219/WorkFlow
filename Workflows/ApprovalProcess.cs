using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 管理获取任务列表的抽象基类
	/// </summary>
	public abstract class ApprovalProcess : IApprovalProcess
	{
		private IApprovalSaveService approvalService;
		private IWorkflowPersistService workflowPersistService;
		private StateMachineWorkflow workflow;
		/// <summary>
		/// Gets the workflow.
		/// </summary>
		/// <value>The workflow.</value>
		protected StateMachineWorkflow Workflow
		{
			get { return this.workflow; }
		}
		private IApprovalRules rules;
		/// <summary>
		/// Gets the rules.
		/// </summary>
		/// <value>The rules.</value>
		protected IApprovalRules Rules
		{
			get { return this.rules; }
		}
		private IUserIdentity userIdentity;
		/// <summary>
		/// 用户身份
		/// </summary>
		/// <value>The user identity.</value>
		protected IUserIdentity UserIdentity
		{
			get { return this.userIdentity; }
		}
		private string unitCode;
		/// <summary>
		/// 单位代码
		/// </summary>
		/// <value>The unit code.</value>
		protected string UnitCode
		{
			get { return unitCode; }
		}
		private string userId;
		/// <summary>
		/// 用户的Id
		/// </summary>
		/// <value>The user id.</value>
		protected string UserId
		{
			get { return this.userId; }
		}
		/// <summary>
		/// 根据工作流名称创建一个过程对象
		/// </summary>
		/// <param name="workflowName">工作流名称</param>
		/// <param name="rules">审批规则</param>
		/// <param name="userIdentity">用户省份</param>
		public ApprovalProcess(string workflowName, IApprovalRules rules, IUserIdentity userIdentity)
		{
			this.approvalService = WorkflowRuntime.Current.GetService<IApprovalSaveService>();
			this.workflowPersistService = WorkflowRuntime.Current.GetService<IWorkflowPersistService>();
			this.workflow = (StateMachineWorkflow)WorkflowRuntime.Current.GetService<IWorkFlowDefinePersistService>().GetWorkflowDefine(workflowName);
			this.rules = rules;
			this.userIdentity = userIdentity;
			this.unitCode = userIdentity.GetUserUnitCode();
			this.userId = userIdentity.GetUserId();
		}

		/// <summary>
		/// Gets all task states.
		/// </summary>
		/// <param name="taskName">Name of the task.</param>
		/// <returns></returns>
		public virtual List<string> GetAllTaskStates(string taskName)
		{
			//获取某用户特定任务下对应的状态列表
			List<string> taskStateList = new List<string>();
			foreach (ApprovalState state in workflow.States)
			{
				foreach (ApprovalEvent approvalEvent in state.Events)
				{
					foreach (EventRole eventRole in approvalEvent.Roles)
					{
						if (eventRole.Name == rules.UserRole.Name && eventRole.TaskName == taskName)
						{
							if (!taskStateList.Contains(state.Name))
								taskStateList.Add(state.Name);
						}
					}
				}
			}
			return taskStateList;
		}
		/// <summary>
		/// Gets all allowed cancel task states.
		/// </summary>
		/// <returns></returns>
		public virtual List<string> GetAllAllowedCancelTaskStates()
		{
			List<string> alloedCancelStates = new List<string>();
			//得到审批角色所有可撤销状态列表
			foreach (ApprovalState state in workflow.States)
			{
				foreach (ApprovalEvent approvalEvent in state.Events)
				{
					foreach (EventRole eventRole in approvalEvent.Roles)
					{
						if (eventRole.Name == rules.UserRole.Name)
						{
							foreach (string nextStateName in approvalEvent.NextStateNames)
							{
								if (workflow.GetStateByName(nextStateName).AllowedCancel)
								{
									if (!alloedCancelStates.Contains(nextStateName))
										alloedCancelStates.Add(nextStateName);
								}
							}
						}
					}
				}
			}
			return alloedCancelStates;
		}
		/// <summary>
		/// 从任务类表中除去已经指派给他人专办的项目
		/// </summary>
		/// <param name="instanceList">The instance list.</param>
		protected virtual void RemoveAssignedInstance(List<StateMachineWorkflowInstance> instanceList)
		{
			//去除已指定他人专办的实例
			for (int i = instanceList.Count - 1; i >= 0; i--)
			{
				StateMachineWorkflowInstance instance = instanceList[i];
				List<ApprovalAssignment> assignmentList = approvalService.GetAssignmentByAssignToRole(this.workflow.Name, rules.UserRole.Name, instance.Id, instance.StateName);

				//是否已指定同角色中他人专办标识
				bool isAssignedToCurrentUser = true;
				if (assignmentList.Count > 0)
				{
					foreach (ApprovalAssignment assignment in assignmentList)
					{
						if (assignment.ToUserId != null)
						{
							if (assignment.ToUserId.ToLower() == userIdentity.GetUserId().ToLower())
							{
								isAssignedToCurrentUser = true;
								break;
							}
							//如果遍历所有委派列表没有当前用互的记录,则该任务未指定当前用户
							else
								isAssignedToCurrentUser = false;
						}
					}
				}
				//无指定记录则某认指定了所有人
				else
					isAssignedToCurrentUser = true;
				//已指定专办标识为假，从List中除去该实例
				if (!isAssignedToCurrentUser)
					instanceList.Remove(instance);
			}
		}

		/// <summary>
		/// Gets the sub unit role list.
		/// </summary>
		/// <param name="taskStateList">The task state list.</param>
		/// <param name="taskName">Name of the task.</param>
		/// <returns></returns>
		protected abstract List<StateMachineWorkflowInstance> GetSubUnitRoleList(List<string> taskStateList, string taskName);
		/// <summary>
		/// Gets the approval role list.
		/// </summary>
		/// <param name="taskStateList">The task state list.</param>
		/// <param name="taskName">任务名称</param>
		/// <returns></returns>
		protected virtual List<StateMachineWorkflowInstance> GetApprovalRoleList(List<string> taskStateList, string taskName)
		{
			List<StateMachineWorkflowInstance> instanceList = new List<StateMachineWorkflowInstance>();
			instanceList.AddRange(GetList(taskStateList.ToArray()));
			return instanceList;
		}

		#region IApprovalProcess Members

		/// <summary>
		/// 获取当前用户的所有可以执行撤销的工作流实例
		/// </summary>
		/// <returns></returns>
		public virtual List<StateMachineWorkflowInstance> GetCancelAllowedTaskList()
		{
			string[] roleAllowedCancelStateList = GetAllAllowedCancelTaskStates().ToArray();
			List<StateMachineWorkflowInstance> instanceList = new List<StateMachineWorkflowInstance>();
			if (rules.IsSubUnitRole())
			{
				if (rules.IsCreator())
					instanceList.AddRange(GetOwnerList(roleAllowedCancelStateList));
				else
					instanceList.AddRange(GetUnitList(roleAllowedCancelStateList));
			}
			else
			{
				instanceList.AddRange(GetList(roleAllowedCancelStateList));
			}
			RemoveCannotCancelInstances(instanceList);
			return instanceList;
		}

		/// <summary>
		/// 移除当前用户不具有撤销权限的实例
		/// </summary>
		/// <param name="instanceList">任务列表.</param>
		protected virtual void RemoveCannotCancelInstances(List<StateMachineWorkflowInstance> instanceList)
		{
			int i = 0;
			int count = instanceList.Count;
			while (i < count)
			{
				if (!WorkflowRuntime.Current.GetService<IWorkflowSecurityService>().CanCancel(instanceList[i]))
				{
					instanceList.Remove(instanceList[i]);
					count--;
				}
				else
					i++;
			}
		}
		/// <summary>
		/// 按照任务名称获取实例列表
		/// </summary>
		/// <param name="taskName">任务名称</param>
		/// <returns></returns>
		public virtual List<StateMachineWorkflowInstance> GetInstanceList(string taskName)
		{
			List<string> taskStateList = GetAllTaskStates(taskName);
			List<StateMachineWorkflowInstance> instanceList = null;
			//如不是审批用户（是二级单位用户）则按用户名和其所在单位获取任务列表
			if (rules.IsSubUnitRole())
				instanceList = GetSubUnitRoleList(taskStateList, taskName);
			else
				instanceList = GetApprovalRoleList(taskStateList, taskName);
			RemoveAssignedInstance(instanceList);
			return instanceList;
		}
		/// <summary>
		/// 获得某角色的任务列表分类名称列表
		/// </summary>
		/// <returns></returns>
		public List<string> GetTaskNameList()
		{
			List<string> taskList = new List<string>();
			foreach (ApprovalState state in workflow.States)
			{
				foreach (ApprovalEvent approvalEvent in state.Events)
				{
					foreach (EventRole eventRole in approvalEvent.Roles)
					{
						if (eventRole.Name == rules.UserRole.Name)
						{
							if (!string.IsNullOrEmpty(eventRole.TaskName) && !taskList.Contains(eventRole.TaskName))
							{
								taskList.Add(eventRole.TaskName);
							}
						}
					}
				}
			}
			taskList.Sort();
			return taskList;
		}

		#endregion

		/// <summary>
		/// Gets the owner list.
		/// </summary>
		/// <param name="stateName">Name of the state.</param>
		/// <returns></returns>
		public List<StateMachineWorkflowInstance> GetOwnerList(string[] stateNames)
		{
			return WorkflowRuntime.Current.GetOwnerList(workflow.Name, stateNames, userId, unitCode);
		}
		/// <summary>
		/// Gets the unit list.
		/// </summary>
		/// <param name="stateName">Name of the state.</param>
		/// <returns></returns>
		public List<StateMachineWorkflowInstance> GetUnitList(string[] stateNames)
		{
			return WorkflowRuntime.Current.GetUnitList(workflow.Name, stateNames, unitCode);
		}
		/// <summary>
		/// Gets the list.
		/// </summary>
		/// <param name="stateName">Name of the state.</param>
		/// <returns></returns>
		public List<StateMachineWorkflowInstance> GetList(string[] stateNames)
		{
			return WorkflowRuntime.Current.GetListByState(workflow.Name, stateNames);
		}
	}
}

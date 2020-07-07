using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;

namespace OilDigital.Workflows
{
	/// <summary>
	/// �����ȡ�����б�ĳ������
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
		/// �û����
		/// </summary>
		/// <value>The user identity.</value>
		protected IUserIdentity UserIdentity
		{
			get { return this.userIdentity; }
		}
		private string unitCode;
		/// <summary>
		/// ��λ����
		/// </summary>
		/// <value>The unit code.</value>
		protected string UnitCode
		{
			get { return unitCode; }
		}
		private string userId;
		/// <summary>
		/// �û���Id
		/// </summary>
		/// <value>The user id.</value>
		protected string UserId
		{
			get { return this.userId; }
		}
		/// <summary>
		/// ���ݹ��������ƴ���һ�����̶���
		/// </summary>
		/// <param name="workflowName">����������</param>
		/// <param name="rules">��������</param>
		/// <param name="userIdentity">�û�ʡ��</param>
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
			//��ȡĳ�û��ض������¶�Ӧ��״̬�б�
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
			//�õ�������ɫ���пɳ���״̬�б�
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
		/// ����������г�ȥ�Ѿ�ָ�ɸ�����ר�����Ŀ
		/// </summary>
		/// <param name="instanceList">The instance list.</param>
		protected virtual void RemoveAssignedInstance(List<StateMachineWorkflowInstance> instanceList)
		{
			//ȥ����ָ������ר���ʵ��
			for (int i = instanceList.Count - 1; i >= 0; i--)
			{
				StateMachineWorkflowInstance instance = instanceList[i];
				List<ApprovalAssignment> assignmentList = approvalService.GetAssignmentByAssignToRole(this.workflow.Name, rules.UserRole.Name, instance.Id, instance.StateName);

				//�Ƿ���ָ��ͬ��ɫ������ר���ʶ
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
							//�����������ί���б�û�е�ǰ�û��ļ�¼,�������δָ����ǰ�û�
							else
								isAssignedToCurrentUser = false;
						}
					}
				}
				//��ָ����¼��ĳ��ָ����������
				else
					isAssignedToCurrentUser = true;
				//��ָ��ר���ʶΪ�٣���List�г�ȥ��ʵ��
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
		/// <param name="taskName">��������</param>
		/// <returns></returns>
		protected virtual List<StateMachineWorkflowInstance> GetApprovalRoleList(List<string> taskStateList, string taskName)
		{
			List<StateMachineWorkflowInstance> instanceList = new List<StateMachineWorkflowInstance>();
			instanceList.AddRange(GetList(taskStateList.ToArray()));
			return instanceList;
		}

		#region IApprovalProcess Members

		/// <summary>
		/// ��ȡ��ǰ�û������п���ִ�г����Ĺ�����ʵ��
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
		/// �Ƴ���ǰ�û������г���Ȩ�޵�ʵ��
		/// </summary>
		/// <param name="instanceList">�����б�.</param>
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
		/// �����������ƻ�ȡʵ���б�
		/// </summary>
		/// <param name="taskName">��������</param>
		/// <returns></returns>
		public virtual List<StateMachineWorkflowInstance> GetInstanceList(string taskName)
		{
			List<string> taskStateList = GetAllTaskStates(taskName);
			List<StateMachineWorkflowInstance> instanceList = null;
			//�粻�������û����Ƕ�����λ�û������û����������ڵ�λ��ȡ�����б�
			if (rules.IsSubUnitRole())
				instanceList = GetSubUnitRoleList(taskStateList, taskName);
			else
				instanceList = GetApprovalRoleList(taskStateList, taskName);
			RemoveAssignedInstance(instanceList);
			return instanceList;
		}
		/// <summary>
		/// ���ĳ��ɫ�������б���������б�
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

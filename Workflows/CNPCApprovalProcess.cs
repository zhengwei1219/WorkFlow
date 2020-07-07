using System;
using System.Collections.Generic;
using System.Text;

namespace OilDigital.Workflows
{
	/// <summary>
	/// CNPC管理审批流程的类
	/// </summary>
	public class CNPCApprovalProcess : ApprovalProcess
	{
		/// <summary>
		/// 根据工作流名称创建一个过程对象
		/// </summary>
		/// <param name="workflowName">工作流名称</param>
		/// <param name="rules">审批规则</param>
		/// <param name="userIdentity">用户省份</param>
		public CNPCApprovalProcess(string workflowName, IApprovalRules rules, IUserIdentity userIdentity)
			: base(workflowName, rules, userIdentity)
		{ }

		/// <summary>
		/// Gets the sub unit role list.
		/// </summary>
		/// <param name="taskStateList">The task state list.</param>
		/// <param name="taskName">Name of the task.</param>
		/// <returns></returns>
		protected override List<StateMachineWorkflowInstance> GetSubUnitRoleList(List<string> taskStateList, string taskName)
		{
			List<StateMachineWorkflowInstance> instanceList = new List<StateMachineWorkflowInstance>();
			foreach (string stateName in taskStateList)
			{
				ApprovalEvent taskEvent = new ApprovalEvent();
				ApprovalState state = Workflow.GetStateByName(stateName);
				if (state != null)
				{
					foreach (ApprovalEvent approvalEvent in state.Events)
					{
						foreach (EventRole role in approvalEvent.Roles)
						{
							if (role.Name == Rules.UserRole.Name && taskName == role.TaskName)
							{
								taskEvent = approvalEvent;
								break;
							}
						}
					}
					//如果用户为二级单位专办员
					if (Rules.IsCreator())
					{
						//如果该任务只允许立项创建者办理
						if (taskEvent.Authorization != Authorization.DenyOwner.ToString())
							instanceList.AddRange(GetOwnerList(new string[] { stateName }));
					}
					else
					{
						instanceList.AddRange(GetUnitList(new string[] { stateName }));
					}
					//非用户创建，但是属于指定专办的实例，且实例为指定状态
					if (taskEvent.Authorization == Authorization.DenyOwner.ToString())
					{
						//其它二级单位专办员获得专办立项列表
						List<ApprovalAssignment> assignmentList = WorkflowRuntime.Current.GetService<IApprovalSaveService>().GetAssignmentByToUnit(this.Workflow.Name, UnitCode);
						foreach (ApprovalAssignment assignment in assignmentList)
						{
							StateMachineWorkflowInstance assignInstance = (StateMachineWorkflowInstance)WorkflowRuntime.Current.GetInstance(assignment.WorkflowInstanceId);
							if (assignInstance.StateName == stateName)
							{
								instanceList.Add(assignInstance);
							}
						}
					}
				}
			}
			return instanceList;
		}
	}
}

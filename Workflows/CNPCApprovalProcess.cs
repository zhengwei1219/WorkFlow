using System;
using System.Collections.Generic;
using System.Text;

namespace OilDigital.Workflows
{
	/// <summary>
	/// CNPC�����������̵���
	/// </summary>
	public class CNPCApprovalProcess : ApprovalProcess
	{
		/// <summary>
		/// ���ݹ��������ƴ���һ�����̶���
		/// </summary>
		/// <param name="workflowName">����������</param>
		/// <param name="rules">��������</param>
		/// <param name="userIdentity">�û�ʡ��</param>
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
					//����û�Ϊ������λר��Ա
					if (Rules.IsCreator())
					{
						//���������ֻ����������߰���
						if (taskEvent.Authorization != Authorization.DenyOwner.ToString())
							instanceList.AddRange(GetOwnerList(new string[] { stateName }));
					}
					else
					{
						instanceList.AddRange(GetUnitList(new string[] { stateName }));
					}
					//���û���������������ָ��ר���ʵ������ʵ��Ϊָ��״̬
					if (taskEvent.Authorization == Authorization.DenyOwner.ToString())
					{
						//����������λר��Ա���ר�������б�
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

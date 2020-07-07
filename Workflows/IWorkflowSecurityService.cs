using System;
using System.Collections.Generic;
using System.Text;

namespace OilDigital.Workflows
{
	/// <summary>
	/// ������ִ��Ȩ���ж�����ӿ�
	/// </summary>
	public interface IWorkflowSecurityService
	{
		/// <summary>
		/// �Ƿ��Ǹ��û�������
		/// </summary>
		/// <param name="instance">������ʵ��</param>
		/// <param name="userId">�û�Id</param>
		/// <returns>�Ƿ���True ���Ƿ���False</returns>
		bool IsMyTaskInstance(WorkflowInstance instance, string userId);
		/// <summary>
		/// �Ƿ��Ǹ��û��Ĵ�������
		/// </summary>
		/// <param name="instance">����ʵ��</param>
		/// <param name="userId">�û�Id</param>
		/// <returns>�Ƿ���True ���Ƿ���False</returns>
		bool IsMyAgentInstance(WorkflowInstance instance, string userId);
		/// <summary>
		/// ��ȡĳ�û���ĳʵ��ִ��ĳ����ʱ�Ľ�ɫ
		/// </summary>
		/// <param name="instance">������ʵ��</param>
		/// <param name="userId">�û�Id</param>
		/// <param name="actionName">ִ�ж���</param>
		/// <returns></returns>
		ApprovalRole GetActionRole(WorkflowInstance instance, string userId, string actionName);
		/// <summary>
		/// �ж�ĳ�û��Ƿ���ж�ָ��ʵ������ָ��������ִ��Ȩ��
		/// </summary>
		/// <param name="instance">������ʵ��</param>
		/// <param name="actionName">ִ�ж���</param>
		/// <returns>
		/// 	<c>true</c>�û���ִ�иö�������true ���򷵻�<c>false</c>.
		/// </returns>
		bool CanDoAction(WorkflowInstance instance, string actionName);
		/// <summary>
		/// �ж�ĳ�û��Ƿ��ܳ���ʵ������һ������
		/// </summary>
		/// <param name="instance">������ʵ��</param>
		/// <returns>
		/// 	<c>true</c> ����û���ִ�г��������򷵻�<c>false</c>.
		/// </returns>
		bool CanCancel(WorkflowInstance instance);
		/// <summary>
		/// �Ƿ���ʵ����������
		/// </summary>
		/// <param name="instance">ʵ��</param>
		/// <param name="userId">�û�Id</param>
		/// <returns>
		/// 	<c>true</c>�Ƿ���true���Ƿ���false<c>false</c>.
		/// </returns>
		bool IsOwner(WorkflowInstance instance, string userId);
		/// <summary>
		/// �Ƿ��ʵ�������д���Ȩ��
		/// </summary>
		/// <param name="userId">�û�Id</param>
		/// <param name="approvalEvent">������������</param>
		/// <param name="instance">ʵ��</param>
		/// <returns>
		/// 	<c>true</c> ����û���ִ��Ȩ�ޣ����򷵻� <c>false</c>.
		/// </returns>
		bool IsAuthorized(string userId, ApprovalEvent approvalEvent, WorkflowInstance instance);
		/// <summary>
		/// �Ƿ��ʵ�������д���Ȩ��
		/// </summary>
		/// <param name="userId">�û�Id</param>
		/// <param name="approvalEvent">������������</param>
		/// <param name="isOwner">�Ƿ���ʵ������</param>
		/// <param name="role">�û���ɫ</param>
		/// <returns><c>true</c> ����û���ִ��Ȩ�ޣ����򷵻�<c>false</c>.</returns>
		bool IsAuthorized(string userId, ApprovalEvent approvalEvent, bool isOwner, EventRole role);
	}
	/// <summary>
	/// ״̬��������ִ��Ȩ���ж�����
	/// </summary>
	public class WorkflowSecurityService : IWorkflowSecurityService
	{

		#region IWorkflowSecurityService Members
		/// <summary>
		/// �Ƿ��ǵ�ǰ�û�������
		/// </summary>
		/// <param name="instance">������ʵ��</param>
		/// <param name="userId">�û�Id</param>
		/// <returns>�Ƿ���True ���Ƿ���False</returns>
		public virtual bool IsMyTaskInstance(WorkflowInstance instance, string userId)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");
			if (string.IsNullOrEmpty(userId))
				throw new ArgumentNullException("userId");
			IApprovalSaveService service = WorkflowRuntime.Current.GetService<IApprovalSaveService>();
			ApprovalState currentState = ((StateMachineWorkflowInstance)instance).CurrentState;
			List<ApprovalAssignment> assignments = service.GetAssignmentByAssignState(instance.Id, currentState.Name);
			if (assignments.Count == 0)
			{
				foreach (ApprovalEvent approvalEvent in currentState.Events)
				{
					if (IsAuthorized(userId, approvalEvent, instance))
						return true;
				}
			}
			else
			{
				IUserIdentity userIdentity = WorkflowRuntime.Current.GetService<IIdentityService>().GetUserIdentity(userId);
				foreach (ApprovalAssignment assignment in assignments)
				{
					if (assignment.ToUserId == userId)
						return true;
					if (assignment.ToUserId == null && assignment.ToUnitCode.Equals(userIdentity.GetUserUnitCode(), StringComparison.OrdinalIgnoreCase))
						return true;
				}
			}
			return false;
		}
		/// <summary>
		/// �Ƿ��ǵ�ǰ�û��Ĵ�������
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="userId">The user id.</param>
		/// <returns></returns>
		public virtual bool IsMyAgentInstance(WorkflowInstance instance, string userId)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");
			if (string.IsNullOrEmpty(userId))
				throw new ArgumentNullException("userId");
			ApprovalState currentState = ((StateMachineWorkflowInstance)instance).CurrentState;
			IApprovalSaveService approvalService = WorkflowRuntime.Current.GetService<IApprovalSaveService>();
			List<ApprovalAgent> agentList = approvalService.GetValidAgentInfoByToUser(userId);
			if (agentList != null && agentList.Count > 0)
			{
				foreach (ApprovalAgent agentInfo in agentList)
				{
					foreach (ApprovalEvent approvalEvent in currentState.Events)
					{
						if (IsMyTaskInstance(instance, agentInfo.SetUserId))
							return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// �Ƿ���ʵ����������
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="userId">The user id.</param>
		/// <returns>
		/// 	<c>true</c> if the specified instance is owner; otherwise, <c>false</c>.
		/// </returns>
		public virtual bool IsOwner(WorkflowInstance instance, string userId)
		{
			IUserIdentity userIdentity = WorkflowRuntime.Current.GetService<IIdentityService>().GetUserIdentity(userId);
			return userIdentity.GetUserUnitCode().Trim() == instance.Properties["UnitCode"].Trim();
		}

		/// <summary>
		/// �Ƿ��ʵ�������д���Ȩ��
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <param name="approvalEvent">The approval event.</param>
		/// <param name="instance">The instance.</param>
		/// <returns>
		/// 	<c>true</c> if the specified user id is authorized; otherwise, <c>false</c>.
		/// </returns>
		public virtual bool IsAuthorized(string userId, ApprovalEvent approvalEvent, WorkflowInstance instance)
		{
			bool isOwner = IsOwner(instance, userId);
			foreach (EventRole role in approvalEvent.Roles)
			{
				if (IsAuthorized(userId, approvalEvent, isOwner, role))
					return true;
			}
			return false;
		}

		/// <summary>
		/// Determines whether the specified user id is authorized.
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <param name="approvalEvent">The approval event.</param>
		/// <param name="isOwner">if set to <c>true</c> [is owner].</param>
		/// <param name="role">The role.</param>
		/// <returns>
		/// 	<c>true</c> if the specified user id is authorized; otherwise, <c>false</c>.
		/// </returns>
		public virtual bool IsAuthorized(string userId, ApprovalEvent approvalEvent, bool isOwner, EventRole role)
		{
			IUserInRole userInRole = WorkflowRuntime.Current.GetService<IIdentityService>().GetUserInRole();
			return userInRole.IsUserInRole(userId, role.Name) && ((isOwner && approvalEvent.Authorization == Authorization.OwnerOnly.ToString()) || (!isOwner && approvalEvent.Authorization == Authorization.DenyOwner.ToString()) || approvalEvent.Authorization == Authorization.All.ToString());
		}

		/// <summary>
		/// ��ȡĳ�û���ĳʵ��ִ��ĳ����ʱ�Ľ�ɫ�����û���ִ�иö�����Ȩ�ޣ�����null
		/// </summary>
		/// <param name="instance">������ʵ��</param>
		/// <param name="userId">�û���Id</param>
		/// <param name="actionName">ִ�ж���</param>
		/// <returns></returns>
		public virtual ApprovalRole GetActionRole(WorkflowInstance instance, string userId, string actionName)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");
			if (string.IsNullOrEmpty(userId))
				throw new ArgumentNullException("userId");
			if (string.IsNullOrEmpty(actionName))
				throw new ArgumentNullException("actionName");
			bool isOwner = IsOwner(instance, userId);
			ApprovalEvent approvalEvent = ((StateMachineWorkflowInstance)instance).CurrentState.GetEventByName(actionName);
			foreach (EventRole role in approvalEvent.Roles)
			{
				if (IsAuthorized(userId, approvalEvent, isOwner, role))
					return WorkflowUtility.GetUserRoleByName(instance.Workflow, role.Name);
			}
			return null;
		}
		/// <summary>
		/// �ж�ĳ�û��Ƿ���ж�ָ��ʵ������ָ��������ִ��Ȩ��
		/// </summary>
		/// <param name="instance">������ʵ��</param>
		/// <param name="actionName">ִ�ж���</param>
		/// <returns></returns>
		public virtual bool CanDoAction(WorkflowInstance instance, string actionName)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");
			if (string.IsNullOrEmpty(actionName))
				throw new ArgumentNullException("actionName");
			IUserInRole userInRole = WorkflowRuntime.Current.GetService<IIdentityService>().GetUserInRole();
			IUserIdentity userIdentity = WorkflowRuntime.Current.GetService<IIdentityService>().GetUserIdentity();
			if (GetActionRole(instance, userIdentity.GetUserId(), actionName) != null)
				return true;
			ApprovalEvent approvalEvent = ((StateMachineWorkflowInstance)instance).CurrentState.GetEventByName(actionName);
			IApprovalSaveService approvalService = WorkflowRuntime.Current.GetService<IApprovalSaveService>();
			List<ApprovalAgent> agentList = approvalService.GetValidAgentInfoByToUser(userIdentity.GetUserId());
			if (agentList != null && agentList.Count > 0)
			{
				foreach (ApprovalAgent agentInfo in agentList)
				{
					foreach (EventRole role in approvalEvent.Roles)
					{
						if (IsAuthorized(agentInfo.SetUserId, approvalEvent, instance))
							return true;
					}
				}
			}
			return false;
		}
		/// <summary>
		/// �ж�ĳ�û��Ƿ��ܳ���ʵ������һ������
		/// </summary>
		/// <param name="instance">������ʵ��</param>
		/// <returns></returns>
		public virtual bool CanCancel(WorkflowInstance instance)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");
			IUserIdentity userIdentity = WorkflowRuntime.Current.GetService<IIdentityService>().GetUserIdentity();
			StateMachineWorkflowInstance stateMachine = (StateMachineWorkflowInstance)WorkflowRuntime.Current.GetInstance(instance.Id);
			if (stateMachine == null)
				return false;
			if (stateMachine.LastActivity == null)
				return false;

			//����ж�activity��ִ���û�,�����һ���͵�ǰƥ��,��ô������
			string[] users = stateMachine.LastActivity.UserId.Trim().Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			string currentUser=userIdentity.GetUserId().Trim();
			for (int i = 0; i < users.Length; i++)
			{
				if (users[i].Equals(currentUser, StringComparison.OrdinalIgnoreCase))
					return true;
			}

			return false;
		}

		#endregion
	}
}

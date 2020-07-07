using System;
using System.Collections.Generic;
using System.Text;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 工作流执行权限判定服务接口
	/// </summary>
	public interface IWorkflowSecurityService
	{
		/// <summary>
		/// 是否是该用户的任务
		/// </summary>
		/// <param name="instance">工作流实例</param>
		/// <param name="userId">用户Id</param>
		/// <returns>是返回True 不是返回False</returns>
		bool IsMyTaskInstance(WorkflowInstance instance, string userId);
		/// <summary>
		/// 是否是该用户的代理任务
		/// </summary>
		/// <param name="instance">流程实例</param>
		/// <param name="userId">用户Id</param>
		/// <returns>是返回True 不是返回False</returns>
		bool IsMyAgentInstance(WorkflowInstance instance, string userId);
		/// <summary>
		/// 获取某用户对某实例执行某动作时的角色
		/// </summary>
		/// <param name="instance">工作流实例</param>
		/// <param name="userId">用户Id</param>
		/// <param name="actionName">执行动作</param>
		/// <returns></returns>
		ApprovalRole GetActionRole(WorkflowInstance instance, string userId, string actionName);
		/// <summary>
		/// 判断某用户是否具有对指定实例进行指定动作的执行权限
		/// </summary>
		/// <param name="instance">工作流实例</param>
		/// <param name="actionName">执行动作</param>
		/// <returns>
		/// 	<c>true</c>用户可执行该动作返回true 否则返回<c>false</c>.
		/// </returns>
		bool CanDoAction(WorkflowInstance instance, string actionName);
		/// <summary>
		/// 判断某用户是否能撤销实例的上一步操作
		/// </summary>
		/// <param name="instance">工作流实例</param>
		/// <returns>
		/// 	<c>true</c> 如果用户可执行撤销，否则返回<c>false</c>.
		/// </returns>
		bool CanCancel(WorkflowInstance instance);
		/// <summary>
		/// 是否是实例的所有者
		/// </summary>
		/// <param name="instance">实例</param>
		/// <param name="userId">用户Id</param>
		/// <returns>
		/// 	<c>true</c>是返回true不是返回false<c>false</c>.
		/// </returns>
		bool IsOwner(WorkflowInstance instance, string userId);
		/// <summary>
		/// 是否对实例动作有触发权限
		/// </summary>
		/// <param name="userId">用户Id</param>
		/// <param name="approvalEvent">审批动作对象</param>
		/// <param name="instance">实例</param>
		/// <returns>
		/// 	<c>true</c> 如果用户有执行权限，否则返回 <c>false</c>.
		/// </returns>
		bool IsAuthorized(string userId, ApprovalEvent approvalEvent, WorkflowInstance instance);
		/// <summary>
		/// 是否对实例动作有触发权限
		/// </summary>
		/// <param name="userId">用户Id</param>
		/// <param name="approvalEvent">审批动作对象</param>
		/// <param name="isOwner">是否是实例所属</param>
		/// <param name="role">用户角色</param>
		/// <returns><c>true</c> 如果用户有执行权限，否则返回<c>false</c>.</returns>
		bool IsAuthorized(string userId, ApprovalEvent approvalEvent, bool isOwner, EventRole role);
	}
	/// <summary>
	/// 状态机工作流执行权限判定服务
	/// </summary>
	public class WorkflowSecurityService : IWorkflowSecurityService
	{

		#region IWorkflowSecurityService Members
		/// <summary>
		/// 是否是当前用户的任务
		/// </summary>
		/// <param name="instance">工作流实例</param>
		/// <param name="userId">用户Id</param>
		/// <returns>是返回True 不是返回False</returns>
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
		/// 是否是当前用户的代理任务
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
		/// 是否是实例的所有者
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
		/// 是否对实例动作有触发权限
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
		/// 获取某用户对某实例执行某动作时的角色，如用户无执行该动作的权限，返回null
		/// </summary>
		/// <param name="instance">工作流实例</param>
		/// <param name="userId">用户的Id</param>
		/// <param name="actionName">执行动作</param>
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
		/// 判断某用户是否具有对指定实例进行指定动作的执行权限
		/// </summary>
		/// <param name="instance">工作流实例</param>
		/// <param name="actionName">执行动作</param>
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
		/// 判断某用户是否能撤销实例的上一步操作
		/// </summary>
		/// <param name="instance">工作流实例</param>
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

			//逐个判断activity的执行用户,如果有一个和当前匹配,那么允许撤销
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

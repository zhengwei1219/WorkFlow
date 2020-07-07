using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.IO;
using System.Xml.Serialization;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 工作流工具类,封装了一些常用的方法:1.和审批角色相关的;2.和定制意见相关的.
	/// </summary>
	public static class WorkflowUtility
	{
	
	
		#region 审批角色相关方法
		/// <summary>
		/// Gets all roles.
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <returns></returns>
		public static List<ApprovalRole> GetAllWorkflowRoles(string workflowName)
		{
			StateMachineWorkflow workflow = (StateMachineWorkflow)WorkflowRuntime.Current.GetService<IWorkFlowDefinePersistService>().GetWorkflowDefine(workflowName);
			return new List<ApprovalRole>(workflow.Roles);
		}
		/// <summary>
		/// Gets all roles.
		/// </summary>
		/// <returns></returns>
		public static string[] GetAllRoles(string applicationName)
		{
			string[] workflows = WorkflowRuntime.Current.DefineService.GetAllWorkflowDefineName(applicationName);
			List<string> roles = new List<string>();
			foreach (string workflowName in workflows)
			{
				List<ApprovalRole> allRoles = WorkflowUtility.GetAllWorkflowRoles(workflowName);
				foreach (ApprovalRole role in allRoles)
				{
					if (!roles.Contains(role.Name))
						roles.Add(role.Name);
				}
			}
			return roles.ToArray();
		}
		/// <summary>
		/// 获取指定用户Id的审批角色列表
		/// </summary>
		/// <param name="workflowName">工作流名称</param>
		/// <param name="userId">用户Id</param>
		/// <returns>用户所在的审批角色</returns>
		public static List<ApprovalRole> GetUserRoles(string workflowName, string userId)
		{
			List<ApprovalRole> rolesList = new List<ApprovalRole>();

			//如果用户id为空,那么返回空角色集合
			if (string.IsNullOrEmpty(userId)) return rolesList;

			IUserInRole userInRole = WorkflowRuntime.Current.GetService<IIdentityService>().GetUserInRole();
			StateMachineWorkflow workflow = (StateMachineWorkflow)WorkflowRuntime.Current.GetService<IWorkFlowDefinePersistService>().GetWorkflowDefine(workflowName);

			foreach (ApprovalRole approvalRole in workflow.Roles)
			{
				if (userInRole.IsUserInRole(userId, approvalRole.Name))
				{
					if (!rolesList.Contains(approvalRole))
						rolesList.Add(approvalRole);
				}
			}
			return rolesList;
		}
		/// <summary>
		/// 获取某角色名称的审批角色
		/// </summary>
		/// <param name="workflowName">工作流名称</param>
		/// <param name="roleName">角色名称</param>
		/// <returns></returns>
		public static ApprovalRole GetUserRoleByName(string workflowName, string roleName)
		{
			WorkFlowDefine workflow = WorkflowRuntime.Current.GetService<IWorkFlowDefinePersistService>().GetWorkflowDefine(workflowName);
			return GetUserRoleByName(workflow, roleName);
		}

		/// <summary>
		/// Gets the name of the user role by.
		/// </summary>
		/// <param name="define">The define.</param>
		/// <param name="roleName">Name of the role.</param>
		/// <returns></returns>
		public static ApprovalRole GetUserRoleByName(WorkFlowDefine define, string roleName)
		{
			StateMachineWorkflow workflow = define as StateMachineWorkflow;
			return workflow.GetRoleByName(roleName);
		}

		#endregion

		#region 审批用户定制审批意见的方法
		/// <summary>
		/// 保存用户定制意见
		/// </summary>
		/// <param name="commentInfo">定制意见内容</param>
		/// <param name="applicationName">审批应用的名称:如果要实现不同的应用下同一个审批类型可以定制不同的意见,那么需要传入非空字符串.</param>
		/// <param name="approvalType">审批操作类别</param>
		/// <param name="userId">The user id.</param>
		public static void InsertCommentInfo(string commentInfo, string applicationName, string approvalType, string userId)
		{
			IApprovalSaveService service = WorkflowRuntime.Current.GetService<IApprovalSaveService>();
			string type = string.Format("{0}{1}", applicationName, approvalType);
			if (!string.IsNullOrEmpty(commentInfo) && service.GetCommentInfo(commentInfo, userId, type).Count == 0)
			{
				//如该定制内容不存在，则添加该意见
				ApprovalComment comment = new ApprovalComment();
				comment.CommentInfo = commentInfo;
				comment.ApprovalType = type;
				comment.OwnerUserId = WorkflowRuntime.Current.GetService<IIdentityService>().GetUserIdentity().GetUserId();
				service.InsertCommentInfo(comment);
			}
		}
		/// <summary>
		/// 获取用户的定制意见信息
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <param name="applicationName">审批应用的名称:如果要实现不同的应用下同一个审批类型可以定制不同的意见,那么需要传入非空字符串.</param>
		/// <param name="approvalType">审批操作类别</param>
		/// <returns>定制意见和其Id的数组</returns>
		public static List<ApprovalComment> GetUserCommentInfo(string userId,string applicationName,string approvalType)
		{
			IApprovalSaveService service = WorkflowRuntime.Current.GetService<IApprovalSaveService>();
			return service.GetUserCommentInfo(userId, string.Format("{0}{1}",applicationName,approvalType));
		}

		/// <summary>
		/// 删除定制意见
		/// </summary>
		/// <param name="idString">定制意见Id</param>
		public static void DeleteCommentInfo(int id)
		{
			IApprovalSaveService service = WorkflowRuntime.Current.GetService<IApprovalSaveService>();
			service.DeleteCommentInfo(id);
		}
		#endregion
	}
}

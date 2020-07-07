using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.IO;
using System.Xml.Serialization;

namespace OilDigital.Workflows
{
	/// <summary>
	/// ������������,��װ��һЩ���õķ���:1.��������ɫ��ص�;2.�Ͷ��������ص�.
	/// </summary>
	public static class WorkflowUtility
	{
	
	
		#region ������ɫ��ط���
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
		/// ��ȡָ���û�Id��������ɫ�б�
		/// </summary>
		/// <param name="workflowName">����������</param>
		/// <param name="userId">�û�Id</param>
		/// <returns>�û����ڵ�������ɫ</returns>
		public static List<ApprovalRole> GetUserRoles(string workflowName, string userId)
		{
			List<ApprovalRole> rolesList = new List<ApprovalRole>();

			//����û�idΪ��,��ô���ؿս�ɫ����
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
		/// ��ȡĳ��ɫ���Ƶ�������ɫ
		/// </summary>
		/// <param name="workflowName">����������</param>
		/// <param name="roleName">��ɫ����</param>
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

		#region �����û�������������ķ���
		/// <summary>
		/// �����û��������
		/// </summary>
		/// <param name="commentInfo">�����������</param>
		/// <param name="applicationName">����Ӧ�õ�����:���Ҫʵ�ֲ�ͬ��Ӧ����ͬһ���������Ϳ��Զ��Ʋ�ͬ�����,��ô��Ҫ����ǿ��ַ���.</param>
		/// <param name="approvalType">�����������</param>
		/// <param name="userId">The user id.</param>
		public static void InsertCommentInfo(string commentInfo, string applicationName, string approvalType, string userId)
		{
			IApprovalSaveService service = WorkflowRuntime.Current.GetService<IApprovalSaveService>();
			string type = string.Format("{0}{1}", applicationName, approvalType);
			if (!string.IsNullOrEmpty(commentInfo) && service.GetCommentInfo(commentInfo, userId, type).Count == 0)
			{
				//��ö������ݲ����ڣ�����Ӹ����
				ApprovalComment comment = new ApprovalComment();
				comment.CommentInfo = commentInfo;
				comment.ApprovalType = type;
				comment.OwnerUserId = WorkflowRuntime.Current.GetService<IIdentityService>().GetUserIdentity().GetUserId();
				service.InsertCommentInfo(comment);
			}
		}
		/// <summary>
		/// ��ȡ�û��Ķ��������Ϣ
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <param name="applicationName">����Ӧ�õ�����:���Ҫʵ�ֲ�ͬ��Ӧ����ͬһ���������Ϳ��Զ��Ʋ�ͬ�����,��ô��Ҫ����ǿ��ַ���.</param>
		/// <param name="approvalType">�����������</param>
		/// <returns>�����������Id������</returns>
		public static List<ApprovalComment> GetUserCommentInfo(string userId,string applicationName,string approvalType)
		{
			IApprovalSaveService service = WorkflowRuntime.Current.GetService<IApprovalSaveService>();
			return service.GetUserCommentInfo(userId, string.Format("{0}{1}",applicationName,approvalType));
		}

		/// <summary>
		/// ɾ���������
		/// </summary>
		/// <param name="idString">�������Id</param>
		public static void DeleteCommentInfo(int id)
		{
			IApprovalSaveService service = WorkflowRuntime.Current.GetService<IApprovalSaveService>();
			service.DeleteCommentInfo(id);
		}
		#endregion
	}
}

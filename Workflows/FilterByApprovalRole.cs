using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;

namespace OilDigital.Workflows
{

	/// <summary>
	/// 按照级别过滤:删除不在一个级别的审批记录
	/// </summary>
	public class FilterByApprovalRole<T> : IFilter<T>
		where T : IOperatorRelative, new()
	{
		/// <summary>
		/// 用户的审批角色
		/// </summary>
		private ApprovalRole userRole;
		private string workflowName;
		private string userId;

		/// <summary>
		///  构造一个按照审批角色级别进行过滤的审批记录过滤器 <see cref="FilterByApprovalRole&lt;T&gt;"/> class.
		/// </summary>
		public FilterByApprovalRole()
		{
		}
		/// <summary>
		///设置环境参数
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="role">用户的审批角色</param>
		/// <param name="userId">以用户的Id</param>
		public void SetValues(string workflowName, ApprovalRole role, string userId)
		{
			if (role == null)
				throw new ArgumentNullException("role");
			this.userRole = role;
			this.workflowName = workflowName;
			this.userId = userId;
		}

		#region IFilter Members


		/// <summary>
		///记录是否被过滤掉:如果记录需要被过滤掉(去除),那么返回true,否则返回false
		/// </summary>
		/// <param name="record">The record.</param>
		/// <returns>
		/// 	<c>true</c> if the specified record is filtered; otherwise, <c>false</c>.
		/// </returns>
		public bool IsFiltered(T record)
		{
			//没有定义审批角色的不显示
			if (string.IsNullOrEmpty(record.OperatorRole))
			{
				return true;
			}

			//该角色名称对应的角色不正确的不显示
			ApprovalRole role = GetApprovalRoleByName(record.OperatorRole);
			if (role == null)
			{
				return true;
			}

			//有角色,如果级别不在一个级别,那么不显示,或者显示隐藏信息
			if (role.Group != userRole.Group)
			{
				return true;
			}

			return false;
		}

		//获取审批角色的service
		private ApprovalRole GetApprovalRoleByName(string name)
		{
			return WorkflowUtility.GetUserRoleByName(workflowName, name);
		}

		#endregion
	}

	/// <summary>
	/// 按角色过滤审批意见
	/// </summary>
	public class SolutionFilterByApprovalRole : FilterByApprovalRole<ApprovalSolution>
	{
	}

	/// <summary>
	/// 按角色过滤审批记录
	/// </summary>
	public class RecordFilterByApprovalRole : FilterByApprovalRole<ApprovalRecord>
	{
	}
}

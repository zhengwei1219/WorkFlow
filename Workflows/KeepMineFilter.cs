using System;
using System.Collections.Generic;
using System.Text;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 保留我自己(按照角色来比较)
	/// </summary>
	public class KeepMineFilter<T>:IFilter<T>
		where T:IOperatorRelative
	{
		/// <summary>
		/// 构造一个保留自己发出内容的过滤器
		/// </summary>
		public KeepMineFilter()
		{
		}

		private ApprovalRole approvalRole;

		/// <summary>
		///设置环境参数
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="role">用户的审批角色</param>
		/// <param name="userId">以用户的Id</param>
		public void SetValues(string workflowName, ApprovalRole role, string userId)
		{
			this.approvalRole = role;
		}
		#region IFilter<T> Members

		/// <summary>
		/// Determines whether the specified record is filtered.
		/// </summary>
		/// <param name="record">The record.</param>
		/// <returns>
		/// 	<c>true</c> if the specified record is filtered; otherwise, <c>false</c>.
		/// </returns>
		public bool IsFiltered(T record)
		{
			//保留未知操作者的记录
			if (string.IsNullOrEmpty(record.OperatorRole)) return false;

			return !record.OperatorRole.Trim().Equals(approvalRole.Name.Trim(), StringComparison.OrdinalIgnoreCase);
		}

		#endregion
	}

	/// <summary>
	/// 只保留自己提出的审批意见过滤器
	/// </summary>
	public class SolutionKeepMineFilter : KeepMineFilter<ApprovalSolution>
	{
	}

	/// <summary>
	/// 值保留自己的审批记录
	/// </summary>
	public class RecordKeepMineFilter : KeepMineFilter<ApprovalRecord>
	{
	}
	/// <summary>
	/// 保留给我的(按照角色来比较)
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class KeepToMeFilter<T> : IFilter<T>
		where T : IToUserRelative
	{

		private ApprovalRole approvalRole;
		/// <summary>
		/// 构造一个保留自己发出内容的过滤器
		/// </summary>
		public KeepToMeFilter() { }
		/// <summary>
		///设置环境参数
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="role">用户的审批角色</param>
		/// <param name="userId">以用户的Id</param>
		public void SetValues(string workflowName, ApprovalRole role, string userId)
		{
			this.approvalRole = role;
		}
		#region IFilter<T> Members

		/// <summary>
		/// Determines whether the specified record is filtered.
		/// </summary>
		/// <param name="record">The record.</param>
		/// <returns>
		/// 	<c>true</c> if the specified record is filtered; otherwise, <c>false</c>.
		/// </returns>
		public bool IsFiltered(T record)
		{
			//保留未知操作者的记录
			if (string.IsNullOrEmpty(record.ToUserRole)) return true;

			return !record.ToUserRole.Trim().Equals(approvalRole.Name.Trim(), StringComparison.OrdinalIgnoreCase);
		}

		#endregion
	}

	/// <summary>
	/// 只保留给我的审批意见
	/// </summary>
	public class SolutionKeepToMeFilter : KeepToMeFilter<ApprovalSolution>
	{
	}
}

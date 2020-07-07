using System;
using System.Collections.Generic;
using System.Text;

namespace OilDigital.Workflows
{
	/// <summary>
	/// �������Լ�(���ս�ɫ���Ƚ�)
	/// </summary>
	public class KeepMineFilter<T>:IFilter<T>
		where T:IOperatorRelative
	{
		/// <summary>
		/// ����һ�������Լ��������ݵĹ�����
		/// </summary>
		public KeepMineFilter()
		{
		}

		private ApprovalRole approvalRole;

		/// <summary>
		///���û�������
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="role">�û���������ɫ</param>
		/// <param name="userId">���û���Id</param>
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
			//����δ֪�����ߵļ�¼
			if (string.IsNullOrEmpty(record.OperatorRole)) return false;

			return !record.OperatorRole.Trim().Equals(approvalRole.Name.Trim(), StringComparison.OrdinalIgnoreCase);
		}

		#endregion
	}

	/// <summary>
	/// ֻ�����Լ�������������������
	/// </summary>
	public class SolutionKeepMineFilter : KeepMineFilter<ApprovalSolution>
	{
	}

	/// <summary>
	/// ֵ�����Լ���������¼
	/// </summary>
	public class RecordKeepMineFilter : KeepMineFilter<ApprovalRecord>
	{
	}
	/// <summary>
	/// �������ҵ�(���ս�ɫ���Ƚ�)
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class KeepToMeFilter<T> : IFilter<T>
		where T : IToUserRelative
	{

		private ApprovalRole approvalRole;
		/// <summary>
		/// ����һ�������Լ��������ݵĹ�����
		/// </summary>
		public KeepToMeFilter() { }
		/// <summary>
		///���û�������
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="role">�û���������ɫ</param>
		/// <param name="userId">���û���Id</param>
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
			//����δ֪�����ߵļ�¼
			if (string.IsNullOrEmpty(record.ToUserRole)) return true;

			return !record.ToUserRole.Trim().Equals(approvalRole.Name.Trim(), StringComparison.OrdinalIgnoreCase);
		}

		#endregion
	}

	/// <summary>
	/// ֻ�������ҵ��������
	/// </summary>
	public class SolutionKeepToMeFilter : KeepToMeFilter<ApprovalSolution>
	{
	}
}

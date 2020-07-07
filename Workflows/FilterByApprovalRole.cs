using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;

namespace OilDigital.Workflows
{

	/// <summary>
	/// ���ռ������:ɾ������һ�������������¼
	/// </summary>
	public class FilterByApprovalRole<T> : IFilter<T>
		where T : IOperatorRelative, new()
	{
		/// <summary>
		/// �û���������ɫ
		/// </summary>
		private ApprovalRole userRole;
		private string workflowName;
		private string userId;

		/// <summary>
		///  ����һ������������ɫ������й��˵�������¼������ <see cref="FilterByApprovalRole&lt;T&gt;"/> class.
		/// </summary>
		public FilterByApprovalRole()
		{
		}
		/// <summary>
		///���û�������
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="role">�û���������ɫ</param>
		/// <param name="userId">���û���Id</param>
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
		///��¼�Ƿ񱻹��˵�:�����¼��Ҫ�����˵�(ȥ��),��ô����true,���򷵻�false
		/// </summary>
		/// <param name="record">The record.</param>
		/// <returns>
		/// 	<c>true</c> if the specified record is filtered; otherwise, <c>false</c>.
		/// </returns>
		public bool IsFiltered(T record)
		{
			//û�ж���������ɫ�Ĳ���ʾ
			if (string.IsNullOrEmpty(record.OperatorRole))
			{
				return true;
			}

			//�ý�ɫ���ƶ�Ӧ�Ľ�ɫ����ȷ�Ĳ���ʾ
			ApprovalRole role = GetApprovalRoleByName(record.OperatorRole);
			if (role == null)
			{
				return true;
			}

			//�н�ɫ,���������һ������,��ô����ʾ,������ʾ������Ϣ
			if (role.Group != userRole.Group)
			{
				return true;
			}

			return false;
		}

		//��ȡ������ɫ��service
		private ApprovalRole GetApprovalRoleByName(string name)
		{
			return WorkflowUtility.GetUserRoleByName(workflowName, name);
		}

		#endregion
	}

	/// <summary>
	/// ����ɫ�����������
	/// </summary>
	public class SolutionFilterByApprovalRole : FilterByApprovalRole<ApprovalSolution>
	{
	}

	/// <summary>
	/// ����ɫ����������¼
	/// </summary>
	public class RecordFilterByApprovalRole : FilterByApprovalRole<ApprovalRecord>
	{
	}
}

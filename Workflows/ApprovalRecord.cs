using System;
using System.Collections.Generic;
using System.Text;

namespace OilDigital.Workflows
{
	/// <summary>
	/// ������������¼
	/// </summary>
	public class ApprovalRecord : IWorkflowInstanceRelative, IEaRelative, IOperatorRelative, IComparable
	{
		private int id;

		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		/// <value>The id.</value>
		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		private Guid workflowInstanceId;
		/// <summary>
		/// �����Ĺ�����ʵ��Id
		/// </summary>
		public virtual Guid WorkflowInstanceId
		{
			get { return workflowInstanceId; }
			set { workflowInstanceId = value; }
		}

		private int eaid;

		/// <summary>
		/// �ù�����ʵ����ص��ļ�ID
		/// </summary>
		public virtual int EaId
		{
			get { return eaid; }
			set { eaid = value; }
		}


		private string approvalUserId;

		/// <summary>
		/// ִ�в������û�ID
		/// </summary>
		public virtual string OperatorId
		{
			get { return approvalUserId; }
			set { approvalUserId = value; }
		}

		private string approvalUserName;

		/// <summary>
		/// ִ�в������û�������
		/// </summary>
		public virtual string OperatorName
		{
			get { return approvalUserName; }
			set { approvalUserName = value; }
		}

		private string operatorRole;

		/// <summary>
		/// ִ���û����ݵ�������ɫ
		/// </summary>
		public virtual string OperatorRole
		{
			get { return operatorRole; }
			set { operatorRole = value; }
		}

		private string operatorUnitCode;

		/// <summary>
		/// ִ���û��ĵ�λ����
		/// </summary>
		public virtual string OperatorUnitCode
		{
			get { return operatorUnitCode; }
			set { operatorUnitCode = value; }
		}

		private DateTime approvalTime;

		/// <summary>
		/// ����ʱ��
		/// </summary>
		public virtual DateTime OperatorTime
		{
			get { return approvalTime; }
			set { approvalTime = value; }
		}

		private string approvalType;

		/// <summary>
		/// ��������
		/// </summary>
		public virtual string ApprovalType
		{
			get { return approvalType; }
			set { approvalType = value; }
		}

		private string stateName;
		/// <summary>
		/// ����֮ǰ״̬����
		/// </summary>
		public virtual string StateName
		{
			get { return stateName; }
			set { stateName = value; }
		}

		private string solutionInfo;
		/// <summary>
		/// �������
		/// </summary>
		public virtual string SolutionInfo
		{
			get { return solutionInfo; }
			set { solutionInfo = value; }
		}

		private bool isCanceled;
		/// <summary>
		/// �Ƿ��ѱ�ִ�г���
		/// </summary>
		public virtual bool IsCanceled
		{
			get { return isCanceled; }
			set { isCanceled = value; }
		}

		#region IComparable Members

		/// <summary>
		/// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>
		/// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings:
		/// Value
		/// Meaning
		/// Less than zero
		/// This instance is less than <paramref name="obj"/>.
		/// Zero
		/// This instance is equal to <paramref name="obj"/>.
		/// Greater than zero
		/// This instance is greater than <paramref name="obj"/>.
		/// </returns>
		/// <exception cref="T:System.ArgumentException">
		/// 	<paramref name="obj"/> is not the same type as this instance.
		/// </exception>
		public virtual int CompareTo(object obj)
		{
			ApprovalRecord record = obj as ApprovalRecord;
			if (record == null)
				return -1;
			return record.approvalTime.CompareTo(this.approvalTime);
		}

		#endregion
	}
}

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace OilDigital.Workflows
{
	/// <summary>
	/// ָ���û�ר����Ϣ��
	/// </summary>
	[Serializable]
	public class ApprovalAssignment : ICloneable
	{
		private int id;
		/// <summary>
		/// ʵ��Id
		/// </summary>
		[XmlAttribute()]
		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		private Guid workflowInstanceId;
		/// <summary>
		/// ����������ʵ��Id
		/// </summary>
		[XmlAttribute()]
		public virtual Guid WorkflowInstanceId
		{
			get { return workflowInstanceId; }
			set { workflowInstanceId = value; }
		}

		private int eaId;
		/// <summary>
		/// ����Id
		/// </summary>
		[XmlAttribute()]
		public virtual int EaId
		{
			get { return eaId; }
			set { eaId = value; }
		}

		private string assigningRole;
		/// <summary>
		/// ����������������ɫ��
		/// </summary>
		[XmlAttribute()]
		public virtual string FromRole
		{
			get { return assigningRole; }
			set { assigningRole = value; }
		}

		private string assigningUserId;
		/// <summary>
		/// �������û�Id
		/// </summary>
		[XmlAttribute()]
		public virtual string FromUserId
		{
			get { return assigningUserId; }
			set { assigningUserId = value; }
		}

		private string assignToRole;
		/// <summary>
		/// ָ����ɫ
		/// </summary>
		[XmlAttribute()]
		public virtual string ToRole
		{
			get { return assignToRole; }
			set { assignToRole = value; }
		}

		private string assignState;
		/// <summary>
		/// ָ��ר����������״̬
		/// </summary>
		[XmlAttribute()]
		public virtual string AssignState
		{
			get { return assignState; }
			set { assignState = value; }
		}

		private string assignToUserId;
		/// <summary>
		/// ָ��ר������Id
		/// </summary>
		[XmlAttribute()]
		public virtual string ToUserId
		{
			get { return assignToUserId; }
			set { assignToUserId = value; }
		}

		private string assignToUnitCode;
		/// <summary>
		/// ָ��ר����Ա�ĵ�λ����
		/// </summary>
		[XmlAttribute()]
		public virtual string ToUnitCode
		{
			get { return assignToUnitCode; }
			set { assignToUnitCode = value; }
		}

		private DateTime assignDate;
		/// <summary>
		/// ָ��ר��ʱ��
		/// </summary>
		[XmlAttribute()]
		public virtual DateTime AssignDate
		{
			get { return assignDate; }
			set { assignDate = value; }
		}

		/// <summary>
		/// �жϵ�ǰ��Assign�Ƿ��Ѿ�ִ�й���
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if this instance is finished; otherwise, <c>false</c>.
		/// </returns>
		public virtual bool IsFinished()
		{
			return this.ToUserId != null && this.ToUserId.Trim().EndsWith("_finished");
		}

		/// <summary>
		/// ����Ϊ�Ѿ�ִ�й�
		/// </summary>
		public virtual void SetToFinished()
		{
			if (IsFinished()) throw new ApplicationException("��Assign�Ѿ�ִ�����");
			this.ToUserId = string.Format("{0}_finished", this.ToUserId);
		}

		/// <summary>
		/// �����������
		/// </summary>
		public virtual void CancelFinished()
		{
			if (!IsFinished()) throw new ApplicationException("��Assign��δִ�����");
			int pos = this.ToUserId.IndexOf("_finished");
			if (pos >= 0)
				ToUserId = ToUserId.Substring(0, pos);
		}

		/// <summary>
		/// ��ȡUserId,���Assign�Ѿ�finish,��ô����ToUser��������_finished��־,���ô˷������Է���������UserId
		/// </summary>
		/// <returns></returns>
		public virtual string GetToUserId()
		{
			if (this.ToUserId == null) return null;
			int pos = this.ToUserId.IndexOf("_finished");
			return  (pos >= 0)?ToUserId.Substring(0, pos):ToUserId;
		}
		#region ICloneable Members

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		/// A new object that is a copy of this instance.
		/// </returns>
		public virtual object Clone()
		{
			ApprovalAssignment assignment = new ApprovalAssignment();
			assignment.assignDate = this.assignDate;
			assignment.assigningRole = this.assigningRole;
			assignment.assigningUserId = this.assigningUserId;
			assignment.assignState = this.assignState;
			assignment.assignToRole = this.assignToRole;
			assignment.assignToUnitCode = this.assignToUnitCode;
			assignment.assignToUserId = this.assignToUserId;
			assignment.eaId = this.eaId;
			assignment.workflowInstanceId = this.workflowInstanceId;
			assignment.id = 0;
			return assignment;
		}

		#endregion
	}
}

using System;
using System.Data;
using System.Configuration;

namespace OilDigital.Workflows
{
	/// <summary>
	/// ���˶������������
	/// </summary>
	public class ApprovalComment
	{
		private int id;
		/// <summary>
		/// Ψһ��ʶ
		/// </summary>
		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}
	
		private string ownerId;
		/// <summary>
		/// ����������û�Id
		/// </summary>
		public virtual string OwnerUserId
		{
			get { return ownerId; }
			set { ownerId = value; }
		}

		private string approvalType;
		/// <summary>
		/// ����������
		/// </summary>
		public virtual string ApprovalType
		{
			get { return approvalType; }
			set { approvalType = value; }
		}

		private string commentInfo;
		/// <summary>
		/// �������������
		/// </summary>
		public virtual string CommentInfo
		{
			get { return commentInfo; }
			set { commentInfo = value; }
		}
	}
}

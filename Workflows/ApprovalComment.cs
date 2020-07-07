using System;
using System.Data;
using System.Configuration;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 个人定制审批意见类
	/// </summary>
	public class ApprovalComment
	{
		private int id;
		/// <summary>
		/// 唯一标识
		/// </summary>
		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}
	
		private string ownerId;
		/// <summary>
		/// 定制意见的用户Id
		/// </summary>
		public virtual string OwnerUserId
		{
			get { return ownerId; }
			set { ownerId = value; }
		}

		private string approvalType;
		/// <summary>
		/// 定制意见类别
		/// </summary>
		public virtual string ApprovalType
		{
			get { return approvalType; }
			set { approvalType = value; }
		}

		private string commentInfo;
		/// <summary>
		/// 定制意见的内容
		/// </summary>
		public virtual string CommentInfo
		{
			get { return commentInfo; }
			set { commentInfo = value; }
		}
	}
}

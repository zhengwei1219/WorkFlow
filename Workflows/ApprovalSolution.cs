using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 审批意见类
	/// </summary>
	[Serializable]
	public class ApprovalSolution : IOperatorRelative, IEaRelative, IToUserRelative,IComparable
	{
		private int id;
		/// <summary>
		/// 实例Id
		/// </summary>
		[XmlAttribute()]
		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		private Guid workflowInstanceId;
		/// <summary>
		/// 所属工作流实例Id
		/// </summary>
		[XmlAttribute()]
		public virtual Guid WorkflowInstanceId
		{
			get { return workflowInstanceId; }
			set { workflowInstanceId = value; }
		}


		private int eaId;
		/// <summary>
		/// 立项Id
		/// </summary>
		[XmlAttribute()]
		public virtual int EaId
		{
			get { return eaId; }
			set { eaId = value; }
		}

		private string signUserId;
		/// <summary>
		/// 意见人Id
		/// </summary>
		[XmlAttribute()]
		public virtual string OperatorId
		{
			get { return signUserId; }
			set { signUserId = value; }
		}

		private string signUserName;
		/// <summary>
		/// 意见人姓名
		/// </summary>
		[XmlAttribute()]
		public virtual string OperatorName
		{
			get { return signUserName; }
			set { signUserName = value; }
		}

		private string signUserUnitCode;
		/// <summary>
		/// 意见人所在单位代码
		/// </summary>
		public virtual string OperatorUnitCode
		{
			get { return signUserUnitCode; }
			set { signUserUnitCode = value; }
		}


		private string signRole;
		/// <summary>
		/// 意见人所属审批角色
		/// </summary>
		[XmlAttribute()]
		public virtual string OperatorRole
		{
			get { return signRole; }
			set { signRole = value; }
		}

		private DateTime approvalDate;
		/// <summary>
		/// 填写意见的时间
		/// </summary>
		[XmlAttribute()]
		public virtual DateTime OperatorTime
		{
			get { return approvalDate; }
			set { approvalDate = value; }
		}

		private string signTo;
		/// <summary>
		/// 意见对象所属的审批角色
		/// </summary>
		[XmlAttribute()]
		public virtual string ToUserRole
		{
			get { return signTo; }
			set { signTo = value; }
		}

		private string signToUserId;
		/// <summary>
		/// 意见对象用户Id
		/// </summary>
		[XmlAttribute()]
		public virtual string ToUserId
		{
			get { return signToUserId; }
			set { signToUserId = value; }
		}

		private string classify;
		/// <summary>
		/// 意见所属的审批操作类别
		/// </summary>
		[XmlAttribute()]
		public virtual string SolutionType
		{
			get { return classify; }
			set { classify = value; }
		}

		private string solutionInfo;
		/// <summary>
		/// 审批意见内容
		/// </summary>
		[XmlAttribute()]
		public virtual string SolutionInfo
		{
			get { return solutionInfo; }
			set { solutionInfo = value; }
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
			ApprovalSolution solution = obj as ApprovalSolution;
			if (solution == null)
				return -1;
			return solution.OperatorTime.CompareTo(this.OperatorTime);
		}

		#endregion
	}
}

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 指定用户专办信息类
	/// </summary>
	[Serializable]
	public class ApprovalAssignment : ICloneable
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

		private string assigningRole;
		/// <summary>
		/// 发起人所属审批角色名
		/// </summary>
		[XmlAttribute()]
		public virtual string FromRole
		{
			get { return assigningRole; }
			set { assigningRole = value; }
		}

		private string assigningUserId;
		/// <summary>
		/// 发起人用户Id
		/// </summary>
		[XmlAttribute()]
		public virtual string FromUserId
		{
			get { return assigningUserId; }
			set { assigningUserId = value; }
		}

		private string assignToRole;
		/// <summary>
		/// 指定角色
		/// </summary>
		[XmlAttribute()]
		public virtual string ToRole
		{
			get { return assignToRole; }
			set { assignToRole = value; }
		}

		private string assignState;
		/// <summary>
		/// 指定专办立项所属状态
		/// </summary>
		[XmlAttribute()]
		public virtual string AssignState
		{
			get { return assignState; }
			set { assignState = value; }
		}

		private string assignToUserId;
		/// <summary>
		/// 指定专办对象的Id
		/// </summary>
		[XmlAttribute()]
		public virtual string ToUserId
		{
			get { return assignToUserId; }
			set { assignToUserId = value; }
		}

		private string assignToUnitCode;
		/// <summary>
		/// 指定专办人员的单位代码
		/// </summary>
		[XmlAttribute()]
		public virtual string ToUnitCode
		{
			get { return assignToUnitCode; }
			set { assignToUnitCode = value; }
		}

		private DateTime assignDate;
		/// <summary>
		/// 指定专办时间
		/// </summary>
		[XmlAttribute()]
		public virtual DateTime AssignDate
		{
			get { return assignDate; }
			set { assignDate = value; }
		}

		/// <summary>
		/// 判断当前的Assign是否已经执行过了
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if this instance is finished; otherwise, <c>false</c>.
		/// </returns>
		public virtual bool IsFinished()
		{
			return this.ToUserId != null && this.ToUserId.Trim().EndsWith("_finished");
		}

		/// <summary>
		/// 设置为已经执行过
		/// </summary>
		public virtual void SetToFinished()
		{
			if (IsFinished()) throw new ApplicationException("本Assign已经执行完成");
			this.ToUserId = string.Format("{0}_finished", this.ToUserId);
		}

		/// <summary>
		/// 撤销办理完成
		/// </summary>
		public virtual void CancelFinished()
		{
			if (!IsFinished()) throw new ApplicationException("本Assign尚未执行完成");
			int pos = this.ToUserId.IndexOf("_finished");
			if (pos >= 0)
				ToUserId = ToUserId.Substring(0, pos);
		}

		/// <summary>
		/// 获取UserId,如果Assign已经finish,那么会在ToUser后面增加_finished标志,调用此方法可以返回真正的UserId
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

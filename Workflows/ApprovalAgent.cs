using System;
using System.Data;
using System.Configuration;
using System.Web;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 审批代理实体类,保存收取代理信息
	/// </summary>
	public class ApprovalAgent
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

		private string setUserId;
		/// <summary>
		/// 授权人Id
		/// </summary>
		public virtual string SetUserId
		{
			get { return setUserId; }
			set { setUserId = value; }
		}

		private string setUserName;
		/// <summary>
		/// 授权人姓名
		/// </summary>
		public virtual string SetUserName
		{
			get { return setUserName; }
			set { setUserName = value; }
		}

		private string toUserId;
		/// <summary>
		/// 被授权人Id
		/// </summary>
		public virtual string ToUserId
		{
			get { return toUserId; }
			set { toUserId = value; }
		}

		private string toUserName;
		/// <summary>
		/// 被授权人姓名
		/// </summary>
		public virtual string ToUserName
		{
			get { return toUserName; }
			set { toUserName = value; }
		}

		private DateTime fromDay;
		/// <summary>
		/// 授权时间段（起始时间）
		/// </summary>
		public virtual DateTime BeginDate
		{
			get { return fromDay; }
			set { fromDay = value; }
		}

		private DateTime endDate;
		/// <summary>
		/// 授权时间段（终了时间）
		/// </summary>
		public virtual DateTime EndDate
		{
			get { return endDate; }
			set { endDate = value; }
		}

		private DateTime setDate;
		/// <summary>
		/// 授权操作执行的时间
		/// </summary>
		public virtual DateTime SetDate
		{
			get { return setDate; }
			set { setDate = value; }
		}
		private string operatorUser;
		/// <summary>
		/// 执行授权的用户名
		/// </summary>
		/// <value></value>
		public virtual string OperatorUser
		{
			get { return operatorUser; }
			set { operatorUser = value; }
		}
		private string operatorUserName;
		/// <summary>
		/// 执行授权用户姓名
		/// </summary>
		/// <value></value>
		public virtual string OperatorUserName
		{
			get { return operatorUserName; }
			set { operatorUserName = value; }
		}

		private string additionInfo;
		/// <summary>
		/// 授权相关附加信息（如授权原因等）
		/// </summary>
		public virtual string AdditionInfo
		{
			get { return additionInfo; }
			set { additionInfo = value; }
		}
	}
}

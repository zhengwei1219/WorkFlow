using System;
using System.Data;
using System.Configuration;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 立项修改日志类
	/// </summary>
	public class EaAuditLog
	{
		private long id;
		/// <summary>
		/// 唯一标识
		/// </summary>
		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		private int eaId;
		/// <summary>
		/// 立项编号
		/// </summary>
		public virtual int EaId
		{
			get { return eaId; }
			set { eaId = value; }
		}

		private string actiontype;
		/// <summary>
		/// 操作类别
		/// </summary>
		public virtual string ActionType
		{
			get { return actiontype; }
			set { actiontype = value; }
		}

		private string className;
		/// <summary>
		/// 类名
		/// </summary>
		public virtual string ClassName
		{
			get { return className; }
			set { className = value; }
		}
	
		private string userId;
		/// <summary>
		/// 执行修改动作的用户Id
		/// </summary>
		public virtual string UserId
		{
			get { return userId; }
			set { userId = value; }
		}

		private string ip;
		/// <summary>
		/// 修改人IP地址
		/// </summary>
		public virtual string IP
		{
			get { return ip; }
			set { ip = value; }
		}
	

		private string propertyName;
		/// <summary>
		/// 修改的项目的名称
		/// </summary>
		public virtual string PropertyName
		{
			get { return propertyName; }
			set { propertyName = value; }
		}


		private string originalValue;
		/// <summary>
		/// 修改以前的值
		/// </summary>
		public virtual string OriginalValue
		{
			get { return originalValue; }
			set { originalValue = value; }
		}

		private string currentValue;
		/// <summary>
		/// 修改以后的值
		/// </summary>
		public virtual string CurrentValue
		{
			get { return currentValue; }
			set { currentValue = value; }
		}

		private DateTime updateTime;
		/// <summary>
		/// 修改时间
		/// </summary>
		public virtual DateTime UpdateTime
		{
			get { return updateTime; }
			set { updateTime = value; }
		}

		private long threadId;
		/// <summary>
		/// 线程Id
		/// </summary>
		public virtual long ThreadId
		{
			get { return threadId; }
			set { threadId = value; }
		}
	
	}
}

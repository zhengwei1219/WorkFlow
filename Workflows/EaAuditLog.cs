using System;
using System.Data;
using System.Configuration;

namespace OilDigital.Workflows
{
	/// <summary>
	/// �����޸���־��
	/// </summary>
	public class EaAuditLog
	{
		private long id;
		/// <summary>
		/// Ψһ��ʶ
		/// </summary>
		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		private int eaId;
		/// <summary>
		/// ������
		/// </summary>
		public virtual int EaId
		{
			get { return eaId; }
			set { eaId = value; }
		}

		private string actiontype;
		/// <summary>
		/// �������
		/// </summary>
		public virtual string ActionType
		{
			get { return actiontype; }
			set { actiontype = value; }
		}

		private string className;
		/// <summary>
		/// ����
		/// </summary>
		public virtual string ClassName
		{
			get { return className; }
			set { className = value; }
		}
	
		private string userId;
		/// <summary>
		/// ִ���޸Ķ������û�Id
		/// </summary>
		public virtual string UserId
		{
			get { return userId; }
			set { userId = value; }
		}

		private string ip;
		/// <summary>
		/// �޸���IP��ַ
		/// </summary>
		public virtual string IP
		{
			get { return ip; }
			set { ip = value; }
		}
	

		private string propertyName;
		/// <summary>
		/// �޸ĵ���Ŀ������
		/// </summary>
		public virtual string PropertyName
		{
			get { return propertyName; }
			set { propertyName = value; }
		}


		private string originalValue;
		/// <summary>
		/// �޸���ǰ��ֵ
		/// </summary>
		public virtual string OriginalValue
		{
			get { return originalValue; }
			set { originalValue = value; }
		}

		private string currentValue;
		/// <summary>
		/// �޸��Ժ��ֵ
		/// </summary>
		public virtual string CurrentValue
		{
			get { return currentValue; }
			set { currentValue = value; }
		}

		private DateTime updateTime;
		/// <summary>
		/// �޸�ʱ��
		/// </summary>
		public virtual DateTime UpdateTime
		{
			get { return updateTime; }
			set { updateTime = value; }
		}

		private long threadId;
		/// <summary>
		/// �߳�Id
		/// </summary>
		public virtual long ThreadId
		{
			get { return threadId; }
			set { threadId = value; }
		}
	
	}
}

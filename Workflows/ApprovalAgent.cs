using System;
using System.Data;
using System.Configuration;
using System.Web;

namespace OilDigital.Workflows
{
	/// <summary>
	/// ��������ʵ����,������ȡ������Ϣ
	/// </summary>
	public class ApprovalAgent
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

		private string setUserId;
		/// <summary>
		/// ��Ȩ��Id
		/// </summary>
		public virtual string SetUserId
		{
			get { return setUserId; }
			set { setUserId = value; }
		}

		private string setUserName;
		/// <summary>
		/// ��Ȩ������
		/// </summary>
		public virtual string SetUserName
		{
			get { return setUserName; }
			set { setUserName = value; }
		}

		private string toUserId;
		/// <summary>
		/// ����Ȩ��Id
		/// </summary>
		public virtual string ToUserId
		{
			get { return toUserId; }
			set { toUserId = value; }
		}

		private string toUserName;
		/// <summary>
		/// ����Ȩ������
		/// </summary>
		public virtual string ToUserName
		{
			get { return toUserName; }
			set { toUserName = value; }
		}

		private DateTime fromDay;
		/// <summary>
		/// ��Ȩʱ��Σ���ʼʱ�䣩
		/// </summary>
		public virtual DateTime BeginDate
		{
			get { return fromDay; }
			set { fromDay = value; }
		}

		private DateTime endDate;
		/// <summary>
		/// ��Ȩʱ��Σ�����ʱ�䣩
		/// </summary>
		public virtual DateTime EndDate
		{
			get { return endDate; }
			set { endDate = value; }
		}

		private DateTime setDate;
		/// <summary>
		/// ��Ȩ����ִ�е�ʱ��
		/// </summary>
		public virtual DateTime SetDate
		{
			get { return setDate; }
			set { setDate = value; }
		}
		private string operatorUser;
		/// <summary>
		/// ִ����Ȩ���û���
		/// </summary>
		/// <value></value>
		public virtual string OperatorUser
		{
			get { return operatorUser; }
			set { operatorUser = value; }
		}
		private string operatorUserName;
		/// <summary>
		/// ִ����Ȩ�û�����
		/// </summary>
		/// <value></value>
		public virtual string OperatorUserName
		{
			get { return operatorUserName; }
			set { operatorUserName = value; }
		}

		private string additionInfo;
		/// <summary>
		/// ��Ȩ��ظ�����Ϣ������Ȩԭ��ȣ�
		/// </summary>
		public virtual string AdditionInfo
		{
			get { return additionInfo; }
			set { additionInfo = value; }
		}
	}
}

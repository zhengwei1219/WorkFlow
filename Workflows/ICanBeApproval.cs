using System;
using System.Collections.Generic;
using System.Text;

namespace OilDigital.Workflows
{

	/// <summary>
	/// ����״̬����״̬��
	/// </summary>
	public class ApprovalStatusCode
	{
		/// <summary>
		/// ����״̬��û���ύ,��Ϊ�ݸ�״̬.
		/// </summary>
		public const string Normal = "0";

		/// <summary>
		/// �Ѿ��ύ����û�н��գ�
		/// </summary>
		public const string Submitted = "1";

		/// <summary>
		/// �Ѿ�����
		/// </summary>
		public const string Received = "2";

		/// <summary>
		/// ����ͨ��
		/// </summary>
		public const string Approved = "3";

		/// <summary>
		/// ������ֹ
		/// </summary>
		public const string Terminated = "4";

		/// <summary>
		/// ��������,�˻�״̬
		/// </summary>
		public const string Retrial = "5";
	}

	/// <summary>
	/// ���Ա��������̽��������Ľӿ�
	/// </summary>
	public interface ICanBeApproval
	{
		/// <summary>
		/// ��ȡ�����������Id
		/// </summary>
		/// <returns></returns>
		string GetID();

		/// <summary>
		/// ��ȡ����������Ĵ���״̬.
		/// </summary>
		/// <returns></returns>
		string GetStatus();

		/// <summary>
		/// ���û��߻�ý���ʱ��,����ʱ�����ڼ�¼�����������ʱ��"�ݸ�"�����ʽ.
		/// </summary>
		/// <value>The received time.</value>
		DateTime? ReceivedTime { get;set;}

		/// <summary>
		/// Gets or sets ����������ʱ��,��������������������(ͬ��),Ҳ�����Ǳ���ֹ��ʱ��
		/// </summary>
		/// <value>The approval end time.</value>
		DateTime? ApprovalEndTime { get;set;}

		/// <summary>
		/// ��������״̬,�������������״̬����ʹ<see cref="ApprovalStatusCode"/>�ж����״̬����,Ҳ�������Զ�����ַ��� 
		/// </summary>
		/// <param name="approvalStatusCode">����״̬,������</param>
		void SetStatus(string approvalStatusCode);
	}

	/// <summary>
	/// ����������ʱ��������ǰԭʼֵ�Ľӿ�.
	/// ��Щ������������Ҫ�������������������Ч��,�����Ҫ����������������������ǰ���������Ᵽ������.
	/// </summary>
	public interface IKeepOriginalValues
	{
		/// <summary>
		/// ����ԭʼֵ:���ؼ����ݵ�ԭʼ���ݱ���.
		/// </summary>
		void KeepOriginalValues();

		/// <summary>
		/// ���ԭʼֵ,��������ԭʼ���������.
		/// </summary>
		void ClearOriginalValues();
	}
}

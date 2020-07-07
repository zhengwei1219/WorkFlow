using System;
using System.Data;
using System.Configuration;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 
	/// </summary>
	public enum WorkflowExecutionStatus
	{
		/// <summary>
		/// ����������ִ��
		/// </summary>
		Executing,//  Represents the status when an activity is executing. 

		/// <summary>
		/// �ö������ڱ�ȡ��ִ��״̬
		/// </summary>
		Canceling,// Represents the status when an activity is in the process of being canceled. 

		/// <summary>
		/// ������ִ�����
		/// </summary>
		Closed,//  Represents the status when an activity is closed. 

		/// <summary>
		/// ��������ֹ
		/// </summary>
		Aborted,
		/// <summary>
		/// ������ִ�г��ֹ���
		/// </summary>
		Faulting//  Represents the status when an activity is faulting. 
	}
}

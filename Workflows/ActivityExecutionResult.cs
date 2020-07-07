using System;
using System.Collections.Generic;
using System.Text;

namespace OilDigital.Workflows
{

	/// <summary>
	/// ����������ִ�н��
	/// </summary>
	public enum ActivityExecutionResult
	{
		/// <summary>
		/// �˴���ִ�гɹ�
		/// </summary>
		Succeeded,// The activity has transitioned to the closed state from the executing state. 
		
		/// <summary>
		/// �˴ζ���û��ִ��
		/// </summary>
		Canceled, //The activity has transitioned to the closed state from the canceling state. 

		/// <summary>
		/// �˴ζ���ִ�г��ֹ���
		/// </summary>
		Faulted //The activity has transitioned to the closed state from the faulting state. 
	}


	/// <summary>
	/// �������ִ��������״̬
	/// </summary>
	public enum ActivityExecutionStatus
	{
		/// <summary>
		/// ����������ִ��
		/// </summary>
		Executing,//  Represents the status when an activity is executing. 

		/// <summary>
		/// �ö������ڱ�ȡ��ִ��״̬
		/// </summary>
		Canceling ,// Represents the status when an activity is in the process of being canceled. 
		
		/// <summary>
		/// ������ִ�����
		/// </summary>
		Closed,//  Represents the status when an activity is closed. 

		/// <summary>
		/// ������ִ�г��ֹ���
		/// </summary>
		Faulting//  Represents the status when an activity is faulting. 
	}

}

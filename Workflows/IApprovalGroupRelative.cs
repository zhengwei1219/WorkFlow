using System;
using System.Collections.Generic;
using System.Text;

namespace OilDigital.Workflows
{

	/// <summary>
	/// �͹�����ʵ����صĽӿڶ���
	/// </summary>
	public interface IWorkflowInstanceRelative
	{
		/// <summary>
		/// ������ʵ��guid
		/// </summary>
		Guid WorkflowInstanceId { get;set;}
	}

	/// <summary>
	/// ��������صĽӿڶ���
	/// </summary>
	public interface IEaRelative
	{
		/// <summary>
		/// ��������id
		/// </summary>
		int EaId { get;set;}
	}

	/// <summary>
	/// �Ͳ�������صĽӿ�
	/// </summary>
	public interface IOperatorRelative
	{
		/// <summary>
		/// ������id
		/// </summary>
		string OperatorId { get;set;}
		/// <summary>
		/// ����������
		/// </summary>
		string OperatorName { get;set;}
		/// <summary>
		/// ���������ڵ�λ����
		/// </summary>
		string OperatorUnitCode { get;set;}
		/// <summary>
		/// ����������������ɫ
		/// </summary>
		string OperatorRole { get;set;}
		/// <summary>
		/// ����ִ��ʱ��
		/// </summary>
		DateTime OperatorTime { get;set;}
	}

	/// <summary>
	/// ��ĳ���˴�����صĽӿ�
	/// </summary>
	public interface IToUserRelative
	{
		/// <summary>
		/// ��id
		/// </summary>
		string ToUserId { get;set;}
		/// <summary>
		/// ����������������ɫ
		/// </summary>
		string ToUserRole { get;set;}
	}
}

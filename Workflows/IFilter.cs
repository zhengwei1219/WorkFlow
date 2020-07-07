using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;


namespace OilDigital.Workflows
{
	/// <summary>
	/// һ�㻯�Ĺ��˽ӿ�
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IFilter<T>
	{
		/// <summary>
		/// ���ù�����������Ҫ�Ļ�������
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="role">�û���������ɫ</param>
		/// <param name="userId">���û���Id</param>
		void SetValues(string workflowName, ApprovalRole role, string userId);
		/// <summary>
		/// ��¼�Ƿ񱻹��˵�:�����¼��Ҫ�����˵�(ȥ��),��ô����true,���򷵻�false
		/// </summary>
		/// <param name="record">The record.</param>
		/// <returns>
		/// 	<c>true</c> if the specified record is filtered; otherwise, <c>false</c>.
		/// </returns>
		bool IsFiltered(T record);
	}


	//����������¼�Ľӿ�
	/// <summary>
	/// 
	/// </summary>
	public interface IApprovalRecordFilter:IFilter<ApprovalRecord>
	{
		
	}

	//������������Ľӿ�
	/// <summary>
	/// 
	/// </summary>
	public interface IApprovalSolutionFilter : IFilter<ApprovalSolution>
	{
		
	}
}

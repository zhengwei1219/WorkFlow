using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;


namespace OilDigital.Workflows
{
	/// <summary>
	/// 一般化的过滤接口
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IFilter<T>
	{
		/// <summary>
		/// 设置过滤器工作需要的环境变量
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="role">用户的审批角色</param>
		/// <param name="userId">以用户的Id</param>
		void SetValues(string workflowName, ApprovalRole role, string userId);
		/// <summary>
		/// 记录是否被过滤掉:如果记录需要被过滤掉(去除),那么返回true,否则返回false
		/// </summary>
		/// <param name="record">The record.</param>
		/// <returns>
		/// 	<c>true</c> if the specified record is filtered; otherwise, <c>false</c>.
		/// </returns>
		bool IsFiltered(T record);
	}


	//过滤审批记录的接口
	/// <summary>
	/// 
	/// </summary>
	public interface IApprovalRecordFilter:IFilter<ApprovalRecord>
	{
		
	}

	//过滤审批意见的接口
	/// <summary>
	/// 
	/// </summary>
	public interface IApprovalSolutionFilter : IFilter<ApprovalSolution>
	{
		
	}
}

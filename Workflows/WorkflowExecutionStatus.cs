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
		/// 工作流正在执行
		/// </summary>
		Executing,//  Represents the status when an activity is executing. 

		/// <summary>
		/// 该动作处于被取消执行状态
		/// </summary>
		Canceling,// Represents the status when an activity is in the process of being canceled. 

		/// <summary>
		/// 工作流执行完毕
		/// </summary>
		Closed,//  Represents the status when an activity is closed. 

		/// <summary>
		/// 工作流中止
		/// </summary>
		Aborted,
		/// <summary>
		/// 工作流执行出现故障
		/// </summary>
		Faulting//  Represents the status when an activity is faulting. 
	}
}

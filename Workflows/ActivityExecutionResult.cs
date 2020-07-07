using System;
using System.Collections.Generic;
using System.Text;

namespace OilDigital.Workflows
{

	/// <summary>
	/// 工作流动作执行结果
	/// </summary>
	public enum ActivityExecutionResult
	{
		/// <summary>
		/// 此次骤执行成功
		/// </summary>
		Succeeded,// The activity has transitioned to the closed state from the executing state. 
		
		/// <summary>
		/// 此次动作没有执行
		/// </summary>
		Canceled, //The activity has transitioned to the closed state from the canceling state. 

		/// <summary>
		/// 此次动作执行出现故障
		/// </summary>
		Faulted //The activity has transitioned to the closed state from the faulting state. 
	}


	/// <summary>
	/// 工作流活动执行所处的状态
	/// </summary>
	public enum ActivityExecutionStatus
	{
		/// <summary>
		/// 工作流正在执行
		/// </summary>
		Executing,//  Represents the status when an activity is executing. 

		/// <summary>
		/// 该动作处于被取消执行状态
		/// </summary>
		Canceling ,// Represents the status when an activity is in the process of being canceled. 
		
		/// <summary>
		/// 工作流执行完毕
		/// </summary>
		Closed,//  Represents the status when an activity is closed. 

		/// <summary>
		/// 工作流执行出现故障
		/// </summary>
		Faulting//  Represents the status when an activity is faulting. 
	}

}

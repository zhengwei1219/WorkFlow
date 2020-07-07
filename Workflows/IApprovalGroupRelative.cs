using System;
using System.Collections.Generic;
using System.Text;

namespace OilDigital.Workflows
{

	/// <summary>
	/// 和工作流实例相关的接口定义
	/// </summary>
	public interface IWorkflowInstanceRelative
	{
		/// <summary>
		/// 工作流实例guid
		/// </summary>
		Guid WorkflowInstanceId { get;set;}
	}

	/// <summary>
	/// 和立项相关的接口定义
	/// </summary>
	public interface IEaRelative
	{
		/// <summary>
		/// 相关立项的id
		/// </summary>
		int EaId { get;set;}
	}

	/// <summary>
	/// 和操作者相关的接口
	/// </summary>
	public interface IOperatorRelative
	{
		/// <summary>
		/// 操作者id
		/// </summary>
		string OperatorId { get;set;}
		/// <summary>
		/// 操作者姓名
		/// </summary>
		string OperatorName { get;set;}
		/// <summary>
		/// 操作者所在单位代码
		/// </summary>
		string OperatorUnitCode { get;set;}
		/// <summary>
		/// 操作者所属审批角色
		/// </summary>
		string OperatorRole { get;set;}
		/// <summary>
		/// 操作执行时间
		/// </summary>
		DateTime OperatorTime { get;set;}
	}

	/// <summary>
	/// 给某个人处理相关的接口
	/// </summary>
	public interface IToUserRelative
	{
		/// <summary>
		/// 给id
		/// </summary>
		string ToUserId { get;set;}
		/// <summary>
		/// 操作者所属审批角色
		/// </summary>
		string ToUserRole { get;set;}
	}
}

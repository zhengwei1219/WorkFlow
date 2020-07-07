using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 处理审批动作接口
	/// </summary>
	public interface ITaskActionProcessor
	{
		/// <summary>
		/// Initializes the specified service.
		/// </summary>
		/// <param name="service">The service.</param>
		void Initialize(ApprovalService service);
		/// <summary>
		/// Gets or sets the task render.
		/// </summary>
		/// <value>The task render.</value>
		ITableTaskRender ToolbarRender { get; set; }
		/// <summary>
		/// Gets or sets the item distiller.
		/// </summary>
		/// <value>The item distiller.</value>
		ITaskItemDistiller ToolbarItemDistiller { get; set; }
		/// <summary>
		/// Gets the task action item.
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <param name="instanceId">The instance id.</param>
		/// <returns></returns>
		string RenderToolbarTaskActionItems(string userId, Guid instanceId);
		/// <summary>
		/// Gets the toolbar action items.
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <param name="instanceId">The instance id.</param>
		/// <returns></returns>
		List<ITaskActionItem> GetToolbarTaskActionItems(string userId, Guid instanceId);
		/// <summary>
		/// 启动一个新的流程
		/// </summary>
		/// <param name="eaid">项目Id</param>
		/// <returns></returns>
		Guid InitWorkflow(int eaid);
		/// <summary>
		/// 校验审批对象的有效性
		/// <remarks>某些审批对象可能在执行审批动作前,可能需要进行有效期校验.</remarks>
		/// </summary>
		/// <param name="instanceId">流程Id</param>
		/// <param name="msg">校验返回的详细信息</param>
		/// <returns>如果true,表示校验通过,否则返回false,具体的错误信息可以从参数ms中获取</returns>
		bool CheckValid(string instanceId,out string msg);
	
		/// <summary>
		/// 校验审批对象的有效性
		/// <remarks>某些审批对象可能在执行审批动作前,可能需要进行有效期校验.</remarks>
		/// </summary>
		/// <param name="eaid">审批对象ID</param>
		/// <param name="msg">校验返回的详细信息</param>
		/// <returns>如果true,表示校验通过,否则返回false,具体的错误信息可以从参数ms中获取</returns>
		bool CheckValid(int eaid, out string msg);
		/// <summary>
		/// 处理提交动作执行逻辑
		/// </summary>
		/// <param name="instanceId">流程Id</param>
		/// <param name="eventPram">事件名称</param>
		/// <param name="userId">用户Id</param>
		void Submit(string instanceId, string eventPram, string userId);
		/// <summary>
		/// 处理上报动作执行逻辑
		/// </summary>
		/// <param name="instanceId">流程Id</param>
		/// <param name="eventPram">事件名称</param>
		/// <param name="userId">用户Id</param>
		void Upload(string instanceId, string eventPram, string userId);
		/// <summary>
		/// 处理接收动作执行逻辑
		/// </summary>
		/// <param name="instanceId">流程Id</param>
		/// <param name="eventPram">事件名称</param>
		/// <param name="userId">用户Id</param>
		void Receive(string instanceId, string eventPram, string userId);
		/// <summary>
		/// 处理终止动作执行逻辑
		/// </summary>
		/// <param name="instanceId">流程Id</param>
		/// <param name="eventPram">事件名称</param>
		/// <param name="userId">用户Id</param>
		void Terminate(string instanceId, string eventPram, string userId);
		/// <summary>
		/// 删除流程和项目逻辑
		/// </summary>
		/// <param name="instanceId">流程Id</param>
		void Delete(string instanceId);
		/// <summary>
		/// 处理结束流程动作执行逻辑
		/// </summary>
		/// <param name="instanceId">流程Id</param>
		/// <param name="eventPram">事件名称</param>
		/// <param name="userId">用户Id</param>
		void Complete(string instanceId, string eventPram, string userId);
		/// <summary>
		/// 处理结束审批动作执行逻辑
		/// </summary>
		/// <param name="instanceId">流程Id</param>
		/// <param name="eventPram">事件名称</param>
		/// <param name="userId">用户Id</param>
		void Finish(string instanceId, string eventPram, string userId);
		/// <summary>
		/// 处理动作执行逻辑
		/// </summary>
		/// <param name="instanceId">流程Id</param>
		/// <param name="eventPram">事件名称</param>
		/// <param name="userId">用户Id</param>
		void DoAction(string instanceId, string eventPram, string userId);
		/// <summary>
		/// 处理提交动作执行逻辑
		/// </summary>
		/// <param name="instanceId">流程Id</param>
		/// <param name="userId">用户Id</param>
		void DoCancel(string instanceId, string userId);
		/// <summary>
		/// 获取最后审批动作名称
		/// </summary>
		/// <param name="instanceId">实例Id</param>
		/// <returns></returns>
		string GetLastActionName(string instanceId);
	}

	/// <summary>
	/// 
	/// </summary>
	public interface ITaskApprovalService
	{
		/// <summary>
		/// Initializes the specified service.
		/// </summary>
		/// <param name="service">The service.</param>
		void Initialize(ApprovalService service);
		
		/// <summary>
		/// 获取某项目的审批意见信息
		/// </summary>
		/// <param name="eaid">The eaid.</param>
		/// <returns></returns>
		string GetSolution(int eaid);
		/// <summary>
		/// 获取某项目的审批意见信息
		/// </summary>
		/// <param name="eaid">The eaid.</param>
		/// <returns></returns>
		List<ApprovalSolution> GetSolutionInfo(int eaid);
		/// <summary>
		/// 获取某项目的审批记录信息
		/// </summary>
		/// <param name="eaid">The eaid.</param>
		/// <returns></returns>
		string GetApprovalRecord(int eaid);
		/// <summary>
		/// 获取某项目的审批记录信息
		/// </summary>
		/// <param name="eaid">The eaid.</param>
		/// <returns></returns>
		List<ApprovalRecord> GetRecordInfo(int eaid);
		/// <summary>
		/// 获取审批角色接口
		/// </summary>
		/// <param name="role">角色</param>
		/// <returns></returns>
		IApprovalRules GetApprovalRule(ApprovalRole role);

		
	}
}

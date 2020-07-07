using System;
using System.Collections.Generic;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 审批处理接口
	/// </summary>
	internal interface IApprovalProcess
	{
		/// <summary>
		/// 获取当前用户的所有可以执行撤销的工作流实例
		/// </summary>
		/// <returns></returns>
		List<StateMachineWorkflowInstance> GetCancelAllowedTaskList();
		/// <summary>
		/// 按照任务名称获取实例列表
		/// </summary>
		/// <param name="taskName">Name of the task.</param>
		/// <returns></returns>
		List<StateMachineWorkflowInstance> GetInstanceList(string taskName);

		/// <summary>
		/// 获取所有任务列表的名称
		/// </summary>
		/// <returns></returns>
		List<string> GetTaskNameList();
	}
	/// <summary>
	/// 表示审批事件触发权限的枚举
	/// </summary>
	public enum Authorization
	{
		/// <summary>
		/// All允许任何人
		/// </summary>
		All,
		/// <summary>
		/// OwnerOnly只有拥有者
		/// </summary>
		OwnerOnly,
		/// <summary>
		/// DenyOwner排除拥有者
		/// </summary>
		DenyOwner
	}
	/// <summary>
	/// 即时提醒接口，实现对处理人员和联系人的提醒消息的发送
	/// </summary>
	public abstract class ApprovalReminder
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ApprovalReminder"/> class.
		/// </summary>
		public ApprovalReminder() { }
		/// <summary>
		/// 发送提醒信息方法
		/// </summary>
		/// <param name="args">The <see cref="OilDigital.Workflows.WorkflowEventArgs"/> instance containing the event data.</param>
		/// <param name="remind">提醒类型</param>
		/// <param name="assignedUsers">下一步动作的执行者</param>
		/// <returns>发送成功消息的Id集合</returns>
		public abstract int[] SendMsg(WorkflowEventArgs args, MsgRemind remind, string[] assignedUsers);
		/// <summary>
		/// 删除提醒信息
		/// </summary>
		/// <param name="id">信息的Id</param>
		public abstract void DeleteMsg(int id);
	}
}

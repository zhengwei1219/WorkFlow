using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;

namespace OilDigital.Workflows
{

	/// <summary>
	/// 工作流存储服务接口
	/// </summary>
	public interface IApprovalSaveService
	{
		#region 批示意见存取和删除

		/// <summary>
		/// 插入一条审批意见信息
		/// </summary>
		/// <param name="solution">意见</param>
		/// <returns></returns>
		ApprovalSolution InsertSolution(ApprovalSolution solution);
		/// <summary>
		/// 获取某立项的审批信息
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		ApprovalSolution GetSolutionById(int id);
		/// <summary>
		/// 获取某立项的审批信息
		/// </summary>
		/// <param name="workflowName">流程名称</param>
		/// <param name="eaId">立项Id</param>
		/// <returns></returns>
		List<ApprovalSolution> GetSolution(string workflowName, int eaId);
		/// <summary>
		/// 获取某工作流的审批信息
		/// </summary>
		/// <param name="instanceId"></param>
		/// <returns></returns>
		List<ApprovalSolution> GetSolution(Guid instanceId);
		/// <summary>
		/// 删除审批意见信息
		/// </summary>
		/// <param name="id"></param>
		void DeleteSolutionById(int id);
		/// <summary>
		/// 按实例Id删除审批意见记录
		/// </summary>
		/// <param name="instanceId">The instance id.</param>
		void DeleteSolution(Guid instanceId);

		#endregion

		#region 审批记录存取
		/// <summary>
		/// 插入审批记录
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		ApprovalRecord InsertRecord(ApprovalRecord record);
		/// <summary>
		/// 保存审批记录
		/// </summary>
		/// <param name="record">审批记录.</param>
		/// <returns></returns>
		ApprovalRecord SaveRecord(ApprovalRecord record);
		/// <summary>
		/// 按Id删除审批记录
		/// </summary>
		/// <param name="id">The id.</param>
		void DeleteRecordById(int id);
		/// <summary>
		/// 按实例Id删除审批记录
		/// </summary>
		/// <param name="instanceId">The instance id.</param>
		void DeleteRecord(Guid instanceId);
		/// <summary>
		/// Gets the record.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		ApprovalRecord GetRecordById(int id);
		/// <summary>
		/// 按eaId获取审批记录
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="eaId">The ea id.</param>
		/// <returns></returns>
		List<ApprovalRecord> GetRecord(string workflowName, int eaId);
		/// <summary>
		/// 按InstanceId获取审批记录
		/// </summary>
		/// <param name="intanceId">The intance id.</param>
		/// <returns></returns>
		List<ApprovalRecord> GetRecord(Guid intanceId);
		/// <summary>
		/// 获取某用户某段时间段的审批记录
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="startDate">起始时间</param>
		/// <param name="endDate">终止时间</param>
		/// <param name="userId">用户Id</param>
		/// <returns></returns>
		List<ApprovalRecord> GetRecord(string workflowName, DateTime startDate, DateTime endDate, string userId);
		/// <summary>
		/// 获取审批记录
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="startDate">起始时间</param>
		/// <param name="endDate">截止时间</param>
		/// <param name="userId">用户UserId(为空忽略改条件)</param>
		/// <param name="unitCode">单位代码(为空忽略改条件)</param>
		/// <param name="roleName">角色名称(为空忽略改条件)</param>
		/// <returns></returns>
		List<ApprovalRecord> GetRecord(string workflowName, DateTime startDate, DateTime endDate, string userId, string unitCode, string roleName);
		
		#endregion

		#region 定制意见存取
		/// <summary>
		/// 获取用户在某个审批动作下的所有定制意见.
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <param name="approvalType">Type of the approval.</param>
		/// <returns></returns>
		List<ApprovalComment> GetUserCommentInfo(string userId, string approvalType);

		/// <summary>
		/// 查找相同的定制意见
		/// </summary>
		/// <param name="commentInfo">The comment info.</param>
		/// <param name="userId">The user id.</param>
		/// <param name="approvalType">Type of the approval.</param>
		/// <returns></returns>
		List<ApprovalComment> GetCommentInfo(string commentInfo, string userId, string approvalType);

		/// <summary>
		/// 删除定制意见
		/// </summary>
		/// <param name="commentId">定制意见Id</param>
		void DeleteCommentInfo(int commentId);

		/// <summary>
		/// 保存新增的定制意见
		/// </summary>
		/// <param name="comment">The comment.</param>
		/// <returns></returns>
		ApprovalComment InsertCommentInfo(ApprovalComment comment);
		#endregion

		#region 存取指定专办信息的方法

		/// <summary>
		/// 插入一条专办信息
		/// </summary>
		/// <param name="assignment">The assignment.</param>
		/// <returns></returns>
		ApprovalAssignment InsertAssignment(ApprovalAssignment assignment);
		/// <summary>
		/// 保存专办信息
		/// </summary>
		/// <param name="assignment">The assignment.</param>
		/// <returns></returns>
		ApprovalAssignment SaveAssignment(ApprovalAssignment assignment);
		/// <summary>
		/// 获取某用户的专办信息
		/// </summary>
		/// <param name="workflowName">工作流名称</param>
		/// <param name="assignToUserId">用户的登录Id</param>
		/// <returns></returns>
		List<ApprovalAssignment> GetAssignmentByUserId(string workflowName, string assignToUserId);
		/// <summary>
		/// 按被指定专办的角色和立项状态获取专办信息
		/// </summary>
		/// <param name="workflowName">工作流名称</param>
		/// <param name="assignToRole">别指定的角色</param>
		/// <param name="instanceId">实例Id</param>
		/// <param name="assignState">指定状态</param>
		/// <returns></returns>
		List<ApprovalAssignment> GetAssignmentByAssignToRole(string workflowName, string assignToRole, Guid instanceId, string assignState);

		/// <summary>
		/// 按被指定专办的角色和立项状态获取专办信息
		/// </summary>
		/// <param name="assignToRole">别指定的角色</param>
		/// <param name="instances">The instances.</param>
		/// <returns></returns>
		List<ApprovalAssignment> GetAssignmentByAssignToRole(string assignToRole, InstanceCollection instances);
		/// <summary>
		/// 获取某工作流实例在某状态下的指定专办信息
		/// </summary>
		/// <param name="instanceId">实例Id</param>
		/// <param name="assignState">指定状态</param>
		/// <returns></returns>
		List<ApprovalAssignment> GetAssignmentByAssignState(Guid instanceId, string assignState);
		/// <summary>
		/// 按指定专办单位获取指定专办信息
		/// </summary>
		/// <param name="workflowName">工作流名称</param>
		/// <param name="toUnitCode">专办单位的单位代码</param>
		/// <returns></returns>
		List<ApprovalAssignment> GetAssignmentByToUnit(string workflowName, string toUnitCode);
		/// <summary>
		/// 获取专办信息
		/// </summary>
		/// <param name="id">专办id</param>
		/// <returns></returns>
		ApprovalAssignment GetAssignmentById(int id);
		/// <summary>
		/// 按eaId获取指定专办信息
		/// </summary>
		/// <param name="workflowName">工作流名称</param>
		/// <param name="eaId">相关项目Id</param>
		/// <returns></returns>
		List<ApprovalAssignment> GetAssignment(string workflowName, int eaId);
		/// <summary>
		/// 按实例Id获取指定专办信息
		/// </summary>
		/// <param name="instanceId">The instance id.</param>
		/// <returns></returns>
		List<ApprovalAssignment> GetAssignment(Guid instanceId);

		/// <summary>
		/// 按实例Id数组获取指定专办信息
		/// </summary>
		/// <param name="instanceId">The instance id.</param>
		/// <returns></returns>
		List<ApprovalAssignment> GetAssignment(Guid[] instanceId);

		/// <summary>
		/// 删除专办信息
		/// </summary>
		/// <param name="id"></param>
		void DeleteAssignmentById(int id);
		/// <summary>
		/// 按实例Id删除专办信息
		/// </summary>
		/// <param name="instanceId">The instance id.</param>
		void DeleteAssignment(Guid instanceId);
		
		#endregion

		#region 委托代理信息存取

		/// <summary>
		/// 插入一条委托代理信息
		/// </summary>
		/// <param name="agentInfo"></param>
		/// <returns></returns>
		ApprovalAgent InsertAgent(ApprovalAgent agentInfo);
		/// <summary>
		/// 获取某用户当前有效委托代办信息
		/// </summary>
		/// <param name="toUserId">被委托人Id</param>
		/// <returns></returns>
		List<ApprovalAgent> GetValidAgentInfoByToUser(string toUserId);
		/// <summary>
		/// 获取某用户的委托代办信息
		/// </summary>
		/// <param name="toUserId">被委托人Id</param>
		/// <returns></returns>
		List<ApprovalAgent> GetAgentInfoByToUser(string toUserId);
		/// <summary>
		/// 按委托人Id获取委托代办名单
		/// </summary>
		/// <param name="setAgentUserId"></param>
		/// <returns></returns>
		List<ApprovalAgent> GetAgentInfoByFromUser(string setAgentUserId);
		/// <summary>
		/// 按Id删除委托代办信息
		/// </summary>
		/// <param name="id"></param>
		void DeleteAgentInfoById(int id);
		/// <summary>
		/// 按Id获取代理信息
		/// </summary>
		/// <param name="id">代理的Id</param>
		/// <returns></returns>
		ApprovalAgent GetAgentInfoById(int id);
		/// <summary>
		/// 保存代理信息
		/// </summary>
		/// <param name="agentInfo">代理信息</param>
		/// <returns></returns>
		ApprovalAgent UpdateAgentInfo(ApprovalAgent agentInfo);
		/// <summary>
		/// 获取所有授权代办信息
		/// </summary>
		/// <returns></returns>
		List<ApprovalAgent> GetAllAgentInfo();

		#endregion
	}
}

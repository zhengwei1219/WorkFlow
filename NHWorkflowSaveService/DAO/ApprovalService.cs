using System;
using System.Collections.Generic;
using System.Text;

namespace OilDigital.Workflows.DAO
{
	/// <summary>
	/// 基于Nhibernate的工作流存储服务
	/// </summary>
	public class NHApprovalService : IApprovalSaveService
	{

		#region 审批意见逻辑

		private ApprovalSolutionDao GetSolutionDao()
		{
			return new ApprovalSolutionDao();
		}
		/// <summary>
		/// 插入一条审批意见信息
		/// </summary>
		/// <param name="solution">意见</param>
		/// <returns></returns>
		public ApprovalSolution InsertSolution(ApprovalSolution solution)
		{
			return GetSolutionDao().SaveOrUpdate(solution);
		}
		/// <summary>
		/// Saves the solution.
		/// </summary>
		/// <param name="solution">The solution.</param>
		/// <returns></returns>
		public ApprovalSolution SaveSolution(ApprovalSolution solution)
		{
			return GetSolutionDao().SaveOrUpdate(solution);
		}
		/// <summary>
		/// Gets the solution.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		public ApprovalSolution GetSolutionById(int id)
		{
			return GetSolutionDao().GetById(id, false);
		}
		/// <summary>
		/// 获取某立项的审批信息
		/// </summary>
		/// <param name="workflowName">流程名称</param>
		/// <param name="eaId">立项Id</param>
		/// <returns></returns>
		public List<ApprovalSolution> GetSolution(string workflowName, int eaId)
		{
			return GetSolutionDao().GetSolution(workflowName, eaId);
		}

		/// <summary>
		/// 获取某工作流的审批信息
		/// </summary>
		/// <param name="instanceId"></param>
		/// <returns></returns>
		public List<ApprovalSolution> GetSolution(Guid instanceId)
		{
			return GetSolutionDao().GetSolution(instanceId);
		}
		/// <summary>
		/// 按实例Id删除审批意见记录
		/// </summary>
		/// <param name="instanceId">The instance id.</param>
		public void DeleteSolution(Guid instanceId)
		{
			ApprovalSolutionDao dao = GetSolutionDao();
			List<ApprovalSolution> solutions = dao.GetSolution(instanceId);
			for (int i = solutions.Count - 1; i >= 0; i--)
			{
				dao.Delete(solutions[i]);
			}
		}
		/// <summary>
		/// 删除审批意见信息
		/// </summary>
		/// <param name="id"></param>
		public void DeleteSolutionById(int id)
		{
			GetSolutionDao().DeletebyId(id);
		}

		#endregion

		#region 审批记录逻辑

		/// <summary>
		/// Gets the record DAO.
		/// </summary>
		/// <returns></returns>
		private ApprovalRecordDao GetRecordDao()
		{
			return new ApprovalRecordDao();
		}
		/// <summary>
		/// 插入审批记录
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		public ApprovalRecord InsertRecord(ApprovalRecord record)
		{
			return GetRecordDao().SaveOrUpdate(record);
		}
		/// <summary>
		/// Saves the record.
		/// </summary>
		/// <param name="record">The record.</param>
		/// <returns></returns>
		public ApprovalRecord SaveRecord(ApprovalRecord record)
		{
			return GetRecordDao().SaveOrUpdate(record);
		}
		/// <summary>
		/// Gets the record.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		public ApprovalRecord GetRecordById(int id)
		{
			return GetRecordDao().GetById(id, false);
		}
		/// <summary>
		/// 按eaId获取审批记录
		/// </summary>
		/// <param name="workflowName"></param>
		/// <param name="eaId"></param>
		/// <returns></returns>
		public List<ApprovalRecord> GetRecord(string workflowName, int eaId)
		{
			return GetRecordDao().GetRecord(workflowName, eaId);
		}
		/// <summary>
		/// 按实例Id获取审批记录
		/// </summary>
		/// <param name="instanceId">The instance id.</param>
		/// <returns></returns>
		public List<ApprovalRecord> GetRecord(Guid instanceId)
		{
			return GetRecordDao().GetRecord(instanceId);
		}
		/// <summary>
		/// 获取某用户某段时间段的审批记录
		/// </summary>
		/// <param name="workflowName"></param>
		/// <param name="startDate">起始时间</param>
		/// <param name="endDate">终止时间</param>
		/// <param name="userId">用户Id</param>
		/// <returns></returns>
		public List<ApprovalRecord> GetRecord(string workflowName, DateTime startDate, DateTime endDate, string userId)
		{
			return GetRecordDao().GetRecord(workflowName, startDate, endDate, userId);
		}
		/// <summary>
		/// 获取审批记录
		/// </summary>
		/// <param name="workflowName"></param>
		/// <param name="startDate">起始时间</param>
		/// <param name="endDate">截止时间</param>
		/// <param name="userId">用户UserId(为空忽略改条件)</param>
		/// <param name="unitCode">单位代码(为空忽略改条件)</param>
		/// <param name="roleName">角色名称(为空忽略改条件)</param>
		/// <returns></returns>
		public List<ApprovalRecord> GetRecord(string workflowName, DateTime startDate, DateTime endDate, string userId, string unitCode, string roleName)
		{
			return GetRecordDao().GetRecord(workflowName, startDate, endDate, userId, unitCode, roleName);
		}
		/// <summary>
		/// Deletes the approval rercord.
		/// </summary>
		/// <param name="id"></param>
		public void DeleteRecordById(int id)
		{
			GetRecordDao().DeletebyId(id);
		}
		/// <summary>
		/// 按实例Id删除审批记录
		/// </summary>
		/// <param name="instanceId">The instance id.</param>
		public void DeleteRecord(Guid instanceId)
		{
			ApprovalRecordDao dao = GetRecordDao();
			List<ApprovalRecord> records = dao.GetRecord(instanceId);
			for (int i = records.Count - 1; i >= 0; i--)
			{
				dao.Delete(records[i]);
			}
		}

		#endregion

		#region 定制意见存取
		private ApprovalCommentDao commentDao = new ApprovalCommentDao();
		/// <summary>
		/// 获取用户在某个审批动作下的所有定制意见.
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <param name="approvalType">Type of the approval.</param>
		/// <returns></returns>
		public List<ApprovalComment> GetUserCommentInfo(string userId, string approvalType)
		{
			return commentDao.GetUserCommentInfo(userId, approvalType);
		}

		/// <summary>
		/// 查找相同的定制意见
		/// </summary>
		/// <param name="commentInfo">The comment info.</param>
		/// <param name="userId">The user id.</param>
		/// <param name="approvalType">Type of the approval.</param>
		/// <returns></returns>
		public List<ApprovalComment> GetCommentInfo(string commentInfo, string userId, string approvalType)
		{
			return commentDao.GetCommentInfo(commentInfo, userId, approvalType);
		}

		/// <summary>
		/// 删除定制意见
		/// </summary>
		/// <param name="commentId">定制意见Id</param>
		public void DeleteCommentInfo(int commentId)
		{
			commentDao.DeletebyId(commentId);
		}

		/// <summary>
		/// 保存新增的定制意见
		/// </summary>
		/// <param name="comment">The comment.</param>
		/// <returns></returns>
		public ApprovalComment InsertCommentInfo(ApprovalComment comment)
		{
			return commentDao.Save(comment);
		}
		#endregion

		#region 指定专办逻辑

		private ApprovalAssignmentDao GetAssignmentDao()
		{
			return new ApprovalAssignmentDao();
		}
		/// <summary>
		/// 插入一条专办信息
		/// </summary>
		/// <param name="assignment"></param>
		/// <returns></returns>
		public ApprovalAssignment InsertAssignment(ApprovalAssignment assignment)
		{
			return GetAssignmentDao().SaveOrUpdate(assignment);
		}
		/// <summary>
		/// Saves the assignment.
		/// </summary>
		/// <param name="assignment">The assignment.</param>
		public ApprovalAssignment SaveAssignment(ApprovalAssignment assignment)
		{
			return GetAssignmentDao().SaveOrUpdate(assignment);
		}
		/// <summary>
		/// 获取某用户的专办信息
		/// </summary>
		/// <param name="workflowName">工作流名称</param>
		/// <param name="assignToUserId">用户的登录Id</param>
		/// <returns></returns>
		public List<ApprovalAssignment> GetAssignmentByUserId(string workflowName, string assignToUserId)
		{
			return GetAssignmentDao().GetAssignmentByUserId(workflowName, assignToUserId);
		}
		/// <summary>
		/// 按指定专办单位获取指定专办信息
		/// </summary>
		/// <param name="workflowName">工作流名称</param>
		/// <param name="toUnitCode">专办单位的单位代码</param>
		/// <returns></returns>
		public List<ApprovalAssignment> GetAssignmentByToUnit(string workflowName, string toUnitCode)
		{
			return GetAssignmentDao().GetAssignmentByToUnit(workflowName, toUnitCode);
		}
		/// <summary>
		/// 按被指定专办的角色和立项状态获取专办信息
		/// </summary>
		/// <param name="workflowName">工作流名称</param>
		/// <param name="assignToRole">别指定的角色</param>
		/// <param name="instanceId">实例Id</param>
		/// <param name="assignState">指定状态</param>
		/// <returns></returns>
		public List<ApprovalAssignment> GetAssignmentByAssignToRole(string workflowName, string assignToRole, Guid instanceId, string assignState)
		{
			return GetAssignmentDao().GetAssignmentByAssignedRole(workflowName, assignToRole, instanceId, assignState);
		}

		/// <summary>
		/// 按被指定专办的角色和立项状态获取专办信息
		/// </summary>
		/// <param name="assignToRole">别指定的角色</param>
		/// <param name="instances">The instances.</param>
		/// <returns></returns>
		public List<ApprovalAssignment> GetAssignmentByAssignToRole(string assignToRole, InstanceCollection instances)
		{
			return GetAssignmentDao().GetAssignmentByAssignedRole(assignToRole, instances); 
		}
		/// <summary>
		/// 获取某工作流实例在某状态下的指定专办信息
		/// </summary>
		/// <param name="instanceId">实例Id</param>
		/// <param name="assignState">指定状态</param>
		/// <returns></returns>
		public List<ApprovalAssignment> GetAssignmentByAssignState(Guid instanceId, string assignState)
		{
			return GetAssignmentDao().GetAssignmentByAssignState(instanceId, assignState);
		}
		/// <summary>
		/// 获取专办信息
		/// </summary>
		/// <param name="id">专办id</param>
		/// <returns></returns>
		public ApprovalAssignment GetAssignmentById(int id)
		{
			return GetAssignmentDao().GetById(id, false);
		}

		/// <summary>
		/// Gets the assignment.
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="eaid">The eaid.</param>
		/// <returns></returns>
		public List<ApprovalAssignment> GetAssignment(string workflowName, int eaid)
		{
			return GetAssignmentDao().GetAssignment(workflowName, eaid);
		}

		/// <summary>
		/// 按实例Id获取指定专办信息
		/// </summary>
		/// <param name="instanceId">The instance id.</param>
		/// <returns></returns>
		public List<ApprovalAssignment> GetAssignment(Guid instanceId)
		{
			return GetAssignmentDao().GetAssignment(instanceId);
		}

		/// <summary>
		/// 按实例id数组获取指定专办信息
		/// </summary>
		/// <param name="instanceId">The instance id.</param>
		/// <returns></returns>
		public List<ApprovalAssignment> GetAssignment(Guid[] instanceIds)
		{
			return GetAssignmentDao().GetAssignment(instanceIds);
		}

		/// <summary>
		/// 删除专办信息
		/// </summary>
		/// <param name="id"></param>
		public void DeleteAssignmentById(int id)
		{
			GetAssignmentDao().DeletebyId(id);
		}
		/// <summary>
		/// 按实例Id删除专办信息
		/// </summary>
		/// <param name="instanceId">The instance id.</param>
		public void DeleteAssignment(Guid instanceId)
		{
			ApprovalAssignmentDao dao = GetAssignmentDao();
			List<ApprovalAssignment> assignments = dao.GetAssignment(instanceId);
			for (int i = assignments.Count-1; i >=0; i--)
			{
				dao.Delete(assignments[i]);
			}
		}
		#endregion

		#region 委托代办逻辑

		/// <summary>
		/// Gets the agent DAO.
		/// </summary>
		/// <returns></returns>
		private ApprovalAgentDao GetAgentDao() 
		{
			return new ApprovalAgentDao();
		}
		/// <summary>
		/// 插入一条委托代理信息
		/// </summary>
		/// <param name="agentInfo"></param>
		/// <returns></returns>
		public ApprovalAgent InsertAgent(ApprovalAgent agentInfo)
		{
			return GetAgentDao().SaveOrUpdate(agentInfo);
		}

		/// <summary>
		/// 按Id获取代理信息
		/// </summary>
		/// <param name="id">代理的Id</param>
		/// <returns></returns>
		public ApprovalAgent GetAgentInfoById(int id)
		{
			return GetAgentDao().GetById(id, false);
		}

		/// <summary>
		/// 获取有效委托代办信息
		/// </summary>
		/// <param name="toUserId">被委托人Id</param>
		/// <returns></returns>
		public List<ApprovalAgent> GetValidAgentInfoByToUser(string toUserId)
		{
			return GetAgentDao().GetValidAgentInfoByToUser(toUserId);
		}

		/// <summary>
		/// 获取委托代办信息
		/// </summary>
		/// <param name="toUserId">被委托人Id</param>
		/// <returns></returns>
		public List<ApprovalAgent> GetAgentInfoByToUser(string toUserId)
		{
			return GetAgentDao().GetAgentInfoByToUser(toUserId);
		}
		/// <summary>
		/// 按委托人Id获取委托代办名单
		/// </summary>
		/// <param name="setAgentUserId"></param>
		/// <returns></returns>
		public List<ApprovalAgent> GetAgentInfoByFromUser(string setAgentUserId)
		{
			return GetAgentDao().GetApprovalAgentByUserId(setAgentUserId);
		}
		/// <summary>
		/// 保存代理信息
		/// </summary>
		/// <param name="agentInfo">代理信息</param>
		/// <returns></returns>
		public ApprovalAgent UpdateAgentInfo(ApprovalAgent agentInfo)
		{
			return GetAgentDao().SaveOrUpdate(agentInfo);
		}
		/// <summary>
		/// 按Id删除委托代办信息
		/// </summary>
		/// <param name="id"></param>
		public void DeleteAgentInfoById(int id)
		{
			GetAgentDao().DeletebyId(id);
		}

		/// <summary>
		/// 获取所有授权代办信息
		/// </summary>
		/// <returns></returns>
		public List<ApprovalAgent> GetAllAgentInfo()
		{
			return GetAgentDao().GetAllAgentInfo();
		}

		#endregion
	}
}

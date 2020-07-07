using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;

namespace OilDigital.Workflows
{

	/// <summary>
	/// �������洢����ӿ�
	/// </summary>
	public interface IApprovalSaveService
	{
		#region ��ʾ�����ȡ��ɾ��

		/// <summary>
		/// ����һ�����������Ϣ
		/// </summary>
		/// <param name="solution">���</param>
		/// <returns></returns>
		ApprovalSolution InsertSolution(ApprovalSolution solution);
		/// <summary>
		/// ��ȡĳ�����������Ϣ
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		ApprovalSolution GetSolutionById(int id);
		/// <summary>
		/// ��ȡĳ�����������Ϣ
		/// </summary>
		/// <param name="workflowName">��������</param>
		/// <param name="eaId">����Id</param>
		/// <returns></returns>
		List<ApprovalSolution> GetSolution(string workflowName, int eaId);
		/// <summary>
		/// ��ȡĳ��������������Ϣ
		/// </summary>
		/// <param name="instanceId"></param>
		/// <returns></returns>
		List<ApprovalSolution> GetSolution(Guid instanceId);
		/// <summary>
		/// ɾ�����������Ϣ
		/// </summary>
		/// <param name="id"></param>
		void DeleteSolutionById(int id);
		/// <summary>
		/// ��ʵ��Idɾ�����������¼
		/// </summary>
		/// <param name="instanceId">The instance id.</param>
		void DeleteSolution(Guid instanceId);

		#endregion

		#region ������¼��ȡ
		/// <summary>
		/// ����������¼
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		ApprovalRecord InsertRecord(ApprovalRecord record);
		/// <summary>
		/// ����������¼
		/// </summary>
		/// <param name="record">������¼.</param>
		/// <returns></returns>
		ApprovalRecord SaveRecord(ApprovalRecord record);
		/// <summary>
		/// ��Idɾ��������¼
		/// </summary>
		/// <param name="id">The id.</param>
		void DeleteRecordById(int id);
		/// <summary>
		/// ��ʵ��Idɾ��������¼
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
		/// ��eaId��ȡ������¼
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="eaId">The ea id.</param>
		/// <returns></returns>
		List<ApprovalRecord> GetRecord(string workflowName, int eaId);
		/// <summary>
		/// ��InstanceId��ȡ������¼
		/// </summary>
		/// <param name="intanceId">The intance id.</param>
		/// <returns></returns>
		List<ApprovalRecord> GetRecord(Guid intanceId);
		/// <summary>
		/// ��ȡĳ�û�ĳ��ʱ��ε�������¼
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="startDate">��ʼʱ��</param>
		/// <param name="endDate">��ֹʱ��</param>
		/// <param name="userId">�û�Id</param>
		/// <returns></returns>
		List<ApprovalRecord> GetRecord(string workflowName, DateTime startDate, DateTime endDate, string userId);
		/// <summary>
		/// ��ȡ������¼
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="startDate">��ʼʱ��</param>
		/// <param name="endDate">��ֹʱ��</param>
		/// <param name="userId">�û�UserId(Ϊ�պ��Ը�����)</param>
		/// <param name="unitCode">��λ����(Ϊ�պ��Ը�����)</param>
		/// <param name="roleName">��ɫ����(Ϊ�պ��Ը�����)</param>
		/// <returns></returns>
		List<ApprovalRecord> GetRecord(string workflowName, DateTime startDate, DateTime endDate, string userId, string unitCode, string roleName);
		
		#endregion

		#region ���������ȡ
		/// <summary>
		/// ��ȡ�û���ĳ�����������µ����ж������.
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <param name="approvalType">Type of the approval.</param>
		/// <returns></returns>
		List<ApprovalComment> GetUserCommentInfo(string userId, string approvalType);

		/// <summary>
		/// ������ͬ�Ķ������
		/// </summary>
		/// <param name="commentInfo">The comment info.</param>
		/// <param name="userId">The user id.</param>
		/// <param name="approvalType">Type of the approval.</param>
		/// <returns></returns>
		List<ApprovalComment> GetCommentInfo(string commentInfo, string userId, string approvalType);

		/// <summary>
		/// ɾ���������
		/// </summary>
		/// <param name="commentId">�������Id</param>
		void DeleteCommentInfo(int commentId);

		/// <summary>
		/// ���������Ķ������
		/// </summary>
		/// <param name="comment">The comment.</param>
		/// <returns></returns>
		ApprovalComment InsertCommentInfo(ApprovalComment comment);
		#endregion

		#region ��ȡָ��ר����Ϣ�ķ���

		/// <summary>
		/// ����һ��ר����Ϣ
		/// </summary>
		/// <param name="assignment">The assignment.</param>
		/// <returns></returns>
		ApprovalAssignment InsertAssignment(ApprovalAssignment assignment);
		/// <summary>
		/// ����ר����Ϣ
		/// </summary>
		/// <param name="assignment">The assignment.</param>
		/// <returns></returns>
		ApprovalAssignment SaveAssignment(ApprovalAssignment assignment);
		/// <summary>
		/// ��ȡĳ�û���ר����Ϣ
		/// </summary>
		/// <param name="workflowName">����������</param>
		/// <param name="assignToUserId">�û��ĵ�¼Id</param>
		/// <returns></returns>
		List<ApprovalAssignment> GetAssignmentByUserId(string workflowName, string assignToUserId);
		/// <summary>
		/// ����ָ��ר��Ľ�ɫ������״̬��ȡר����Ϣ
		/// </summary>
		/// <param name="workflowName">����������</param>
		/// <param name="assignToRole">��ָ���Ľ�ɫ</param>
		/// <param name="instanceId">ʵ��Id</param>
		/// <param name="assignState">ָ��״̬</param>
		/// <returns></returns>
		List<ApprovalAssignment> GetAssignmentByAssignToRole(string workflowName, string assignToRole, Guid instanceId, string assignState);

		/// <summary>
		/// ����ָ��ר��Ľ�ɫ������״̬��ȡר����Ϣ
		/// </summary>
		/// <param name="assignToRole">��ָ���Ľ�ɫ</param>
		/// <param name="instances">The instances.</param>
		/// <returns></returns>
		List<ApprovalAssignment> GetAssignmentByAssignToRole(string assignToRole, InstanceCollection instances);
		/// <summary>
		/// ��ȡĳ������ʵ����ĳ״̬�µ�ָ��ר����Ϣ
		/// </summary>
		/// <param name="instanceId">ʵ��Id</param>
		/// <param name="assignState">ָ��״̬</param>
		/// <returns></returns>
		List<ApprovalAssignment> GetAssignmentByAssignState(Guid instanceId, string assignState);
		/// <summary>
		/// ��ָ��ר�쵥λ��ȡָ��ר����Ϣ
		/// </summary>
		/// <param name="workflowName">����������</param>
		/// <param name="toUnitCode">ר�쵥λ�ĵ�λ����</param>
		/// <returns></returns>
		List<ApprovalAssignment> GetAssignmentByToUnit(string workflowName, string toUnitCode);
		/// <summary>
		/// ��ȡר����Ϣ
		/// </summary>
		/// <param name="id">ר��id</param>
		/// <returns></returns>
		ApprovalAssignment GetAssignmentById(int id);
		/// <summary>
		/// ��eaId��ȡָ��ר����Ϣ
		/// </summary>
		/// <param name="workflowName">����������</param>
		/// <param name="eaId">�����ĿId</param>
		/// <returns></returns>
		List<ApprovalAssignment> GetAssignment(string workflowName, int eaId);
		/// <summary>
		/// ��ʵ��Id��ȡָ��ר����Ϣ
		/// </summary>
		/// <param name="instanceId">The instance id.</param>
		/// <returns></returns>
		List<ApprovalAssignment> GetAssignment(Guid instanceId);

		/// <summary>
		/// ��ʵ��Id�����ȡָ��ר����Ϣ
		/// </summary>
		/// <param name="instanceId">The instance id.</param>
		/// <returns></returns>
		List<ApprovalAssignment> GetAssignment(Guid[] instanceId);

		/// <summary>
		/// ɾ��ר����Ϣ
		/// </summary>
		/// <param name="id"></param>
		void DeleteAssignmentById(int id);
		/// <summary>
		/// ��ʵ��Idɾ��ר����Ϣ
		/// </summary>
		/// <param name="instanceId">The instance id.</param>
		void DeleteAssignment(Guid instanceId);
		
		#endregion

		#region ί�д�����Ϣ��ȡ

		/// <summary>
		/// ����һ��ί�д�����Ϣ
		/// </summary>
		/// <param name="agentInfo"></param>
		/// <returns></returns>
		ApprovalAgent InsertAgent(ApprovalAgent agentInfo);
		/// <summary>
		/// ��ȡĳ�û���ǰ��Чί�д�����Ϣ
		/// </summary>
		/// <param name="toUserId">��ί����Id</param>
		/// <returns></returns>
		List<ApprovalAgent> GetValidAgentInfoByToUser(string toUserId);
		/// <summary>
		/// ��ȡĳ�û���ί�д�����Ϣ
		/// </summary>
		/// <param name="toUserId">��ί����Id</param>
		/// <returns></returns>
		List<ApprovalAgent> GetAgentInfoByToUser(string toUserId);
		/// <summary>
		/// ��ί����Id��ȡί�д�������
		/// </summary>
		/// <param name="setAgentUserId"></param>
		/// <returns></returns>
		List<ApprovalAgent> GetAgentInfoByFromUser(string setAgentUserId);
		/// <summary>
		/// ��Idɾ��ί�д�����Ϣ
		/// </summary>
		/// <param name="id"></param>
		void DeleteAgentInfoById(int id);
		/// <summary>
		/// ��Id��ȡ������Ϣ
		/// </summary>
		/// <param name="id">�����Id</param>
		/// <returns></returns>
		ApprovalAgent GetAgentInfoById(int id);
		/// <summary>
		/// ���������Ϣ
		/// </summary>
		/// <param name="agentInfo">������Ϣ</param>
		/// <returns></returns>
		ApprovalAgent UpdateAgentInfo(ApprovalAgent agentInfo);
		/// <summary>
		/// ��ȡ������Ȩ������Ϣ
		/// </summary>
		/// <returns></returns>
		List<ApprovalAgent> GetAllAgentInfo();

		#endregion
	}
}

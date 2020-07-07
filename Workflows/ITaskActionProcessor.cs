using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;

namespace OilDigital.Workflows
{
	/// <summary>
	/// �������������ӿ�
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
		/// ����һ���µ�����
		/// </summary>
		/// <param name="eaid">��ĿId</param>
		/// <returns></returns>
		Guid InitWorkflow(int eaid);
		/// <summary>
		/// У�������������Ч��
		/// <remarks>ĳЩ�������������ִ����������ǰ,������Ҫ������Ч��У��.</remarks>
		/// </summary>
		/// <param name="instanceId">����Id</param>
		/// <param name="msg">У�鷵�ص���ϸ��Ϣ</param>
		/// <returns>���true,��ʾУ��ͨ��,���򷵻�false,����Ĵ�����Ϣ���ԴӲ���ms�л�ȡ</returns>
		bool CheckValid(string instanceId,out string msg);
	
		/// <summary>
		/// У�������������Ч��
		/// <remarks>ĳЩ�������������ִ����������ǰ,������Ҫ������Ч��У��.</remarks>
		/// </summary>
		/// <param name="eaid">��������ID</param>
		/// <param name="msg">У�鷵�ص���ϸ��Ϣ</param>
		/// <returns>���true,��ʾУ��ͨ��,���򷵻�false,����Ĵ�����Ϣ���ԴӲ���ms�л�ȡ</returns>
		bool CheckValid(int eaid, out string msg);
		/// <summary>
		/// �����ύ����ִ���߼�
		/// </summary>
		/// <param name="instanceId">����Id</param>
		/// <param name="eventPram">�¼�����</param>
		/// <param name="userId">�û�Id</param>
		void Submit(string instanceId, string eventPram, string userId);
		/// <summary>
		/// �����ϱ�����ִ���߼�
		/// </summary>
		/// <param name="instanceId">����Id</param>
		/// <param name="eventPram">�¼�����</param>
		/// <param name="userId">�û�Id</param>
		void Upload(string instanceId, string eventPram, string userId);
		/// <summary>
		/// ������ն���ִ���߼�
		/// </summary>
		/// <param name="instanceId">����Id</param>
		/// <param name="eventPram">�¼�����</param>
		/// <param name="userId">�û�Id</param>
		void Receive(string instanceId, string eventPram, string userId);
		/// <summary>
		/// ������ֹ����ִ���߼�
		/// </summary>
		/// <param name="instanceId">����Id</param>
		/// <param name="eventPram">�¼�����</param>
		/// <param name="userId">�û�Id</param>
		void Terminate(string instanceId, string eventPram, string userId);
		/// <summary>
		/// ɾ�����̺���Ŀ�߼�
		/// </summary>
		/// <param name="instanceId">����Id</param>
		void Delete(string instanceId);
		/// <summary>
		/// ����������̶���ִ���߼�
		/// </summary>
		/// <param name="instanceId">����Id</param>
		/// <param name="eventPram">�¼�����</param>
		/// <param name="userId">�û�Id</param>
		void Complete(string instanceId, string eventPram, string userId);
		/// <summary>
		/// ���������������ִ���߼�
		/// </summary>
		/// <param name="instanceId">����Id</param>
		/// <param name="eventPram">�¼�����</param>
		/// <param name="userId">�û�Id</param>
		void Finish(string instanceId, string eventPram, string userId);
		/// <summary>
		/// ������ִ���߼�
		/// </summary>
		/// <param name="instanceId">����Id</param>
		/// <param name="eventPram">�¼�����</param>
		/// <param name="userId">�û�Id</param>
		void DoAction(string instanceId, string eventPram, string userId);
		/// <summary>
		/// �����ύ����ִ���߼�
		/// </summary>
		/// <param name="instanceId">����Id</param>
		/// <param name="userId">�û�Id</param>
		void DoCancel(string instanceId, string userId);
		/// <summary>
		/// ��ȡ���������������
		/// </summary>
		/// <param name="instanceId">ʵ��Id</param>
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
		/// ��ȡĳ��Ŀ�����������Ϣ
		/// </summary>
		/// <param name="eaid">The eaid.</param>
		/// <returns></returns>
		string GetSolution(int eaid);
		/// <summary>
		/// ��ȡĳ��Ŀ�����������Ϣ
		/// </summary>
		/// <param name="eaid">The eaid.</param>
		/// <returns></returns>
		List<ApprovalSolution> GetSolutionInfo(int eaid);
		/// <summary>
		/// ��ȡĳ��Ŀ��������¼��Ϣ
		/// </summary>
		/// <param name="eaid">The eaid.</param>
		/// <returns></returns>
		string GetApprovalRecord(int eaid);
		/// <summary>
		/// ��ȡĳ��Ŀ��������¼��Ϣ
		/// </summary>
		/// <param name="eaid">The eaid.</param>
		/// <returns></returns>
		List<ApprovalRecord> GetRecordInfo(int eaid);
		/// <summary>
		/// ��ȡ������ɫ�ӿ�
		/// </summary>
		/// <param name="role">��ɫ</param>
		/// <returns></returns>
		IApprovalRules GetApprovalRule(ApprovalRole role);

		
	}
}

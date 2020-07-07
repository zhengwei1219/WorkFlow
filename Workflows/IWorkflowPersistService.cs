using System;
using System.Collections.Generic;
using System.Text;

namespace OilDigital.Workflows
{
	/// <summary>
	/// ��������ȡ����ӿ�
	/// </summary>
	public interface IWorkflowPersistService
	{
		#region ������ʵ����ȡ����
		/// <summary>
		/// ���湤����ʵ��
		/// </summary>
		/// <param name="instance"></param>
		StateMachineWorkflowInstance SaveWorkflowInstance(StateMachineWorkflowInstance instance);
		/// <summary>
		/// ����һ���µĹ�����
		/// </summary>
		/// <param name="instance"></param>
		StateMachineWorkflowInstance InsertWorkflowInstance(StateMachineWorkflowInstance instance);
		/// <summary>
		/// ��ȡ����ĳ״̬�µ�����״̬��������ʵ��
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="stateName">״̬����</param>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		List<StateMachineWorkflowInstance> GetWorkflowInstance(string workflowName, string[] stateNames, Type type);
		/// <summary>
		/// �������ߵ�λ��Id����ʵ��״̬��ȡʵ��
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="stateName">ʵ��״̬����</param>
		/// <param name="createUserId">������Id</param>
		/// <param name="unitCode">�����ߵ�λ����</param>
		/// <returns></returns>
		List<StateMachineWorkflowInstance> GetWorkflowInstance(string workflowName, string[] stateNames, string createUserId, string unitCode);
		/// <summary>
		/// ��ȡĳ�û�������ʵ��
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="createUserId">�����û���Id</param>
		/// <returns></returns>
		List<StateMachineWorkflowInstance> GetWorkflowInstance(string workflowName, string createUserId);
		/// <summary>
		/// ��ʵ����״̬��ʵ�������ߵĵ�λ�����ȡʵ��
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="stateName">ʵ��״̬����</param>
		/// <param name="unitCode">������ʵ�����û���λ����</param>
		/// <returns></returns>
		List<StateMachineWorkflowInstance> GetWorkflowInstance(string workflowName, string[] stateNames, string unitCode);
		/// <summary>
		/// ����Id��ȡĳ״̬��������ʵ��
		/// </summary>
		/// <param name="id">ʵ��Id</param>
		/// <returns></returns>
		StateMachineWorkflowInstance GetWorkflowInstance(Guid id);
		/// <summary>
		/// ��ȡĳʵ��
		/// </summary>
		/// <param name="id">ʵ��Id</param>
		/// <param name="isAttachMore">�Ƿ�װ�ظ�����Ϣ</param>
		/// <returns></returns>
		StateMachineWorkflowInstance GetWorkflowInstance(Guid id, bool isAttachMore);
		/// <summary>
		/// ��ȡĳ�����Ӧ��ʵ��
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="eaId">����Id</param>
		/// <param name="isAttachMore">�Ƿ�װ�ظ�����Ϣ</param>
		/// <returns></returns>
		List<StateMachineWorkflowInstance> GetWorkflowInstance(string workflowName, int eaId, bool isAttachMore);
		/// <summary>
		/// ��ȡָ��������ʱ��κ�ָ��״̬�µĹ�����ʵ��
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="startDate">��ʼ����</param>
		/// <param name="endDate">��ֹ����</param>
		/// <param name="stateName">ʵ������״̬</param>
		/// <returns></returns>
		List<StateMachineWorkflowInstance> GetWorkflowInstance(string workflowName, DateTime startDate, DateTime endDate, string[] stateNames);
		/// <summary>
		/// ��ȡָ��������ʱ��κ�ָ��״̬�µĹ�����ʵ��
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="startDate">��ʼ����</param>
		/// <param name="endDate">��ֹ����</param>
		/// <param name="stateName">ʵ������״̬</param>
		/// <param name="unitCode">��λ����.</param>
		/// <returns></returns>
		List<StateMachineWorkflowInstance> GetWorkflowInstance(string workflowName, DateTime startDate, DateTime endDate, string[] stateNames, string unitCode);
		/// <summary>
		/// ɾ��ָ��������
		/// </summary>
		/// <param name="instance"></param>
		void DeleteWorkflowInstance(WorkflowInstance instance);
		
		#endregion
	}

	/// <summary>
	/// 
	/// </summary>
	public abstract class WorkflowPersistService : IWorkflowPersistService
	{
		#region IWorkflowPersistService Members
		/// <summary>
		/// ���湤����ʵ��
		/// </summary>
		/// <param name="instance"></param>
		protected abstract StateMachineWorkflowInstance SaveWorkflowInstance(StateMachineWorkflowInstance instance);
		/// <summary>
		/// ���湤����ʵ��
		/// </summary>
		/// <param name="instance"></param>
		StateMachineWorkflowInstance IWorkflowPersistService.SaveWorkflowInstance(StateMachineWorkflowInstance instance)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");
			return SaveWorkflowInstance(instance);
		}
		/// <summary>
		/// ����һ���µĹ�����
		/// </summary>
		/// <param name="instance"></param>
		protected abstract StateMachineWorkflowInstance InsertWorkflowInstance(StateMachineWorkflowInstance instance);
		/// <summary>
		/// ����һ���µĹ�����
		/// </summary>
		/// <param name="instance"></param>
		StateMachineWorkflowInstance IWorkflowPersistService.InsertWorkflowInstance(StateMachineWorkflowInstance instance)
		{
			return InsertWorkflowInstance(instance);
		}
		/// <summary>
		/// ��ȡ����ĳ״̬�µ�����״̬��������ʵ��
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="stateName">״̬����</param>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		protected abstract List<StateMachineWorkflowInstance> GetWorkflowInstance(string workflowName, string[] stateNames, Type type);
		/// <summary>
		/// ��ȡ����ĳ״̬�µ�����״̬��������ʵ��
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="stateName">״̬����</param>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		List<StateMachineWorkflowInstance> IWorkflowPersistService.GetWorkflowInstance(string workflowName, string[] stateNames, Type type)
		{
			if (string.IsNullOrEmpty(workflowName))
				throw new ArgumentNullException("workflowName");
			if (stateNames==null || stateNames.Length==0)
				throw new ArgumentNullException("stateNames");
			if (type == null)
				throw new ArgumentNullException("type");
			return GetWorkflowInstance(workflowName, stateNames, type);
		}
		/// <summary>
		/// �������ߵ�λ��Id����ʵ��״̬��ȡʵ��
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="stateName">ʵ��״̬����</param>
		/// <param name="createUserId">������Id</param>
		/// <param name="unitCode">�����ߵ�λ����</param>
		/// <returns></returns>
		protected abstract List<StateMachineWorkflowInstance> GetWorkflowInstance(string workflowName, string[] stateNames, string createUserId, string unitCode);
		/// <summary>
		/// �������ߵ�λ��Id����ʵ��״̬��ȡʵ��
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="stateName">ʵ��״̬����</param>
		/// <param name="createUserId">������Id</param>
		/// <param name="unitCode">�����ߵ�λ����</param>
		/// <returns></returns>
		List<StateMachineWorkflowInstance> IWorkflowPersistService.GetWorkflowInstance(string workflowName, string[] stateNames, string createUserId, string unitCode)
		{
			if (string.IsNullOrEmpty(workflowName))
				throw new ArgumentNullException("workflowName");
			if (stateNames == null || stateNames.Length == 0)
				throw new ArgumentNullException("stateNames");
			if (string.IsNullOrEmpty(createUserId))
				throw new ArgumentNullException("createUserId");
			return GetWorkflowInstance(workflowName, stateNames, createUserId, unitCode);
		}
		/// <summary>
		/// ��ȡĳ�û�������ʵ��
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="createUserId">�����û���Id</param>
		/// <returns></returns>
		protected abstract List<StateMachineWorkflowInstance> GetWorkflowInstance(string workflowName, string createUserId);
		/// <summary>
		/// ��ȡĳ�û�������ʵ��
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="createUserId">�����û���Id</param>
		/// <returns></returns>
		List<StateMachineWorkflowInstance> IWorkflowPersistService.GetWorkflowInstance(string workflowName, string createUserId)
		{
			if (string.IsNullOrEmpty(workflowName))
				throw new ArgumentNullException("workflowName");
			if (string.IsNullOrEmpty(createUserId))
				throw new ArgumentNullException("createUserId");
			return GetWorkflowInstance(workflowName, createUserId);
		}
		/// <summary>
		/// ��ʵ����״̬��ʵ�������ߵĵ�λ�����ȡʵ��
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="stateName">ʵ��״̬����</param>
		/// <param name="unitCode">������ʵ�����û���λ����</param>
		/// <returns></returns>
		protected abstract List<StateMachineWorkflowInstance> GetWorkflowInstance(string workflowName, string[] stateNames, string unitCode);
		/// <summary>
		/// ��ʵ����״̬��ʵ�������ߵĵ�λ�����ȡʵ��
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="stateName">ʵ��״̬����</param>
		/// <param name="unitCode">������ʵ�����û���λ����</param>
		/// <returns></returns>
		List<StateMachineWorkflowInstance> IWorkflowPersistService.GetWorkflowInstance(string workflowName, string[] stateNames, string unitCode)
		{
			if (string.IsNullOrEmpty(workflowName))
				throw new ArgumentNullException("workflowName");
			if (stateNames == null || stateNames.Length == 0)
				throw new ArgumentNullException("stateNames");
			return GetWorkflowInstance(workflowName, stateNames, unitCode);
		}
		/// <summary>
		/// ����Id��ȡĳ״̬��������ʵ��
		/// </summary>
		/// <param name="id">ʵ��Id</param>
		/// <returns></returns>
		protected abstract StateMachineWorkflowInstance GetWorkflowInstance(Guid id);
		/// <summary>
		/// ����Id��ȡĳ״̬��������ʵ��
		/// </summary>
		/// <param name="id">ʵ��Id</param>
		/// <returns></returns>
		StateMachineWorkflowInstance IWorkflowPersistService.GetWorkflowInstance(Guid id)
		{
			return GetWorkflowInstance(id);
		}
		/// <summary>
		/// ��ȡĳʵ��
		/// </summary>
		/// <param name="id">ʵ��Id</param>
		/// <param name="isAttachMore">�Ƿ�װ�ظ�����Ϣ</param>
		/// <returns></returns>
		protected abstract StateMachineWorkflowInstance GetWorkflowInstance(Guid id, bool isAttachMore);
		/// <summary>
		/// ��ȡĳʵ��
		/// </summary>
		/// <param name="id">ʵ��Id</param>
		/// <param name="isAttachMore">�Ƿ�װ�ظ�����Ϣ</param>
		/// <returns></returns>
		StateMachineWorkflowInstance IWorkflowPersistService.GetWorkflowInstance(Guid id, bool isAttachMore)
		{
			return GetWorkflowInstance(id, isAttachMore);
		}
		/// <summary>
		/// ��ȡĳ�����Ӧ��ʵ��
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="eaId">����Id</param>
		/// <param name="isAttachMore">�Ƿ�װ�ظ�����Ϣ</param>
		/// <returns></returns>
		protected abstract List<StateMachineWorkflowInstance> GetWorkflowInstance(string workflowName, int eaId, bool isAttachMore);
		/// <summary>
		/// ��ȡĳ�����Ӧ��ʵ��
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="eaId">����Id</param>
		/// <param name="isAttachMore">�Ƿ�װ�ظ�����Ϣ</param>
		/// <returns></returns>
		List<StateMachineWorkflowInstance> IWorkflowPersistService.GetWorkflowInstance(string workflowName, int eaId, bool isAttachMore)
		{
			return GetWorkflowInstance(workflowName, eaId, isAttachMore);
		}
		/// <summary>
		/// ��ȡָ��������ʱ��κ�ָ��״̬�µĹ�����ʵ��
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="startDate">��ʼ����</param>
		/// <param name="endDate">��ֹ����</param>
		/// <param name="stateName">ʵ������״̬</param>
		/// <returns></returns>
		protected abstract List<StateMachineWorkflowInstance> GetWorkflowInstance(string workflowName, DateTime startDate, DateTime endDate, string[] stateNames);
		/// <summary>
		/// ��ȡָ��������ʱ��κ�ָ��״̬�µĹ�����ʵ��
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="startDate">��ʼ����</param>
		/// <param name="endDate">��ֹ����</param>
		/// <param name="stateName">ʵ������״̬</param>
		/// <returns></returns>
		List<StateMachineWorkflowInstance> IWorkflowPersistService.GetWorkflowInstance(string workflowName, DateTime startDate, DateTime endDate, string[] stateNames)
		{
			if (string.IsNullOrEmpty(workflowName))
				throw new ArgumentNullException("workflowName");
			if (stateNames == null || stateNames.Length == 0)
				throw new ArgumentNullException("stateNames");
			return GetWorkflowInstance(workflowName, startDate, endDate, stateNames);
		}
		/// <summary>
		/// ��ȡָ��������ʱ��κ�ָ��״̬�µĹ�����ʵ��
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="startDate">��ʼ����</param>
		/// <param name="endDate">��ֹ����</param>
		/// <param name="stateName">ʵ������״̬</param>
		/// <param name="unitCode">��λ����.</param>
		/// <returns></returns>
		protected abstract List<StateMachineWorkflowInstance> GetWorkflowInstance(string workflowName, DateTime startDate, DateTime endDate, string[] stateNames, string unitCode);
		/// <summary>
		/// ��ȡָ��������ʱ��κ�ָ��״̬�µĹ�����ʵ��
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="startDate">��ʼ����</param>
		/// <param name="endDate">��ֹ����</param>
		/// <param name="stateName">ʵ������״̬</param>
		/// <param name="unitCode">��λ����.</param>
		/// <returns></returns>
		List<StateMachineWorkflowInstance> IWorkflowPersistService.GetWorkflowInstance(string workflowName, DateTime startDate, DateTime endDate, string[] stateNames, string unitCode)
		{
			if (string.IsNullOrEmpty(workflowName))
				throw new ArgumentNullException("workflowName");
			if (stateNames == null || stateNames.Length == 0)
				throw new ArgumentNullException("stateNames");
			return GetWorkflowInstance(workflowName, startDate, endDate, stateNames, unitCode);
		}
		/// <summary>
		/// ɾ��ָ��������
		/// </summary>
		/// <param name="instance"></param>
		protected abstract void DeleteWorkflowInstance(WorkflowInstance instance);
		/// <summary>
		/// ɾ��ָ��������
		/// </summary>
		/// <param name="instance"></param>
		void IWorkflowPersistService.DeleteWorkflowInstance(WorkflowInstance instance)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");
			DeleteWorkflowInstance(instance);
		}

		#endregion
	}
}

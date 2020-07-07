using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace OilDigital.Workflows.DAO
{
	/// <summary>
	/// 
	/// </summary>
	public class NHWorkflowPersistService : WorkflowPersistService
	{
		/// <summary>
		/// ���湤����ʵ��
		/// </summary>
		/// <param name="instance"></param>
		protected override StateMachineWorkflowInstance SaveWorkflowInstance(StateMachineWorkflowInstance instance)
		{
			foreach (Activity activity in instance.ExecutedActivities)
			{
				AddApprovalActivity(activity);
			}
			NHStateMachineInstanceDao dao = new NHStateMachineInstanceDao();
			NHStateMachineInstance nHInstance = dao.GetById(instance.Id, false);
			ConvertNHFromInstance(instance, nHInstance);
			dao.SaveOrUpdate(nHInstance);
			return instance;
		}
		/// <summary>
		/// ����һ���µĹ�����
		/// </summary>
		/// <param name="instance"></param>
		protected override StateMachineWorkflowInstance InsertWorkflowInstance(StateMachineWorkflowInstance instance)
		{
			foreach (Activity activity in instance.ExecutedActivities)
			{
				AddApprovalActivity(activity);
			}
			NHStateMachineInstanceDao dao = new NHStateMachineInstanceDao();
			NHStateMachineInstance nHInstance = new NHStateMachineInstance();
			nHInstance.Id = instance.Id;
			ConvertNHFromInstance(instance, nHInstance);
			dao.Save(nHInstance);
			return instance;
		}

		/// <summary>
		/// ��ȡ����ĳ״̬�µ�����״̬��������ʵ��
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="stateName">״̬����</param>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		protected override List<StateMachineWorkflowInstance> GetWorkflowInstance(string workflowName, string[] stateNames, Type type)
		{
			NHStateMachineInstanceDao dao = new NHStateMachineInstanceDao();
			List<NHStateMachineInstance> nHInstanceList = new List<NHStateMachineInstance>();
			nHInstanceList = dao.GetInstanceByStateName(workflowName, stateNames, type.Name);
			List<StateMachineWorkflowInstance> instanceList = new List<StateMachineWorkflowInstance>();
			foreach (NHStateMachineInstance instance in nHInstanceList)
			{
				StateMachineWorkflowInstance stateMachineInstance = ConvertNHToInstance(instance, false);
				instanceList.Add(stateMachineInstance);
			}
			return instanceList;
		}

		/// <summary>
		/// �������ߵ�λ��Id����ʵ��״̬��ȡʵ��
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="stateName">ʵ��״̬����</param>
		/// <param name="createUserId">������Id</param>
		/// <param name="unitCode">�����ߵ�λ����</param>
		/// <returns></returns>
		protected override List<StateMachineWorkflowInstance> GetWorkflowInstance(string workflowName, string[] stateNames, string createUserId, string unitCode)
		{
			NHStateMachineInstanceDao dao = new NHStateMachineInstanceDao();
			List<StateMachineWorkflowInstance> instanceList = new List<StateMachineWorkflowInstance>();
			List<NHStateMachineInstance> nHInstanceList = new List<NHStateMachineInstance>();
			nHInstanceList = dao.GetUserInstance(workflowName, stateNames, createUserId, unitCode);
			foreach (NHStateMachineInstance nHInstance in nHInstanceList)
			{
				StateMachineWorkflowInstance instance = ConvertNHToInstance(nHInstance, false);
				instanceList.Add(instance);
			}
			return instanceList;
		}

		/// <summary>
		/// ��ʵ����״̬��ʵ�������ߵĵ�λ�����ȡʵ��
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="stateName">ʵ��״̬����</param>
		/// <param name="unitCode">������ʵ�����û���λ����</param>
		/// <returns></returns>
		protected override List<StateMachineWorkflowInstance> GetWorkflowInstance(string workflowName,string[] stateNames, string unitCode)
		{
			NHStateMachineInstanceDao dao = new NHStateMachineInstanceDao();
			List<StateMachineWorkflowInstance> instanceList = new List<StateMachineWorkflowInstance>();
			List<NHStateMachineInstance> nHInstanceList = new List<NHStateMachineInstance>();
			nHInstanceList = dao.GetUserInstance(workflowName, stateNames, unitCode);
			foreach (NHStateMachineInstance nHInstance in nHInstanceList)
			{
				StateMachineWorkflowInstance instance = ConvertNHToInstance(nHInstance, false);
				instanceList.Add(instance);
			}
			return instanceList;
		}

		/// <summary>
		/// ��ȡĳ�û�������ʵ��
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="createUserId">�����û���Id</param>
		/// <returns></returns>
		protected override List<StateMachineWorkflowInstance> GetWorkflowInstance(string workflowName, string createUserId)
		{
			NHStateMachineInstanceDao dao = new NHStateMachineInstanceDao();
			List<StateMachineWorkflowInstance> instanceList = new List<StateMachineWorkflowInstance>();
			List<NHStateMachineInstance> nHInstanceList = new List<NHStateMachineInstance>();
			nHInstanceList = dao.GetUserInstance(workflowName, createUserId);
			foreach (NHStateMachineInstance nHInstance in nHInstanceList)
			{
				StateMachineWorkflowInstance instance = ConvertNHToInstance(nHInstance, false);
				instanceList.Add(instance);
			}
			return instanceList;
		}

		/// <summary>
		/// ��ȡĳ�����Ӧ��ʵ��
		/// </summary>
		/// <param name="workflowName"></param>
		/// <param name="eaId">����Id</param>
		/// <param name="isAttachMore">�Ƿ���Ҫ����������Ϣ:��ʵ������ִ�ж�����ʵ��</param>
		/// <returns></returns>
		protected override List<StateMachineWorkflowInstance> GetWorkflowInstance(string workflowName, int eaId, bool isAttachMore)
		{
			NHStateMachineInstanceDao dao = new NHStateMachineInstanceDao();
			List<StateMachineWorkflowInstance> instanceList = new List<StateMachineWorkflowInstance>();
			List<NHStateMachineInstance> nHInstanceList = new List<NHStateMachineInstance>();
			nHInstanceList = dao.GetUserInstance(workflowName, eaId);
			foreach (NHStateMachineInstance nHInstance in nHInstanceList)
			{
				StateMachineWorkflowInstance instance = ConvertNHToInstance(nHInstance, isAttachMore);
				instanceList.Add(instance);
			}
			return instanceList;
		}

		/// <summary>
		/// ɾ��ָ��������
		/// </summary>
		/// <param name="instance"></param>
		protected override void DeleteWorkflowInstance(WorkflowInstance instance)
		{
			NHStateMachineInstanceDao dao = new NHStateMachineInstanceDao();
			Guid instanceId = instance.Id;
			dao.Delete(dao.GetInstanceById(instanceId)[0]);
		}

		/// <summary>
		/// ��NHStateMachineInstance�������л�Ϊ״̬������������
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="isAttachMore">�Ƿ���Ҫ����������Ϣ:��ʵ������ִ�ж�����ʵ��</param>
		/// <returns></returns>
		private StateMachineWorkflowInstance ConvertNHToInstance(NHStateMachineInstance instance, bool isAttachMore)
		{
			StateMachineWorkflowInstance newInstance = new StateMachineWorkflowInstance();
			newInstance.WorkflowName = instance.WorkflowName;
			newInstance.EaId = instance.EaId;
			newInstance.Id = instance.Id;
			if (!string.IsNullOrEmpty(instance.ParentId))
			{
				newInstance.ParentId = new Guid(instance.ParentId);
			}
			newInstance.PersistTime = instance.PersistTime;
			newInstance.Properties.Add("UnitCode", instance.CreaterUnit);
			newInstance.Properties.Add("CreaterUserId", instance.CreaterUserId);
			newInstance.StateName = instance.StateName;
			if (!string.IsNullOrEmpty(instance.Children))
			{
				string[] childrenId = instance.Children.Split(';');
				List<Guid> childrenIds = new List<Guid>();
				for (int i = 0; i < childrenId.Length; i++)
				{
					Guid childId = new Guid(childrenId[i]);
					childrenIds.Add(childId);
				}
				newInstance.ChildrenId = childrenIds;
			}
			if (!string.IsNullOrEmpty(instance.StateRecord))
			{
				string[] stateNames = instance.StateRecord.Split(';');
				newInstance.StateRecordNames = new List<string>(stateNames);
			}
			if (isAttachMore)
			{
				if (!string.IsNullOrEmpty(instance.ExecuteActivitiesIds))
				{
					string[] ids = instance.ExecuteActivitiesIds.Split(";".ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
					List<long> allIds= new List<long>();
					for (int i = 0; i < ids.Length; i++)
					{
						long activityId;
						if (long.TryParse(ids[i], out activityId))
						{
							allIds.Add(activityId);
						}
					}
					newInstance.ExecutedActivities.AddRange(GetApprovalActivitys(allIds.ToArray()));
				}
			}

			return newInstance;
		}

		/// <summary>
		/// ��״̬��������ʵ�����л�ת��ΪNHStateMachineInstance
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="nHInstance">The n H instance.</param>
		private void ConvertNHFromInstance(StateMachineWorkflowInstance instance, NHStateMachineInstance nHInstance)
		{
			nHInstance.EaId = instance.EaId;
			if (instance.ParentId != Guid.Empty)
			{
				nHInstance.ParentId = instance.ParentId.ToString();
			}
			nHInstance.WorkflowName = instance.WorkflowName;
			nHInstance.StateName = instance.StateName;
			nHInstance.CreaterUserId = instance.Properties["CreaterUserId"];
			nHInstance.CreaterUnit = instance.Properties["UnitCode"];
			nHInstance.PersistTime = instance.PersistTime;
			nHInstance.TypeName = typeof(StateMachineWorkflowInstance).Name;
			if (instance.ChildrenId != null)
			{
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < instance.ChildrenId.Count; i++)
				{
					if (i != 0) sb.Append(";");
					sb.Append(instance.ChildrenId[i].ToString());
				}
				nHInstance.Children = sb.ToString();
			}
			else
			{
				nHInstance.Children = string.Empty;
			}


			if (instance.ExecutedActivities != null)
			{
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < instance.ExecutedActivities.Count; i++)
				{
					if (i != 0) sb.Append(";");
					sb.Append(instance.ExecutedActivities[i].Id.ToString());
				}
				nHInstance.ExecuteActivitiesIds = sb.ToString();
			}
			else
			{
				nHInstance.ExecuteActivitiesIds = string.Empty;
			}


			if (instance.StateRecordNames != null)
			{
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < instance.StateRecordNames.Count; i++)
				{
					if (i != 0) sb.Append(";");
					sb.Append(instance.StateRecordNames[i]);
				}
				nHInstance.StateRecord = sb.ToString();
			}
			else
			{
				nHInstance.StateRecord = string.Empty;
			}
		}

		/// <summary>
		/// ����Id��ȡĳ״̬��������ʵ��
		/// </summary>
		/// <param name="id">ʵ��Id</param>
		/// <returns></returns>
		protected override StateMachineWorkflowInstance GetWorkflowInstance(Guid id)
		{
			NHStateMachineInstanceDao dao = new NHStateMachineInstanceDao();
			List<NHStateMachineInstance> instanceList = dao.GetInstanceById(id);
			if (instanceList == null || instanceList.Count == 0)
				throw new ApplicationException(string.Format("\"{0}\" entity cannot found!", id));
			else
			{
				NHStateMachineInstance nHInstance = instanceList[0];
				return ConvertNHToInstance(nHInstance, true);
			}
		}
		/// <summary>
		/// Internals the get workflow instance.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		protected StateMachineWorkflowInstance InternalGetWorkflowInstance(Guid id)
		{
			NHStateMachineInstanceDao dao = new NHStateMachineInstanceDao();
			List<NHStateMachineInstance> instanceList = dao.GetInstanceById(id);
			if (instanceList == null || instanceList.Count == 0)
				throw new ApplicationException(string.Format("\"{0}\" entity cannot found!", id));
			else
			{
				NHStateMachineInstance nHInstance = instanceList[0];
				return ConvertNHToInstance(nHInstance, true);
			}
		}
		/// <summary>
		/// ��ȡĳʵ��
		/// </summary>
		/// <param name="id">ʵ��Id</param>
		/// <param name="isAttachMore">�Ƿ�װ�ظ�����Ϣ</param>
		/// <returns></returns>
		protected override StateMachineWorkflowInstance GetWorkflowInstance(Guid id, bool isAttachMore)
		{
			NHStateMachineInstanceDao dao = new NHStateMachineInstanceDao();
			List<NHStateMachineInstance> instanceList = dao.GetInstanceById(id);
			if (instanceList == null || instanceList.Count == 0)
				throw new ApplicationException(string.Format("\"{0}\" entity cannot found!", id));
			else
			{
				NHStateMachineInstance nHInstance = instanceList[0];
				return ConvertNHToInstance(nHInstance, isAttachMore);
			}
		}

		/// <summary>
		/// ��ȡָ��������ʱ��κ�ָ��״̬�µĹ�����ʵ��
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="startDate">��ʼ����</param>
		/// <param name="endDate">��ֹ����</param>
		/// <param name="stateName">ʵ������״̬</param>
		/// <returns></returns>
		protected override List<StateMachineWorkflowInstance> GetWorkflowInstance(string workflowName, DateTime startDate, DateTime endDate,string[] stateNames)
		{
			NHStateMachineInstanceDao dao = new NHStateMachineInstanceDao();
			List<StateMachineWorkflowInstance> instanceList = new List<StateMachineWorkflowInstance>();
			List<NHStateMachineInstance> nHInstanceList = dao.GetInstanceByPersisteTime(workflowName, startDate, endDate, stateNames);
			foreach (NHStateMachineInstance nHInstance in nHInstanceList)
			{
				StateMachineWorkflowInstance instance = ConvertNHToInstance(nHInstance, false);
				instanceList.Add(instance);
			}
			return instanceList;
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
		protected override List<StateMachineWorkflowInstance> GetWorkflowInstance(string workflowName, DateTime startDate, DateTime endDate,string[] stateNames, string unitCode)
		{
			NHStateMachineInstanceDao dao = new NHStateMachineInstanceDao();
			List<StateMachineWorkflowInstance> instanceList = new List<StateMachineWorkflowInstance>();
			List<NHStateMachineInstance> nHInstanceList = dao.GetInstanceByPersisteTime(workflowName, startDate, endDate, stateNames, unitCode);
			foreach (NHStateMachineInstance nHInstance in nHInstanceList)
			{
				StateMachineWorkflowInstance instance = ConvertNHToInstance(nHInstance, false);
				instanceList.Add(instance);
			}
			return instanceList;
		}
		/// <summary>
		/// ������Ϣ
		/// </summary>
		/// <param name="approvalActivity">�����</param>
		protected void AddApprovalActivity(Activity approvalActivity)
		{
			NHActivityDao dao = new NHActivityDao();
			NHActivity activity = new NHActivity(approvalActivity);
			if (activity.Id > 0)
				return;
			dao.SaveOrUpdate(activity);
			approvalActivity.Id = activity.Id;
		}

		/// <summary>
		/// ��Id��ȡ�����ʵ��
		/// </summary>
		/// <param name="id">�����Id</param>
		/// <returns></returns>
		protected Activity GetApprovalActivity(long id)
		{
			NHActivityDao dao = new NHActivityDao();
			NHActivity activity = dao.GetById(id, false);
			return activity.CovertTo();
		}

		protected List<Activity> GetApprovalActivitys(long[] ids)
		{
			NHActivityDao dao = new NHActivityDao();
			List<NHActivity> lists = dao.GetNHActivitys(ids);
			List<Activity> rtn = new List<Activity>();
			foreach (NHActivity one in lists)
			{
				rtn.Add(one.CovertTo());
			}
			return rtn;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;

namespace OilDigital.Workflows
{
	/// <summary>
	/// ������Ŀ�����ߣ�����ʵ���������Ϣ���뵽������Ŀ��
	/// </summary>
	public interface ITaskItemPropertyDistiller
	{
		/// <summary>
		/// ����һ���µ�������Ŀ����
		/// </summary>
		/// <returns></returns>
		ITaskItem CreateTaskItem();
		/// <summary>
		/// ����������Ŀ���������ֵ
		/// </summary>
		/// <param name="collection">The collection.</param>
		/// <param name="taskItem">������Ŀ</param>
		/// <param name="userIdentity">The user identity.</param>
		void FillItemProperty(InstanceCollection collection, ITaskItem taskItem, IUserIdentity userIdentity);
	}

	/// <summary>
	/// ������Ŀ�����߻��࣬���ݻ������ݵ�ָ����taskItem
	/// </summary>
	public class TaskItemPropertyDistillerBase : ITaskItemPropertyDistiller
	{

		#region ITaskItemPropertyConnector2 Members

		/// <summary>
		/// ����������Ŀ���������ֵ
		/// </summary>
		/// <param name="collection">The collection.</param>
		/// <param name="taskItem">������Ŀ</param>
		/// <param name="userIdentity">The user identity.</param>
		public virtual void FillItemProperty(InstanceCollection collection, ITaskItem taskItem, IUserIdentity userIdentity)
		{
			if (collection == null || collection.Count == 0)
				throw new ArgumentException("ʵ�����ϲ���Ϊ��", "collection");
			taskItem.EaId = collection[0].Instance.EaId;
			InstanceWithRole instance = GetInstance(collection);
			taskItem.InstanceId = instance.Instance.Id;
			taskItem.PersistTime = instance.Instance.PersistTime;
			taskItem.ExceedTime = instance.ExceedTime;
		}

		/// <summary>
		/// Gets the root instance.
		/// </summary>
		/// <param name="collection">The collection.</param>
		/// <returns></returns>
		private InstanceWithRole GetInstance(InstanceCollection collection)
		{
			foreach (InstanceWithRole one in collection)
			{
				if (one.Instance.ParentId == Guid.Empty || one.Instance.ParentId == null)
					return one;
			}
			return collection[0];
		}

		/// <summary>
		/// ����һ���µ�������Ŀ����
		/// </summary>
		/// <returns></returns>
		public virtual ITaskItem CreateTaskItem()
		{
			return new TaskItem();
		}
		#endregion
	}
}

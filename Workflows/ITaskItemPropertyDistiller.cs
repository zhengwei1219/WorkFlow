using System;
using System.Collections.Generic;
using System.Text;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 任务项目连接者，负责将实例的相关信息传入到任务项目中
	/// </summary>
	public interface ITaskItemPropertyDistiller
	{
		/// <summary>
		/// 创建一个新的任务项目对象
		/// </summary>
		/// <returns></returns>
		ITaskItem CreateTaskItem();
		/// <summary>
		/// 设置任务项目的相关属性值
		/// </summary>
		/// <param name="collection">The collection.</param>
		/// <param name="taskItem">工作项目</param>
		/// <param name="userIdentity">The user identity.</param>
		void FillItemProperty(InstanceCollection collection, ITaskItem taskItem, IUserIdentity userIdentity);
	}

	/// <summary>
	/// 任务项目连接者基类，传递基本数据到指定的taskItem
	/// </summary>
	public class TaskItemPropertyDistillerBase : ITaskItemPropertyDistiller
	{

		#region ITaskItemPropertyConnector2 Members

		/// <summary>
		/// 设置任务项目的相关属性值
		/// </summary>
		/// <param name="collection">The collection.</param>
		/// <param name="taskItem">工作项目</param>
		/// <param name="userIdentity">The user identity.</param>
		public virtual void FillItemProperty(InstanceCollection collection, ITaskItem taskItem, IUserIdentity userIdentity)
		{
			if (collection == null || collection.Count == 0)
				throw new ArgumentException("实例集合不能为空", "collection");
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
		/// 创建一个新的任务项目对象
		/// </summary>
		/// <returns></returns>
		public virtual ITaskItem CreateTaskItem()
		{
			return new TaskItem();
		}
		#endregion
	}
}

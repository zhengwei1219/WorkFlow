using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 任务提取者
	/// </summary>
	public class TaskDistiller
	{
		/// <summary>
		/// 实例提取器集合
		/// </summary>
		internal InstanceDistiller[] InstanceDistillers;
		/// <summary>
		/// 过滤器集合
		/// </summary>
		internal InstanceFilter[] Filters;
		/// <summary>
		/// 呈现器
		/// </summary>
		internal ITaskRender Render;
		/// <summary>
		/// 项目提取器
		/// </summary>
		internal ITaskItemDistiller ItemDistiller;

		private ApprovalRole forRole;

		/// <summary>
		/// Gets or sets ,不同的审批角色有不同的Taskdistiller,ForRole表示此TaskDistill对应的审批角色对象,<seealso cref="ApprovalRole"/>的实例
		/// </summary>
		/// <value>For role.</value>
		public ApprovalRole ForRole
		{
			get { return forRole; }
			set { forRole = value; }
		}

		/// <summary>
		/// 获取指定工作流、角色、时间段的实例集合
		/// </summary>
		/// <param name="workflowName">流程名称</param>
		/// <param name="userIdentity">用户身份</param>
		/// <param name="role">审批角色</param>
		/// <param name="startDate">时间段起始时间</param>
		/// <param name="endDate">时间段截止时间</param>
		/// <returns></returns>
		public InstanceCollection Distill(string workflowName, IUserIdentity userIdentity, ApprovalRole role, DateTime startDate, DateTime endDate)
		{
			//提取实例集合的过程，先遍历每一个提取者对象进行实例提取，后遍历每个过滤器进行实例过滤，最后返回剩余的实例的集合
			InstanceCollection instances = new InstanceCollection();
			StateMachineWorkflow workflow = (StateMachineWorkflow)WorkflowRuntime.Current.GetService<IWorkFlowDefinePersistService>().GetWorkflowDefine(workflowName);
			foreach (InstanceDistiller distiller in InstanceDistillers)
			{
				instances.AddRange(distiller.InternalDistill(workflowName, userIdentity, role, startDate, endDate));
			}
			if (Filters != null)
			{
				foreach (InstanceFilter filter in Filters)
				{
					if (filter is IInstanceCollectionFilter)
					{
						IInstanceCollectionFilter collFilter = (IInstanceCollectionFilter)filter;
						collFilter.Filter(instances, role, userIdentity);
					}
					else
					{
						for (int i = instances.Count - 1; i >= 0; i--)
						{
							if (filter.InternalIsMatch(instances[i].Instance, role, userIdentity))
								instances.RemoveAt(i);
						}
					}
				}
			}
			return instances;
		}

		#region ICloneable Members

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		/// A new object that is a copy of this instance.
		/// </returns>
		internal TaskDistiller Clone()
		{
			TaskDistiller distiller = new TaskDistiller();
			List<InstanceDistiller> distillers = new List<InstanceDistiller>();
			foreach (InstanceDistiller o in InstanceDistillers)
			{
				distillers.Add(o.Clone());
			}
			distiller.InstanceDistillers = distillers.ToArray();
			if (Filters != null)
			{
				List<InstanceFilter> filters = new List<InstanceFilter>();
				foreach (InstanceFilter filter in Filters)
				{
					filters.Add(filter.Clone());
				}
				distiller.Filters = filters.ToArray();
			}
			distiller.ItemDistiller = ItemDistiller;
			distiller.Render = Render;
			return distiller;
		}

		#endregion
	}
}

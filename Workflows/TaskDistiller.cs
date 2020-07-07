using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;

namespace OilDigital.Workflows
{
	/// <summary>
	/// ������ȡ��
	/// </summary>
	public class TaskDistiller
	{
		/// <summary>
		/// ʵ����ȡ������
		/// </summary>
		internal InstanceDistiller[] InstanceDistillers;
		/// <summary>
		/// ����������
		/// </summary>
		internal InstanceFilter[] Filters;
		/// <summary>
		/// ������
		/// </summary>
		internal ITaskRender Render;
		/// <summary>
		/// ��Ŀ��ȡ��
		/// </summary>
		internal ITaskItemDistiller ItemDistiller;

		private ApprovalRole forRole;

		/// <summary>
		/// Gets or sets ,��ͬ��������ɫ�в�ͬ��Taskdistiller,ForRole��ʾ��TaskDistill��Ӧ��������ɫ����,<seealso cref="ApprovalRole"/>��ʵ��
		/// </summary>
		/// <value>For role.</value>
		public ApprovalRole ForRole
		{
			get { return forRole; }
			set { forRole = value; }
		}

		/// <summary>
		/// ��ȡָ������������ɫ��ʱ��ε�ʵ������
		/// </summary>
		/// <param name="workflowName">��������</param>
		/// <param name="userIdentity">�û����</param>
		/// <param name="role">������ɫ</param>
		/// <param name="startDate">ʱ�����ʼʱ��</param>
		/// <param name="endDate">ʱ��ν�ֹʱ��</param>
		/// <returns></returns>
		public InstanceCollection Distill(string workflowName, IUserIdentity userIdentity, ApprovalRole role, DateTime startDate, DateTime endDate)
		{
			//��ȡʵ�����ϵĹ��̣��ȱ���ÿһ����ȡ�߶������ʵ����ȡ�������ÿ������������ʵ�����ˣ���󷵻�ʣ���ʵ���ļ���
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

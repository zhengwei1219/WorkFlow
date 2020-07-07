using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 获取用户审批信息的一般类
	/// </summary>
	/// <typeparam name="T">用于替换被过滤掉的记录的类型</typeparam>
	public class DrillApprovalInfo<T>
	{
		/// <summary>
		/// 构造一个提取审批信息的通用类,不显示任何被过滤掉的记录
		/// </summary>
		public DrillApprovalInfo() : this(false, default(T)) { }

		/// <summary>
		/// 构造一个提取审批信息的通用类并用特定的记录来替换掉被过滤的记录
		/// </summary>
		/// <param name="hidden">用于替换被过滤掉的记录的替换记录</param>
		public DrillApprovalInfo(T hidden) : this( true, hidden) { }

		/// <summary>
		/// 构造一个提取审批信息的通用类
		/// </summary>
		/// <param name="showFiltered">是否隐藏显示被过滤掉的记录</param>
		/// <param name="hidden">用于替换被过滤掉的记录的替换记录</param>
		public DrillApprovalInfo( bool showFiltered, T hidden)
		{
			
			this.showFiltered = showFiltered;
			this.hidden = hidden;
			//初始化空filter
			filters = new List<IFilter<T>>();
			//获取持久化实例
			approvalService = WorkflowRuntime.Current.GetService<IApprovalSaveService>();
		}

		//审批持久化类实例
		/// <summary>
		/// 
		/// </summary>
		protected IApprovalSaveService approvalService;

		/// <summary>
		/// 
		/// </summary>
		protected List<IFilter<T>> filters;

		/// <summary>
		/// 过滤接口集合
		/// </summary>
		public List<IFilter<T>> Filters
		{
			get { return filters; }
			set { filters = value; }
		}

		private bool showFiltered;

		/// <summary>
		/// 是否显示被过滤的记录
		/// </summary>
		public bool ShowFiltered
		{
			get { return showFiltered; }
			set { showFiltered = value; }
		}

		/// <summary>
		/// 显示被过滤的记录的时候,替换显示的内容
		/// </summary>
		private T hidden;
		/// <summary>
		/// 获取指定工作流实例Id的审批记录
		/// </summary>
		/// <param name="applicationName">Name of the application.</param>
		/// <param name="eaId">The ea id.</param>
		/// <returns></returns>
		public List<T> GetInfo(string applicationName, int eaId)
		{
			if (eaId <= 0)
				throw new ArgumentException("eaid must be greater than zero in GetRecordInfo(eaid)", "eaid");

			//获取所有审批记录
			List<T> recordList = GetAllApprovalInfoById(applicationName, eaId);

			for (int i = recordList.Count -1; i >=0;i--)
			{
				//没有定义过滤器,那么保留
				if(filters.Count==0) continue;

				bool isKept = false;
				//按照筛选器挨个筛选
				for (int j = 0; j < filters.Count; j++)
				{
					isKept = isKept || (!filters[j].IsFiltered(recordList[i]));
					if (isKept) break;
				}

				//被过滤掉了
				if (!isKept)
				{
					if (showFiltered)
					{
						//recordList[i] = GetHiddenOnShow(recordList[i]);
						recordList[i] = hidden;
					}
					else
						recordList.RemoveAt(i);
				}
			}
			return recordList;
		}

		/*
		protected virtual T GetHiddenOnShow(T record)
		{
 			if (record is IOperatorRelative)
			{
				((IOperatorRelative)hidden).OperatorTime=((IOperatorRelative)record).OperatorTime;
			}
			return hidden;
		}
		*/
		/// <summary>
		/// 获取全部审批信息的方法,必须被各个子类重载
		/// </summary>
		/// <param name="applicationName">Name of the application.</param>
		/// <param name="eaid">The eaid.</param>
		/// <returns></returns>
		protected virtual List<T> GetAllApprovalInfoById(string applicationName, int eaid)
		{
			return null;
		}
	}
}

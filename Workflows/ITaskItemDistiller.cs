using System;
using System.Collections.Generic;
using System.Text;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 从工作流实例对象中提取可以执行的动作
	/// </summary>
	public interface ITaskItemDistiller
	{
		
		/// <summary>
		/// 获取实例的操作项目集合
		/// </summary>
		/// <param name="collection">实例集合:针对某一个特定的审批对象(按照被审批对象进行合并后的,单个审批实例对应的工作流集合)</param>
		/// <param name="userIdentity">用户身份</param>
		/// <returns></returns>
		ITaskItem Distill(InstanceCollection collection, IUserIdentity userIdentity);
		/// <summary>
		/// 获取实例动作项目集合
		/// </summary>
		/// <param name="instanceWithRole">实例</param>
		/// <param name="userIdentity">用户身份</param>
		/// <returns></returns>
		//List<ITaskActionItem> Distill(InstanceWithRole instanceWithRole, IUserIdentity userIdentity);
	}

	/// <summary>
	/// 可被初始化的Distiller
	/// </summary>
	public interface IInitializableDistiller
	{
		/// <summary>
		/// 在调用Distill 方法进行对单个审批实例进行提取前,针对本次提取进行的一些初始化动作.
		/// 可以重载此方法,进行一些初始化操作.
		/// </summary>
		/// <param name="ForRole">不同的审批角色有不同的ITaskItemDistiller,ForRole表示此TaskDistill对应的审批角色对象,<seealso cref="ApprovalRole"/>的实例</param>
		/// <param name="allCollection">本次提取的所有工作流实例的集合,注意此集合中,单个被审批对象可能有多个审批实例</param>
		/// <value>For role.</value>
		void Initialize(ApprovalRole ForRole, InstanceCollection allCollection);

	}
}

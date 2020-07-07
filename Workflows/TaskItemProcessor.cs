using System;
using System.Collections.Generic;
namespace OilDigital.Workflows
{
	/// <summary>
	/// 任务项目处理器接口，负责将工作流实例转换为任务列表条目
	/// </summary>
	public interface ITaskItemProcessor
	{
		/// <summary>
		/// 获取或设置任务项目连接者
		/// </summary>
		ITaskItemPropertyConnector PropertyConnector { get;set;}
		/// <summary>
		/// Gets the user identity.
		/// </summary>
		/// <value>The user identity.</value>
		IUserIdentity UserIdentity { get;}
		/// <summary>
		/// 创建一个新的任务项目对象
		/// </summary>
		/// <returns></returns>
		ITaskItem CreateTaskItem();
		/// <summary>
		/// 将实例集合转换为实例项目列表
		/// </summary>
		/// <param name="instances">实例集合</param>
		/// <returns></returns>
		List<ITaskItem> GenerateTaskTable(List<StateMachineWorkflowInstance> instances);
		/// <summary>
		/// 将实例集合转换为实例项目列表
		/// </summary>
		/// <param name="instances">实例集合</param>
		/// <param name="isQuery">实例集合是查询结果为true 是用户办理的任务为false</param>
		/// <returns></returns>
		List<ITaskItem> GenerateTaskTable(List<StateMachineWorkflowInstance> instances, bool isQuery);
		/// <summary>
		/// 获取实例的操作项目集合
		/// </summary>
		/// <param name="instance">某实例</param>
		/// <param name="isToolbar">是否是工具栏</param>
		/// <returns></returns>
		List<ITaskActionItem> GetInstanceActionItems(StateMachineWorkflowInstance instance, bool isToolbar);
		/// <summary>
		/// 获取实例某角色的操作项目集合
		/// </summary>
		/// <param name="instance">某实例</param>
		/// <param name="role">某角色</param>
		/// <returns></returns>
		List<ITaskActionItem> GetInstanceActionItems(StateMachineWorkflowInstance instance, ApprovalRole role);
		/// <summary>
		/// 获取某实例查询结果的操作项目集合
		/// </summary>
		/// <param name="instance">某实例</param>
		/// <returns></returns>
		List<ITaskActionItem> GetQueryPageViewActionItems(StateMachineWorkflowInstance instance);
		/// <summary>
		/// 获取工具栏附加操作项目
		/// </summary>
		/// <param name="instance">某实例</param>
		/// <returns></returns>
		List<ITaskActionItem> GetToolbarAddtionActionItems(StateMachineWorkflowInstance instance);
	}

	/// <summary>
	/// 任务项目连接者，负责将实例的相关信息转入到任务项目中
	/// </summary>
	public interface ITaskItemPropertyConnector
	{
		/// <summary>
		/// 设置任务项目的相关属性值
		/// </summary>
		/// <param name="instance">该任务项目相关的工作流实例</param>
		/// <param name="taskItem">工作项目</param>
		void FillItemProperty(StateMachineWorkflowInstance instance, ITaskItem taskItem);
	}

	/// <summary>
	/// 任务项目处理器的基类(template method design pattern)
	/// </summary>
	public abstract class TaskItemProcessor : ITaskItemProcessor
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TaskItemProcessor"/> class.
		/// </summary>
		/// <param name="userIdentity">The user identity.</param>
		public TaskItemProcessor(IUserIdentity userIdentity)
		{
			this.userIdentity = userIdentity;
			this.unitCode = userIdentity.GetUserUnitCode().Trim();
		}
		private IUserIdentity userIdentity;
		/// <summary>
		/// 用户的身份
		/// </summary>
		/// <value>The user identity.</value>
		public IUserIdentity UserIdentity
		{
			get { return this.userIdentity; }
		}
		private string unitCode;
		/// <summary>
		/// 用户的单位代码
		/// </summary>
		protected string UnitCode
		{
			get { return this.unitCode; }
		}
		#region ITaskItemProcessor Members

		/// <summary>
		/// Creates the task item.
		/// </summary>
		/// <returns></returns>
		public abstract ITaskItem CreateTaskItem();
		/// <summary>
		/// 获取和设置任务项目属性连接器
		/// </summary>
		public abstract ITaskItemPropertyConnector PropertyConnector { get;set;}
		/// <summary>
		/// 为每一个工作流实例创建任务项
		/// </summary>
		/// <param name="instances">实例集合</param>
		/// <returns></returns>
		public List<ITaskItem> GenerateTaskTable(List<StateMachineWorkflowInstance> instances)
		{
			return GenerateTaskTable(instances, false);
		}
		/// <summary>
		/// 将实例集合转换为实例项目列表.提取参数中指定的一组工作流实例,每一个实例允许执行的动作形成一个任务表.
		/// </summary>
		/// <param name="instances">实例集合</param>
		/// <param name="isQuery">实例集合是查询结果为true 是用户办理的任务为false</param>
		/// <returns></returns>
		public List<ITaskItem> GenerateTaskTable(List<StateMachineWorkflowInstance> instances, bool isQuery)
		{
			List<ITaskItem> taskList = new List<ITaskItem>();
			//遍历每个实例
			foreach (StateMachineWorkflowInstance instance in instances)
			{
				ITaskItem oneItem = CreateTaskItem();
				//填充实例项目的内容
				PropertyConnector.FillItemProperty(instance, oneItem);
				//如果实例列表为查询结果 则获取查询结果的动作项目，否则调用获取任务类型的动作项目
				if (isQuery)
				{
					List<ITaskActionItem> items = GetQueryPageViewActionItems(instance);
					//如果对某实例无操作项目，则直接跳过该实例
					if (items == null)
						continue;
					oneItem.ActionItems.AddRange(items);
				}
				else
				{
					List<ITaskActionItem> items = GetInstanceActionItems(instance, false);
					//如果对某实例无操作项目，则直接跳过该实例
					if (items == null || items.Count == 0)
						continue;
					oneItem.ActionItems.AddRange(items);
				}
				taskList.Add(oneItem);
			}
			return taskList;
		}
		/// <summary>
		/// 获取实例的操作项目集合,方法是从工作流配置xml文件中定义的操作(动作),如果是作为工具条形式提取,那么自动<see cref="GetToolbarAddtionActionItems"/>方法,附加额外的自定义项目.
		/// </summary>
		/// <param name="instance">某实例</param>
		/// <param name="isTooBar">if set to <c>true</c> 工具条形式提取.</param>
		/// <returns></returns>
		public List<ITaskActionItem> GetInstanceActionItems(StateMachineWorkflowInstance instance, bool isTooBar)
		{
			List<ITaskActionItem> items = new List<ITaskActionItem>();
			List<ApprovalRole> roles = WorkflowUtility.GetUserRoles(instance.WorkflowName, userIdentity.GetUserId());
			//如是任务栏，则先添加任务栏相关的动作项目
			if (isTooBar)
				items.AddRange(GetToolbarAddtionActionItems(instance));
			//遍历当前用户所有角色，添加每个角色对应的动作项目
			foreach (ApprovalRole role in roles)
			{
				items.AddRange(GetInstanceActionItems(instance, role));
			}
			return items;
		}
		/// <summary>
		/// 获取实例中,指定的角色的可以执行的操作项目集合.即:从工作流配置文件xml中,如何提取可执行操作.
		/// </summary>
		/// <param name="instance">某实例</param>
		/// <param name="role">某角色</param>
		/// <returns></returns>
		public abstract List<ITaskActionItem> GetInstanceActionItems(StateMachineWorkflowInstance instance, ApprovalRole role);

		/// <summary>
		///  获取工具栏附加操作项目.在提取工作流实例可执行的操作集合,并呈现为工具条时(如:在立项详情页面的下面显示可执行的动作),除了在xml文件中定义的操作(动作)外,需要额外附加的动作项目.
		/// </summary>
		/// <param name="instance">某实例</param>
		/// <returns></returns>
		public abstract List<ITaskActionItem> GetToolbarAddtionActionItems(StateMachineWorkflowInstance instance);
		/// <summary>
		///获取某实例查询结果的操作项目集合
		/// </summary>
		/// <param name="instance">某实例</param>
		/// <returns></returns>
		public abstract List<ITaskActionItem> GetQueryPageViewActionItems(StateMachineWorkflowInstance instance);

		#endregion
	}

	/// <summary>
	/// 
	/// </summary>
	public interface IItemProcessor
	{
		/// <summary>
		/// Generates the task table.
		/// </summary>
		/// <param name="instances">The instances.</param>
		/// <returns></returns>
		List<ITaskItem> GenerateTaskTable(List<StateMachineWorkflowInstance> instances);
		/// <summary>
		/// Gets the instance action items.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <returns></returns>
		List<ITaskActionItem> GetInstanceActionItems(StateMachineWorkflowInstance instance);
	}
}

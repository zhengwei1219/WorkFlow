using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 
	/// </summary>
	public interface ITaskActionItem
	{
		/// <summary>
		/// 动作名称
		/// </summary>
		/// <value>The name.</value>
		string Name { get;}
		/// <summary>
		/// 动作描述
		/// </summary>
		/// <value>The description.</value>
		string Description { get;}
		/// <summary>
		/// 执行该动作的url,和ActionScript只需要定义两者之一
		/// </summary>
		/// <value>The action URL.</value>
		string ActionUrl { get;}
		/// <summary>
		/// 执行该动作的Script脚本,和ActionUrl只需要定义两者之一
		/// </summary>
		/// <value>The action script.</value>
		string ActionScript { get;}
	}
	/// <summary>
	/// 任务项目
	/// </summary>
	public class TaskActionItem : ITaskActionItem
	{
		/// <summary>
		/// 构造一个可执行动作项,参数全部使用空值
		/// </summary>
		public TaskActionItem()
			: this(string.Empty, string.Empty, string.Empty, string.Empty)
		{}

		/// <summary>
		/// 构造一个可执行动作项
		/// </summary>
		/// <param name="name">动作名称</param>
		/// <param name="actionUrl">执行动作的页面的url地址</param>
		/// <param name="actionScript">执行动作的javascript脚本</param>
		public TaskActionItem(string name, string actionUrl, string actionScript)
			: this(name, actionUrl, actionScript, string.Empty)
		{}

		/// <summary>
		/// 构造一个可执行动作项
		/// </summary>
		/// <param name="name">动作名称</param>
		/// <param name="actionUrl">执行动作的页面的url地址</param>
		/// <param name="actionScript">执行动作的javascript脚本</param>
		/// <param name="description">动作的描述</param>
		public TaskActionItem(string name, string actionUrl, string actionScript, string description)
		{
			this.text = name;
			this.actionUrl = actionUrl;
			this.actionScript = actionScript;
			this.description = description;
		}
		private string text;

		/// <summary>
		/// 任务名称(用于显示):在待办任务列表中显示的每一条待办的文本信息.
		/// </summary>
		public string Name
		{
			get { return text; }
			set { text = value; }
		}

		private string actionUrl;

		/// <summary>
		/// 执行任务的页面
		/// </summary>
		public string ActionUrl
		{
			get { return actionUrl; }
			set { actionUrl = value; }
		}

		private string actionScript;

		/// <summary>
		/// 动作的javascript脚本
		/// </summary>
		public string ActionScript
		{
			get { return actionScript; }
			set { actionScript = value; }
		}

		private string description;

		/// <summary>
		/// 动作描述
		/// </summary>
		public string Description
		{
			get { return description; }
			set { description = value; }
		}
	}
	/// <summary>
	/// 单条任务项,包含任务的相关信息以及可执行的动作列表,查看动作项等.
	/// </summary>
	public interface ITaskItem
	{
		/// <summary>
		/// Gets or sets 执行期限.
		/// </summary>
		/// <value>The exceed time.</value>
		TimeSpan ExceedTime { get;set;}
		/// <summary>
		/// Gets or sets 任务名称
		/// </summary>
		/// <value>The name of the task.</value>
		string TaskName { get;set;}
		/// <summary>
		/// Gets or sets可执行的动作集合
		/// </summary>
		/// <value>The action items.</value>
		List<ITaskActionItem> ActionItems { get; set; }
		/// <summary>
		/// Gets or sets 对应的被审批对象Id
		/// </summary>
		/// <value>The ea id.</value>
		int EaId { get; set; }
		/// <summary>
		/// Gets or sets 工作流实例id
		/// </summary>
		/// <value>The instance id.</value>
		Guid InstanceId { get; set; }
		/// <summary>
		/// Gets or sets 最后一步执行时间
		/// </summary>
		/// <value>The persist time.</value>
		DateTime PersistTime { get; set; }
		/// <summary>
		/// Gets or sets 查看 该任务详情的动作定义
		/// </summary>
		/// <value>The view action.</value>
		TaskActionItem ViewAction { get; set; }

		/// <summary>
		/// Gets or set审批对象的实例
		/// </summary>
		/// <value>The ea object.</value>
		object EaObject { get;set;}
	}
	/// <summary>
	/// 每一个工作流实例的任务项目
	/// </summary>
	public class TaskItem : ITaskItem
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TaskItem"/> class.
		/// </summary>
		public TaskItem()
		{
			actionItems = new List<ITaskActionItem>();
		}

		private TimeSpan exceedTime;

		/// <summary>
		/// Gets or sets the exceed time.
		/// </summary>
		/// <value>The exceed time.</value>
		public TimeSpan ExceedTime
		{
			get { return exceedTime; }
			set { exceedTime = value; }
		}

		private string taskName;

		/// <summary>
		/// Gets or sets the name of the task.
		/// </summary>
		/// <value>The name of the task.</value>
		public string TaskName
		{
			get { return taskName; }
			set { taskName = value; }
		}


		private Guid instanceId;
		/// <summary>
		/// 工作流实例的guid
		/// </summary>
		public Guid InstanceId
		{
			get { return instanceId; }
			set { instanceId = value; }
		}

		private int eaid;
		/// <summary>
		/// 工作流实例针对的立项id
		/// </summary>
		public int EaId
		{
			get { return eaid; }
			set { eaid = value; }
		}

		private object eaObject;

		/// <summary>
		/// Gets or sets 审批对象的实例
		/// </summary>
		/// <value>The EA object.</value>
		public object EaObject
		{
			get { return eaObject; }
			set { eaObject = value; }
		}


		private TaskActionItem viewAction;
		/// <summary>
		/// 查看动作
		/// </summary>
		public TaskActionItem ViewAction
		{
			get { return viewAction; }
			set { viewAction = value; }
		}

		private List<ITaskActionItem> actionItems;
		/// <summary>
		/// 操作动作项目
		/// </summary>
		public List<ITaskActionItem> ActionItems
		{
			get { return actionItems; }
			set { actionItems = value; }
		}

		private string applyUnit;
		/// <summary>
		/// 申报单位名称
		/// </summary>
		public string ApplyUnitName
		{
			get { return applyUnit; }
			set { applyUnit = value; }
		}

		private string applyLetterNumber;
		/// <summary>
		/// 申报文号
		/// </summary>
		public string ApplyLetterNumber
		{
			get { return applyLetterNumber; }
			set { applyLetterNumber = value; }
		}

		private string receivedLetterNumber;
		/// <summary>
		/// 接收文号
		/// </summary>
		public string ReceivedLetterNumber
		{
			get { return receivedLetterNumber; }
			set { receivedLetterNumber = value; }
		}

		private string currentState;
		/// <summary>
		/// 当前状态
		/// </summary>
		public string CurrentState
		{
			get { return currentState; }
			set { currentState = value; }
		}

		private DateTime persistTime;
		/// <summary>
		/// 持久化时间
		/// </summary>
		public DateTime PersistTime
		{
			get { return persistTime; }
			set { persistTime = value; }
		}
	}

	/// <summary>
	/// 某一类任务的任务列表
	/// </summary>
	public class TaskList:IComparable
	{
		/// <summary>
		/// 创建某一类动作的任务列表的实例
		/// </summary>
		/// <param name="category">类别</param>
		/// <param name="items">工作流实例集合</param>
		public TaskList(string category, List<ITaskItem> items)
		{
			this.category = category;
			this.items = items.ToArray();
		}

		private string category = "";
		/// <summary>
		/// 任务类别
		/// </summary>
		public string Cagegory
		{
			get { return category; }
			set { category = value; }
		}

		private ITaskItem[] items;
		/// <summary>
		/// 可以执行的工作流实例列表
		/// </summary>
		public ITaskItem[] Items
		{
			get { return items; }
			set { items = value; }
		}

		#region IComparable Members

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>
		/// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than <paramref name="obj"/>. Zero This instance is equal to <paramref name="obj"/>. Greater than zero This instance is greater than <paramref name="obj"/>.
		/// </returns>
		/// <exception cref="T:System.ArgumentException">
		/// 	<paramref name="obj"/> is not the same type as this instance. </exception>
		public int CompareTo(object obj)
		{
			TaskList item = obj as TaskList;
			return this.Cagegory.CompareTo(item.Cagegory);
		}

		#endregion
	}
}

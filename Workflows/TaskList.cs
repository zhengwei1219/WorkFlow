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
		/// ��������
		/// </summary>
		/// <value>The name.</value>
		string Name { get;}
		/// <summary>
		/// ��������
		/// </summary>
		/// <value>The description.</value>
		string Description { get;}
		/// <summary>
		/// ִ�иö�����url,��ActionScriptֻ��Ҫ��������֮һ
		/// </summary>
		/// <value>The action URL.</value>
		string ActionUrl { get;}
		/// <summary>
		/// ִ�иö�����Script�ű�,��ActionUrlֻ��Ҫ��������֮һ
		/// </summary>
		/// <value>The action script.</value>
		string ActionScript { get;}
	}
	/// <summary>
	/// ������Ŀ
	/// </summary>
	public class TaskActionItem : ITaskActionItem
	{
		/// <summary>
		/// ����һ����ִ�ж�����,����ȫ��ʹ�ÿ�ֵ
		/// </summary>
		public TaskActionItem()
			: this(string.Empty, string.Empty, string.Empty, string.Empty)
		{}

		/// <summary>
		/// ����һ����ִ�ж�����
		/// </summary>
		/// <param name="name">��������</param>
		/// <param name="actionUrl">ִ�ж�����ҳ���url��ַ</param>
		/// <param name="actionScript">ִ�ж�����javascript�ű�</param>
		public TaskActionItem(string name, string actionUrl, string actionScript)
			: this(name, actionUrl, actionScript, string.Empty)
		{}

		/// <summary>
		/// ����һ����ִ�ж�����
		/// </summary>
		/// <param name="name">��������</param>
		/// <param name="actionUrl">ִ�ж�����ҳ���url��ַ</param>
		/// <param name="actionScript">ִ�ж�����javascript�ű�</param>
		/// <param name="description">����������</param>
		public TaskActionItem(string name, string actionUrl, string actionScript, string description)
		{
			this.text = name;
			this.actionUrl = actionUrl;
			this.actionScript = actionScript;
			this.description = description;
		}
		private string text;

		/// <summary>
		/// ��������(������ʾ):�ڴ��������б�����ʾ��ÿһ��������ı���Ϣ.
		/// </summary>
		public string Name
		{
			get { return text; }
			set { text = value; }
		}

		private string actionUrl;

		/// <summary>
		/// ִ�������ҳ��
		/// </summary>
		public string ActionUrl
		{
			get { return actionUrl; }
			set { actionUrl = value; }
		}

		private string actionScript;

		/// <summary>
		/// ������javascript�ű�
		/// </summary>
		public string ActionScript
		{
			get { return actionScript; }
			set { actionScript = value; }
		}

		private string description;

		/// <summary>
		/// ��������
		/// </summary>
		public string Description
		{
			get { return description; }
			set { description = value; }
		}
	}
	/// <summary>
	/// ����������,��������������Ϣ�Լ���ִ�еĶ����б�,�鿴�������.
	/// </summary>
	public interface ITaskItem
	{
		/// <summary>
		/// Gets or sets ִ������.
		/// </summary>
		/// <value>The exceed time.</value>
		TimeSpan ExceedTime { get;set;}
		/// <summary>
		/// Gets or sets ��������
		/// </summary>
		/// <value>The name of the task.</value>
		string TaskName { get;set;}
		/// <summary>
		/// Gets or sets��ִ�еĶ�������
		/// </summary>
		/// <value>The action items.</value>
		List<ITaskActionItem> ActionItems { get; set; }
		/// <summary>
		/// Gets or sets ��Ӧ�ı���������Id
		/// </summary>
		/// <value>The ea id.</value>
		int EaId { get; set; }
		/// <summary>
		/// Gets or sets ������ʵ��id
		/// </summary>
		/// <value>The instance id.</value>
		Guid InstanceId { get; set; }
		/// <summary>
		/// Gets or sets ���һ��ִ��ʱ��
		/// </summary>
		/// <value>The persist time.</value>
		DateTime PersistTime { get; set; }
		/// <summary>
		/// Gets or sets �鿴 ����������Ķ�������
		/// </summary>
		/// <value>The view action.</value>
		TaskActionItem ViewAction { get; set; }

		/// <summary>
		/// Gets or set���������ʵ��
		/// </summary>
		/// <value>The ea object.</value>
		object EaObject { get;set;}
	}
	/// <summary>
	/// ÿһ��������ʵ����������Ŀ
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
		/// ������ʵ����guid
		/// </summary>
		public Guid InstanceId
		{
			get { return instanceId; }
			set { instanceId = value; }
		}

		private int eaid;
		/// <summary>
		/// ������ʵ����Ե�����id
		/// </summary>
		public int EaId
		{
			get { return eaid; }
			set { eaid = value; }
		}

		private object eaObject;

		/// <summary>
		/// Gets or sets ���������ʵ��
		/// </summary>
		/// <value>The EA object.</value>
		public object EaObject
		{
			get { return eaObject; }
			set { eaObject = value; }
		}


		private TaskActionItem viewAction;
		/// <summary>
		/// �鿴����
		/// </summary>
		public TaskActionItem ViewAction
		{
			get { return viewAction; }
			set { viewAction = value; }
		}

		private List<ITaskActionItem> actionItems;
		/// <summary>
		/// ����������Ŀ
		/// </summary>
		public List<ITaskActionItem> ActionItems
		{
			get { return actionItems; }
			set { actionItems = value; }
		}

		private string applyUnit;
		/// <summary>
		/// �걨��λ����
		/// </summary>
		public string ApplyUnitName
		{
			get { return applyUnit; }
			set { applyUnit = value; }
		}

		private string applyLetterNumber;
		/// <summary>
		/// �걨�ĺ�
		/// </summary>
		public string ApplyLetterNumber
		{
			get { return applyLetterNumber; }
			set { applyLetterNumber = value; }
		}

		private string receivedLetterNumber;
		/// <summary>
		/// �����ĺ�
		/// </summary>
		public string ReceivedLetterNumber
		{
			get { return receivedLetterNumber; }
			set { receivedLetterNumber = value; }
		}

		private string currentState;
		/// <summary>
		/// ��ǰ״̬
		/// </summary>
		public string CurrentState
		{
			get { return currentState; }
			set { currentState = value; }
		}

		private DateTime persistTime;
		/// <summary>
		/// �־û�ʱ��
		/// </summary>
		public DateTime PersistTime
		{
			get { return persistTime; }
			set { persistTime = value; }
		}
	}

	/// <summary>
	/// ĳһ������������б�
	/// </summary>
	public class TaskList:IComparable
	{
		/// <summary>
		/// ����ĳһ�ද���������б��ʵ��
		/// </summary>
		/// <param name="category">���</param>
		/// <param name="items">������ʵ������</param>
		public TaskList(string category, List<ITaskItem> items)
		{
			this.category = category;
			this.items = items.ToArray();
		}

		private string category = "";
		/// <summary>
		/// �������
		/// </summary>
		public string Cagegory
		{
			get { return category; }
			set { category = value; }
		}

		private ITaskItem[] items;
		/// <summary>
		/// ����ִ�еĹ�����ʵ���б�
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

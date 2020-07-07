using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;


namespace OilDigital.Workflows
{
	/// <summary>
	/// 审批状态
	/// </summary>
    [Serializable]
	[XmlInclude(typeof(InvokeWorkflowState))]
    public class ApprovalState
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="ApprovalState"/> class.
		/// </summary>
		public ApprovalState()
		{
		}
		private string name;
		/// <summary>
		/// 名称
		/// </summary>
		[XmlAttribute()]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		private string description;
		/// <summary>
		/// 说明
		/// </summary>
		[XmlAttribute()]
		public string Description
		{
			get { return description; }
			set { description = value; }
		}

		private bool allowedCancel = true;
		/// <summary>
		/// 是否允许撤销
		/// </summary>
		[XmlAttribute()]
		public bool AllowedCancel
		{
			get { return allowedCancel; }
			set { allowedCancel = value; }
		}

		private bool isApprovalState = true;
		/// <summary>
		/// 是否是审批状态,如果为false,那么在提取任务时,处于该状态的实例将不会被提取到任务列表,缺省为true
		/// </summary>
		[XmlAttribute()]
		public bool IsApprovalState
		{
			get { return isApprovalState; }
			set { isApprovalState = value; }
		}

		/// <summary>
		/// 允许出发的时间集合
		/// </summary>
		[XmlArray("Events")]
		[XmlArrayItem("ApprovalEvent")]
		public ApprovalEvent[] Events
		{
			get
			{
				List<ApprovalEvent> list = new List<ApprovalEvent>();
				foreach (string key in this.eventList.Keys)
				{
					list.Add(eventList[key]);
				}
				return list.ToArray();
			}
			set
			{
				foreach (ApprovalEvent approvalEvent in value)
				{
					eventList.Add(approvalEvent.Name, approvalEvent);
				}
			}
		}

		private Dictionary<string, ApprovalEvent> eventList = new Dictionary<string, ApprovalEvent>();
		/// <summary>
		/// 获取该状态下的事件
		/// </summary>
		/// <param name="name">事件名称</param>
		/// <returns></returns>
		public ApprovalEvent GetEventByName(string name)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (!eventList.ContainsKey(name))
				throw new ArgumentException("event name:" + name + " must in keys", "name");
			return eventList[name];
		}
    }
	/// <summary>
	/// 调用子工作流的状态
	/// </summary>
	[Serializable()]
	public class InvokeWorkflowState : ApprovalState
	{
		private string invokingWorkflowName;
		/// <summary>
		/// 启动子流程的的名称，为空则不启动子流程
		/// </summary>
		[XmlAttribute()]
		public string InvokingWorkflowName
		{
			get { return invokingWorkflowName; }
			set { invokingWorkflowName = value; }
		}

		private string completedTransitionState;
		/// <summary>
		/// 子流程结束后跳转到的状态
		/// </summary>
		[XmlAttribute()]
		public string CompletedTransitionState
		{
			get { return completedTransitionState; }
			set { completedTransitionState = value; }
		}

		private string abortTransitionState;
		/// <summary>
		/// 子流程中止时跳转到的状态
		/// </summary>
		[XmlAttribute()]
		public string AbortTransitionState
		{
			get { return abortTransitionState; }
			set { abortTransitionState = value; }
		}
	}
}



using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;


namespace OilDigital.Workflows
{
	/// <summary>
	/// ����״̬
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
		/// ����
		/// </summary>
		[XmlAttribute()]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		private string description;
		/// <summary>
		/// ˵��
		/// </summary>
		[XmlAttribute()]
		public string Description
		{
			get { return description; }
			set { description = value; }
		}

		private bool allowedCancel = true;
		/// <summary>
		/// �Ƿ�������
		/// </summary>
		[XmlAttribute()]
		public bool AllowedCancel
		{
			get { return allowedCancel; }
			set { allowedCancel = value; }
		}

		private bool isApprovalState = true;
		/// <summary>
		/// �Ƿ�������״̬,���Ϊfalse,��ô����ȡ����ʱ,���ڸ�״̬��ʵ�������ᱻ��ȡ�������б�,ȱʡΪtrue
		/// </summary>
		[XmlAttribute()]
		public bool IsApprovalState
		{
			get { return isApprovalState; }
			set { isApprovalState = value; }
		}

		/// <summary>
		/// ���������ʱ�伯��
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
		/// ��ȡ��״̬�µ��¼�
		/// </summary>
		/// <param name="name">�¼�����</param>
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
	/// �����ӹ�������״̬
	/// </summary>
	[Serializable()]
	public class InvokeWorkflowState : ApprovalState
	{
		private string invokingWorkflowName;
		/// <summary>
		/// ���������̵ĵ����ƣ�Ϊ��������������
		/// </summary>
		[XmlAttribute()]
		public string InvokingWorkflowName
		{
			get { return invokingWorkflowName; }
			set { invokingWorkflowName = value; }
		}

		private string completedTransitionState;
		/// <summary>
		/// �����̽�������ת����״̬
		/// </summary>
		[XmlAttribute()]
		public string CompletedTransitionState
		{
			get { return completedTransitionState; }
			set { completedTransitionState = value; }
		}

		private string abortTransitionState;
		/// <summary>
		/// ��������ֹʱ��ת����״̬
		/// </summary>
		[XmlAttribute()]
		public string AbortTransitionState
		{
			get { return abortTransitionState; }
			set { abortTransitionState = value; }
		}
	}
}



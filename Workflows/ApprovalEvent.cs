using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace OilDigital.Workflows
{
	/// <summary>
	/// �����¼���
	/// </summary>
	[Serializable]
	public class ApprovalEvent
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ApprovalEvent"/> class.
		/// </summary>
		public ApprovalEvent(){}

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
		/// ����
		/// </summary>
		[XmlAttribute()]
		public string Description
		{
			get { return description; }
			set { description = value; }
		}

		private List<EventRole> roles;
		/// <summary>
		/// �����¼��Ľ�ɫ����
		/// </summary>
		[XmlArray("Roles")]
		[XmlArrayItem("EventRole")]
		public List<EventRole> Roles
		{
			get { return roles; }
			set { roles = value; }
		}

		private string authorization = "All";
		/// <summary>
		/// ���������¼���Ȩ���޶�
		/// </summary>
		[XmlAttribute()]
		public string Authorization
		{
			get { return authorization; }
			set { authorization = value; }
		}

		private string bindActivity;
		/// <summary>
		/// Gets or sets the bind activity.
		/// </summary>
		/// <value>The bind activity.</value>
		[XmlAttribute()]
		public string BindActivity
		{
			get { return bindActivity; }
			set { bindActivity = value; }
		}
	
		/// <summary>
		/// �ֺŷָ�����һ��״̬�б�
		/// </summary>
		[XmlAttribute("NextStateNames")]
		public string NextStateNamesString
		{
			get 
			{
				StringBuilder sb = new StringBuilder();
				foreach (string stateName in this.nextStateNames)
				{
					sb.Append(stateName + ";");
				}
				return sb.ToString().Trim(';');
			}
			set 
			{
				if (value == null)
					throw new ArgumentNullException("NextStateNamesString");
				nextStateNames = value.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				if (nextStateNames.Length == 0)
					nextStateNames = new string[1] { "" };
			}
		}

		private string[] nextStateNames;
		/// <summary>
		/// ��һ��״̬���Ƽ���
		/// </summary>
		[XmlIgnore()]
		public string[] NextStateNames
		{
			get { return nextStateNames; }
		}

		private ApprovalEventParameter parameter = ApprovalEventParameter.Empty;
		/// <summary>
		/// Gets or sets �ʹ������¼���ص��ⲿ���ò���
		/// </summary>
		/// <value>The prameter.</value>
		[XmlElement("Parameter")]
		public ApprovalEventParameter Parameter
		{
			get { return parameter; }
			set { parameter = value; }
		}
	}

	/// <summary>
	/// ʱ�������
	/// </summary>
	[Serializable()]
	public class ApprovalEventParameter
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ApprovalEventParameter"/> class.
		/// </summary>
		public ApprovalEventParameter() { }
		private bool needCheckValid = false;
		/// <summary>
		/// Gets or sets �Ƿ���ִ�ж���֮ǰ��У���������Ч��
		/// </summary>
		/// <value><c>true</c> if [need check valid]; otherwise, <c>false</c>.</value>
		[XmlAttribute("NeedCheckValid")]
		public bool NeedCheckValid
		{
			get { return needCheckValid; }
			set { needCheckValid = value; }
		}

		private bool emptySolutionAllowed = true;
		/// <summary>
		/// Gets or sets �Ƿ����������������,default=true
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [empty solution allowed]; otherwise, <c>false</c>.
		/// </value>
		[XmlAttribute("EmptySolutionAllowed")]
		public bool EmptySolutionAllowed
		{
			get { return emptySolutionAllowed; }
			set { emptySolutionAllowed = value; }
		}

		private string defaultSolutionInfo = "";
		/// <summary>
		/// Gets or sets ������������ȱʡ��������.
		/// </summary>
		/// <value>The default solution info.</value>
		[XmlAttribute("DefaultSolutionInfo")]
		public string DefaultSolutionInfo
		{
			get { return defaultSolutionInfo; }
			set { defaultSolutionInfo = value; }
		}

		private MsgRemind reminder = MsgRemind.None;
		/// <summary>
		/// Gets or sets �������Ѳ᷽ʽ
		/// </summary>
		/// <value>The reminder.</value>
		[XmlAttribute("Reminder")]
		public MsgRemind Reminder
		{
			get { return reminder; }
			set { reminder = value; }
		}

		private static ApprovalEventParameter empty = new ApprovalEventParameter();
		/// <summary>
		/// Gets the empty.
		/// </summary>
		/// <value>The empty.</value>
		public static ApprovalEventParameter Empty
		{
			get { return empty; }
		}
	}
	/// <summary>
	/// ����Ϣ���ѷ�ʽ
	/// </summary>
	[Flags]
	public enum MsgRemind
	{
		/// <summary>
		/// ��
		/// </summary>
		None = 0,
		/// <summary>
		/// ֻ��������
		/// </summary>
		Owner = 1,
		/// <summary>
		/// ֻ�в�����
		/// </summary>
		Processor = 2,
		/// <summary>
		/// ����
		/// </summary>
		All = 3
	}
}

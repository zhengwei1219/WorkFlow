using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Collections.Specialized;
using System.Xml;

namespace OilDigital.Workflows
{
	/// <summary>
	/// ������ִ�к��¼�����
	/// </summary>
	public class ActivityExecutedArg : EventArgs
	{
		private WorkflowInstance instance;

		/// <summary>
		/// ִ�иö����Ĺ�����ʵ��
		/// </summary>
		/// <value>The instance.</value>
		public WorkflowInstance Instance
		{
			get { return instance; }
			set { instance = value; }
		}

		private ActivityExecutionResult result;

		/// <summary>
		/// ����ִ�еĽ��.
		/// </summary>
		/// <value>The result.</value>
		public ActivityExecutionResult Result
		{
			get { return result; }
			set { result = value; }
		}

		/// <summary>
		///����һ������ִ������¼���������
		/// </summary>
		/// <param name="instance">ִ�иĶ����Ĺ�����ʵ��</param>
		/// <param name="result">�¼�ִ�еĽ��</param>
		public ActivityExecutedArg(WorkflowInstance instance, ActivityExecutionResult result)
		{
			this.instance = instance;
			this.result = result;
		}
	}

	/// <summary>
	/// ��������������ʱ�Ĳ���
	/// </summary>
	public class OnInstanceEndArg : EventArgs
	{
		private int groupId;

		/// <summary>
		/// Gets the ea id.
		/// </summary>
		/// <value>The ea id.</value>
		public int EaId
		{
			get { return groupId; }
		}

		private Activity lastActivity;

		/// <summary>
		/// Gets ��һ���Ļʵ��
		/// </summary>
		/// <value>The last activity.</value>
		public Activity LastActivity
		{
			get { return lastActivity; }
		}

		private ApprovalState lastState;

		/// <summary>
		/// Gets ��һ����״̬ʵ��
		/// </summary>
		/// <value>The last state.</value>
		public ApprovalState LastState
		{
			get { return lastState; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OnInstanceEndArg"/> class.
		/// </summary>
		public OnInstanceEndArg()
		{

		}
		/// <summary>
		/// Initializes a new instance of the <see cref="OnInstanceEndArg"/> class.
		/// </summary>
		public OnInstanceEndArg(int eaid,Activity lastActivity,ApprovalState lastState)
		{
			this.groupId = eaid;
			this.lastActivity = lastActivity;
			this.lastState = lastState;
		}
	}

}
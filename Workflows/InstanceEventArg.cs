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
	/// 工作流执行后事件参数
	/// </summary>
	public class ActivityExecutedArg : EventArgs
	{
		private WorkflowInstance instance;

		/// <summary>
		/// 执行该动作的工作流实例
		/// </summary>
		/// <value>The instance.</value>
		public WorkflowInstance Instance
		{
			get { return instance; }
			set { instance = value; }
		}

		private ActivityExecutionResult result;

		/// <summary>
		/// 动作执行的结果.
		/// </summary>
		/// <value>The result.</value>
		public ActivityExecutionResult Result
		{
			get { return result; }
			set { result = value; }
		}

		/// <summary>
		///创建一个动作执行完毕事件参数对象
		/// </summary>
		/// <param name="instance">执行改动做的工作流实例</param>
		/// <param name="result">事件执行的结果</param>
		public ActivityExecutedArg(WorkflowInstance instance, ActivityExecutionResult result)
		{
			this.instance = instance;
			this.result = result;
		}
	}

	/// <summary>
	/// 当审批正常结束时的参数
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
		/// Gets 上一步的活动实例
		/// </summary>
		/// <value>The last activity.</value>
		public Activity LastActivity
		{
			get { return lastActivity; }
		}

		private ApprovalState lastState;

		/// <summary>
		/// Gets 上一步的状态实例
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
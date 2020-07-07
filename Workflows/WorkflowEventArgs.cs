using System;
using System.Data;
using System.Configuration;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 工作流时间参数类
	/// </summary>
	public class WorkflowEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="WorkflowEventArgs"/> class.
		/// </summary>
		public WorkflowEventArgs()
			: base()
		{ }
		/// <summary>
		/// Initializes a new instance of the <see cref="WorkflowEventArgs"/> class.
		/// </summary>
		/// <param name="instance">The instance.</param>
		public WorkflowEventArgs(WorkflowInstance instance)
		{
			this.instance = instance;
		}
		private WorkflowInstance instance;
		/// <summary>
		/// 事件参数实例的Id
		/// </summary>
		/// <value>The instance id.</value>
		public WorkflowInstance Instance
		{
			get { return this.instance; }
		}
	}
	/// <summary>
	/// 
	/// </summary>
	public class StateChangedEventArgs : WorkflowEventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="StateChangedEventArgs"/> class.
		/// </summary>
		public StateChangedEventArgs() { }
		/// <summary>
		/// Initializes a new instance of the <see cref="StateChangedEventArgs"/> class.
		/// </summary>
		/// <param name="instance">The instance.</param>
		public StateChangedEventArgs(WorkflowInstance instance)
			: base(instance)
		{ }
		/// <summary>
		/// Initializes a new instance of the <see cref="StateChangedEventArgs"/> class.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="originalState">State of the original.</param>
		/// <param name="currentState">State of the current.</param>
		public StateChangedEventArgs(WorkflowInstance instance, string originalState, string currentState)
			: base(instance)
		{
			this.originalState = originalState;
			this.currentState = currentState;
		}
		private string originalState;
		/// <summary>
		/// Gets the state of the original.
		/// </summary>
		/// <value>The state of the original.</value>
		public string OriginalState
		{
			get { return this.originalState; }
		}
		private string currentState;
		/// <summary>
		/// Gets the state of the current.
		/// </summary>
		/// <value>The state of the current.</value>
		public string CurrentState
		{
			get { return this.currentState; }
		}
	}
}

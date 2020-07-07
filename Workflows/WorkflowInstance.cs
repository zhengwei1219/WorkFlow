using System;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public abstract class WorkflowInstance
	{
		/// <summary>
		/// Gets or sets the workflow.
		/// </summary>
		/// <value>The workflow.</value>
		public abstract WorkFlowDefine Workflow { get;}
		/// <summary>
		/// Gets the name of the workflow.
		/// </summary>
		/// <value>The name of the workflow.</value>
		public abstract string WorkflowName { get; set;}
		/// <summary>
		/// Gets the current status.
		/// </summary>
		/// <value>The current status.</value>
		public abstract WorkflowExecutionStatus CurrentStatus { get; }
		/// <summary>
		/// 工作流实例相关的集合
		/// </summary>
		public abstract NameValueCollection Properties { get;set;}
		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		/// <value>The id.</value>
		public abstract Guid Id { get;set; }
		/// <summary>
		/// Gets or sets the ea id.
		/// </summary>
		/// <value>The ea id.</value>
		public abstract int EaId { get; set;}
		/// <summary>
		/// Gets or sets the external obj.
		/// </summary>
		/// <value>The external obj.</value>
		public abstract ICanBeApproval ExternalObj { get;set;}
		/// <summary>
		/// Gets or sets the parent id.
		/// </summary>
		/// <value>The parent id.</value>
		public abstract Guid ParentId { get; set; }
		/// <summary>
		/// Gets or sets the persist time.
		/// </summary>
		/// <value>The persist time.</value>
		public abstract DateTime PersistTime { get;set;}
		/// <summary>
		/// Gets or sets the children id.
		/// </summary>
		/// <value>The children id.</value>
		public abstract List<Guid> ChildrenId { get; set; }
		/// <summary>
		/// Internals the execute.
		/// </summary>
		/// <param name="activity">The activity.</param>
		internal void InternalExecute(Activity activity)
		{
			ExecuteActivity(activity);
		}
		/// <summary>
		/// Executes the activity.
		/// </summary>
		/// <param name="activity">The activity.</param>
		protected abstract void ExecuteActivity(Activity activity);
		/// <summary>
		/// Starts this instance.
		/// </summary>
		public abstract void Start();
		/// <summary>
		/// Internals the undo.
		/// </summary>
		internal void InternalUndo()
		{
			Undo();
		}
		/// <summary>
		/// Undoes this instance.
		/// </summary>
		protected abstract void Undo();
	}
}

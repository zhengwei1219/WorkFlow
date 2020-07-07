using System;
using System.Collections.Generic;
using System.Text;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 工作流执行异常
	/// </summary>
	public class WorkflowExecuteExeception : ApplicationException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="WorkflowExecuteExeception"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		public WorkflowExecuteExeception(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WorkflowExecuteExeception"/> class.
		/// </summary>
		public WorkflowExecuteExeception() : base() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="WorkflowExecuteExeception"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="innerException">The inner exception.</param>
		public WorkflowExecuteExeception(string message, Exception innerException)
			: base(message, innerException)
		{ }
	}
}

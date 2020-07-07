using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;

namespace OilDigital.Workflows
{

	/// <summary>
	/// ��ȡ�û����������
	/// </summary>
	public class DrillApprovalSolution : DrillApprovalInfo<ApprovalSolution>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DrillApprovalSolution"/> class.
		/// </summary>
		public DrillApprovalSolution() : this(false, null) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="DrillApprovalSolution"/> class.
		/// </summary>
		/// <param name="hidden">The hidden.</param>
		public DrillApprovalSolution(ApprovalSolution hidden)
			: this(true,hidden)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DrillApprovalSolution"/> class.
		/// </summary>
		/// <param name="showFiltered">if set to <c>true</c> [show filtered].</param>
		/// <param name="hidden">The hidden.</param>
		public DrillApprovalSolution(bool showFiltered, ApprovalSolution hidden)
			: base(showFiltered, hidden)
		{
		}

		/// <summary>
		/// ��ȡȫ��������Ϣ�ķ���,���뱻������������
		/// </summary>
		/// <param name="applicationName">Name of the application.</param>
		/// <param name="eaid">The eaid.</param>
		/// <returns></returns>
		protected override List<ApprovalSolution> GetAllApprovalInfoById(string applicationName, int eaid)
		{
			string[] workflowNames = WorkflowRuntime.Current.DefineService.GetAllWorkflowDefineName(applicationName);
			List<ApprovalSolution> solutions = new List<ApprovalSolution>();
			foreach (string workflowName in workflowNames)
			{
				solutions.AddRange(approvalService.GetSolution(workflowName, eaid));
			}
			solutions.Sort();
			return solutions;
		}
	}
}

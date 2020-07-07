using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;

namespace OilDigital.Workflows
{
	
	/// <summary>
	/// ��ȡ�û���������¼
	/// </summary>
	public class DrillApprovalRecord : DrillApprovalInfo<ApprovalRecord>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DrillApprovalRecord"/> class.
		/// </summary>
		public DrillApprovalRecord() : this( false, null) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="DrillApprovalRecord"/> class.
		/// </summary>
		/// <param name="hidden">The hidden.</param>
		public DrillApprovalRecord( ApprovalRecord hidden)
			: this(true,hidden)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DrillApprovalRecord"/> class.
		/// </summary>
		/// <param name="showFiltered">if set to <c>true</c> [show filtered].</param>
		/// <param name="hidden">The hidden.</param>
		public DrillApprovalRecord(bool showFiltered, ApprovalRecord hidden)
			: base(showFiltered, hidden)
		{
		}

		/// <summary>
		/// ��ȡȫ��������Ϣ�ķ���,���뱻������������
		/// </summary>
		/// <param name="applicationName">Name of the application.</param>
		/// <param name="eaid">The eaid.</param>
		/// <returns></returns>
		protected override List<ApprovalRecord> GetAllApprovalInfoById(string applicationName, int eaid)
		{
			string[] workflowNames = WorkflowRuntime.Current.DefineService.GetAllWorkflowDefineName(applicationName);
			List<ApprovalRecord> records = new List<ApprovalRecord>();
			foreach (string workflowName in workflowNames)
			{
				records.AddRange(approvalService.GetRecord(workflowName, eaid));
			}
			records.Sort();
			return records;
		}
	}
}

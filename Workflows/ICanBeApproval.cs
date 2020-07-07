using System;
using System.Collections.Generic;
using System.Text;

namespace OilDigital.Workflows
{

	/// <summary>
	/// 审批状态（大状态）
	/// </summary>
	public class ApprovalStatusCode
	{
		/// <summary>
		/// 正常状态，没有提交,即为草稿状态.
		/// </summary>
		public const string Normal = "0";

		/// <summary>
		/// 已经提交但是没有接收；
		/// </summary>
		public const string Submitted = "1";

		/// <summary>
		/// 已经接收
		/// </summary>
		public const string Received = "2";

		/// <summary>
		/// 审批通过
		/// </summary>
		public const string Approved = "3";

		/// <summary>
		/// 审批终止
		/// </summary>
		public const string Terminated = "4";

		/// <summary>
		/// 审批反馈,退回状态
		/// </summary>
		public const string Retrial = "5";
	}

	/// <summary>
	/// 可以被审批流程进行审批的接口
	/// </summary>
	public interface ICanBeApproval
	{
		/// <summary>
		/// 获取被审批对象的Id
		/// </summary>
		/// <returns></returns>
		string GetID();

		/// <summary>
		/// 获取被审批对象的粗略状态.
		/// </summary>
		/// <returns></returns>
		string GetStatus();

		/// <summary>
		/// 设置或者获得接收时间,接收时间用于记录被审批对象何时从"草稿"变成正式.
		/// </summary>
		/// <value>The received time.</value>
		DateTime? ReceivedTime { get;set;}

		/// <summary>
		/// Gets or sets 审批结束的时间,审批结束包括正常审批(同意),也可能是被中止的时间
		/// </summary>
		/// <value>The approval end time.</value>
		DateTime? ApprovalEndTime { get;set;}

		/// <summary>
		/// 设置审批状态,可以审批对象的状态可以使<see cref="ApprovalStatusCode"/>中定义的状态代码,也可以是自定义的字符串 
		/// </summary>
		/// <param name="approvalStatusCode">审批状态,可以是</param>
		void SetStatus(string approvalStatusCode);
	}

	/// <summary>
	/// 可以在审批时保存审批前原始值的接口.
	/// 有些审批流程中需要评估被审批对象的审批效果,因此需要将被审批对象在审批流程前的数据另外保存起来.
	/// </summary>
	public interface IKeepOriginalValues
	{
		/// <summary>
		/// 保存原始值:将关键数据的原始数据保存.
		/// </summary>
		void KeepOriginalValues();

		/// <summary>
		/// 清除原始值,将保留的原始数据清除掉.
		/// </summary>
		void ClearOriginalValues();
	}
}

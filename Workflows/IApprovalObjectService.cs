using System;
using System.Collections.Generic;
using System.Text;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 用于对审批对象进行操作的服务
	/// <remarks>在审批过程中除了流程本身以及流程相关表有数据修改外,可能需要操作被审批对象,比如修改审批对象的状态,删除审批对象等.</remarks>
	/// </summary>
	public interface IApprovalObjectService
	{
		/// <summary>
		/// 根据ID获取审批对象
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		ICanBeApproval GetByID(string id);

		/// <summary>
		/// 保存审批对象的改动.
		/// </summary>
		/// <param name="obj">The obj.</param>
		void Save(ICanBeApproval obj);


		/// <summary>
		/// 删除被审批对象.
		/// </summary>
		/// <param name="obj">The obj.</param>
		void Delete(ICanBeApproval obj);
	}
}

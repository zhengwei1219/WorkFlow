using System;
using System.Collections.Generic;
using System.Text;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 审批规则接口
	/// </summary>
	public interface IApprovalRules
	{
		/// <summary>
		/// 审批角色
		/// </summary>
		ApprovalRole UserRole { get;}
		/// <summary>
		/// 审批角色是启动工作流的角色
		/// </summary>
		bool IsCreator();
		/// <summary>
		/// 角色是否是二级单位角色
		/// </summary>
		bool IsSubUnitRole();
		/// <summary>
		/// 判断审批角色是否是审批负责单位角色
		/// </summary>
		bool IsApprovalDepRole();
		/// <summary>
		/// 角色是否是审批角色
		/// </summary>
		bool IsApprovalRole();
		/// <summary>
		/// 是否是结束审批角色
		/// </summary>
		bool IsFinisher();
	}
}

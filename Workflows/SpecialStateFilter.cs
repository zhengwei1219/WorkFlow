using System;
using System.Collections.Generic;
using System.Text;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 按状态名进行过滤,StateNames中定义需要过滤掉的状态名,多个名称用分号分割
	/// 在approval.config中配置filter为
	/// <remarks>
	/// <![CDATA[ <filter type="OilDigital.Workflows.SpecialStateFilter,OilDigital.Workflows" >
	///		<param name="StateNames" value="state1;state2....."></param>
	///	</filter>]]>
	/// </remarks>
	/// </summary>
	public class SpecialStateFilter : InstanceFilter
	{
		private string stateNames;
		/// <summary>
		/// 需要过滤的状态，状态匹配的将不再显示在列表中。用分号分割多个。
		/// </summary>
		/// <value>
		/// The states.
		/// </value>
		public string StateNames
		{
			get { return this.stateNames; }
			set { this.stateNames = value; }
		}
		/// <summary>
		/// 判断指定实例是否需要满足过滤条件，满足则返回True，否则返回False
		/// </summary>
		/// <param name="instance">工作流实例</param>
		/// <param name="role">用户的审批角色</param>
		/// <param name="userIdentity">用户的身份</param>
		/// <returns>满足则返回True，否则返回False</returns>
		protected override bool IsMatch(StateMachineWorkflowInstance instance, ApprovalRole role, IUserIdentity userIdentity)
		{
			if (string.IsNullOrEmpty(this.StateNames))
				throw new ApplicationException("请配置需要过滤的状态名");
			string[] stateList = this.StateNames.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			foreach (string one in stateList)
			{
				if (instance.CurrentState.Name.Equals(one.Trim(), StringComparison.OrdinalIgnoreCase))
					return true;
			}
			return false;
		}
	}

	/// <summary>
	/// 过滤掉未接收的护照申请.
	/// </summary>
	public class WSJProcessingPassortApplyFilter : InstanceFilter
	{
		/// <summary>
		/// 判断指定实例是否需要满足过滤条件，满足则返回True，否则返回False
		/// </summary>
		/// <param name="instance">工作流实例</param>
		/// <param name="role">用户的审批角色</param>
		/// <param name="userIdentity">用户的身份</param>
		/// <returns>满足则返回True，否则返回False</returns>
		protected override bool IsMatch(StateMachineWorkflowInstance instance, ApprovalRole role, IUserIdentity userIdentity)
		{
			return instance.CurrentState.Name == "Receiving" || instance.CurrentState.Name == "HandoutApplyPassport";
		}
	}
}

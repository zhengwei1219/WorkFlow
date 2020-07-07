using System;
using System.Collections.Generic;
using System.Text;

namespace OilDigital.Workflows
{
	/// <summary>
	/// ��״̬�����й���,StateNames�ж�����Ҫ���˵���״̬��,��������÷ֺŷָ�
	/// ��approval.config������filterΪ
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
		/// ��Ҫ���˵�״̬��״̬ƥ��Ľ�������ʾ���б��С��÷ֺŷָ�����
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
		/// �ж�ָ��ʵ���Ƿ���Ҫ������������������򷵻�True�����򷵻�False
		/// </summary>
		/// <param name="instance">������ʵ��</param>
		/// <param name="role">�û���������ɫ</param>
		/// <param name="userIdentity">�û������</param>
		/// <returns>�����򷵻�True�����򷵻�False</returns>
		protected override bool IsMatch(StateMachineWorkflowInstance instance, ApprovalRole role, IUserIdentity userIdentity)
		{
			if (string.IsNullOrEmpty(this.StateNames))
				throw new ApplicationException("��������Ҫ���˵�״̬��");
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
	/// ���˵�δ���յĻ�������.
	/// </summary>
	public class WSJProcessingPassortApplyFilter : InstanceFilter
	{
		/// <summary>
		/// �ж�ָ��ʵ���Ƿ���Ҫ������������������򷵻�True�����򷵻�False
		/// </summary>
		/// <param name="instance">������ʵ��</param>
		/// <param name="role">�û���������ɫ</param>
		/// <param name="userIdentity">�û������</param>
		/// <returns>�����򷵻�True�����򷵻�False</returns>
		protected override bool IsMatch(StateMachineWorkflowInstance instance, ApprovalRole role, IUserIdentity userIdentity)
		{
			return instance.CurrentState.Name == "Receiving" || instance.CurrentState.Name == "HandoutApplyPassport";
		}
	}
}

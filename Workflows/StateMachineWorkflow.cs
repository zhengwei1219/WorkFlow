using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace OilDigital.Workflows
{
	/// <summary>
	/// ״̬������������
	/// </summary>
	[Serializable]
	public class StateMachineWorkflow : WorkFlowDefine
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="StateMachineWorkflow"/> class.
		/// </summary>
		public StateMachineWorkflow(){}

		private string initState;
		/// <summary>
		/// ״̬����ʼ״̬,��״̬������һ�δ���ʱ��״̬����
		/// </summary>
		[XmlAttribute()]
		public string InitState
		{
			get { return initState; }
			set { initState = value; }
		}

		private string endState;
		/// <summary>
		/// ״̬���Ľ���״̬����
		/// </summary>
		[XmlAttribute()]
		public string EndState
		{
			get { return endState; }
			set { endState = value; }
		}

		private string terminateState;
		/// <summary>
		/// ���̵���ֹ״̬
		/// </summary>
		[XmlAttribute()]
		public string TerminateState
		{
			get { return terminateState; }
			set { terminateState = value; }
		}

		private ApprovalState[] states;
		/// <summary>
		/// ״̬����������״̬����
		/// </summary>
		/// <value>The states.</value>
		[XmlArray("States")]
		[XmlArrayItem("ApprovalState")]
		public ApprovalState[] States
		{
			get { return states; }
			set
			{
				states = value;
				foreach (ApprovalState state in value)
				{
					statesList.Add(state.Name, state);
				}
			}
		}
		[NonSerialized()]
		private Dictionary<string, ApprovalState> statesList = new Dictionary<string, ApprovalState>();
		private ApprovalRole[] roles;
		/// <summary>
		/// ����ʹ�øù����������н�ɫRole
		/// </summary>
		[XmlArray("Roles")]
		[XmlArrayItem("ApprovalRole")]
		public ApprovalRole[] Roles
		{
			get { return roles; }
			set
			{
				roles = value;
				foreach (ApprovalRole role in value)
				{
					rolesList.Add(role.Name, role);
				}
			}
		}
		private Dictionary<string, ApprovalRole> rolesList = new Dictionary<string, ApprovalRole>();
		/// <summary>
		/// ���ݽ�ɫ����ȡ��֮��Ӧ��������ɫ
		/// </summary>
		/// <param name="roleName"></param>
		/// <returns></returns>
		public ApprovalRole GetRoleByName(string roleName)
		{
			if (roleName == null)
				throw new ArgumentNullException("roleName");
			roleName = roleName.Trim();
			if (!rolesList.ContainsKey(roleName))
				throw new ArgumentException("role:" + roleName + " isn't exist in workflow!", "roleName");
			return rolesList[roleName];
		}

		/// <summary>
		/// ����״̬���ƻ�ȡ״̬
		/// </summary>
		/// <param name="stateName"></param>
		/// <returns></returns>
		public ApprovalState GetStateByName(string stateName)
		{
			if (stateName == null)
				throw new ArgumentNullException("stateName");
			stateName = stateName.Trim();
			if (!statesList.ContainsKey(stateName))
				throw new ArgumentException("state:" + stateName + " isn't exist in workflow!", "stateName");
			return statesList[stateName];
		}

		/// <summary>
		/// Validates the specified error MSG.
		/// </summary>
		/// <param name="errorMsg">The error MSG.</param>
		/// <returns></returns>
		public bool Validate(out string errorMsg)
		{
			StringBuilder sb = new StringBuilder();
			if (!statesList.ContainsKey(initState))
				sb.AppendFormat("������:\"{0}\"�в���������Ϊ\"{1}\"�Ŀ�ʼ״̬��\r\n", this.Name, initState);
			if (!statesList.ContainsKey(endState))
				sb.AppendFormat("������:\"{0}\"�в���������Ϊ\"{1}\"�Ľ���״̬��\r\n", this.Name, endState);
			if (!string.IsNullOrEmpty(terminateState) && !statesList.ContainsKey(endState))
				sb.AppendFormat("������:\"{0}\"�в���������Ϊ\"{1}\"��״̬��\r\n", this.Name, terminateState);
			foreach (ApprovalState state in states)
			{
				//if (!state.Name.Equals(endState) && state.Events.Length == 0)
				//{
				//    sb.AppendFormat("������:\"{0}\"����Ϊ\"{1}\"��״̬�±����������һ���¼�����������:\"{2}({3})��\"\r\n", this.Name, state.Name, state.Name, state.Description);
				//}
				foreach (ApprovalEvent approvalEvent in state.Events)
				{
					foreach (string stateName in approvalEvent.NextStateNames)
					{
						if (!statesList.ContainsKey(stateName))
							sb.AppendFormat("������:\"{0}\"�в���������Ϊ\"{1}\"��״̬����������:\"{2}({3})-{4}��\"\r\n", this.Name, stateName, state.Name, state.Description, approvalEvent.Name);
					}
					foreach (EventRole role in approvalEvent.Roles)
					{
						if (!rolesList.ContainsKey(role.Name))
							sb.AppendFormat("������:\"{0}\"�в���������Ϊ\"{1}\"�Ľ�ɫ����������:\"{2}({3})-{4}��\"\r\n", this.Name, role.Name, state.Name, state.Description, approvalEvent.Name);
					}
				}
			}
			errorMsg = sb.ToString();
			if (errorMsg.Trim() != "")
				return false;
			return true;
		}
	}
}

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

namespace OilDigital.Workflows
{
	/// <summary>
	/// ������ʵ���Ĺ������ӿ�
	/// </summary>
	public abstract class InstanceFilter : ICloneable
	{
		/// <summary>
		/// �ж�ָ��ʵ���Ƿ���Ҫ������������������򷵻�True�����򷵻�False
		/// </summary>
		/// <param name="instance">������ʵ��</param>
		/// <param name="role">�û���������ɫ</param>
		/// <param name="userIdentity">�û������</param>
		/// <returns>
		/// �����򷵻�True�����򷵻�False
		/// </returns>
		protected abstract bool IsMatch(StateMachineWorkflowInstance instance, ApprovalRole role, IUserIdentity userIdentity);
		/// <summary>
		/// �ж�ָ��ʵ���Ƿ���Ҫ������������������򷵻�True�����򷵻�False
		/// </summary>
		/// <param name="instance">������ʵ��</param>
		/// <param name="role">�û���������ɫ</param>
		/// <param name="userIdentity">�û������</param>
		/// <returns>
		/// �����򷵻�True�����򷵻�False
		/// </returns>
		internal bool InternalIsMatch(StateMachineWorkflowInstance instance, ApprovalRole role, IUserIdentity userIdentity) 
		{
			if (instance == null)
				throw new ArgumentNullException("instance");
			if (role == null)
				throw new ArgumentNullException("role");
			if (userIdentity == null)
				throw new ArgumentNullException("userIdentity");
			return IsMatch(instance, role, userIdentity);
		}

		#region ICloneable Members

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		/// A new object that is a copy of this instance.
		/// </returns>
		object ICloneable.Clone()
		{
			return Clone();
		}

		/// <summary>
		/// Clones this instance.
		/// </summary>
		/// <returns></returns>
		internal protected virtual InstanceFilter Clone()
		{
			return this;
		}

		#endregion
	}

	/// <summary>
	/// ʵ�����Ϲ�����,��һ��������ʵ�����Ͻ��й���,������������ȥ��
	/// </summary>
	public interface IInstanceCollectionFilter
	{
		/// <summary>
		/// ��һ��������ʵ�����Ͻ��й���,������������ȥ��
		/// </summary>
		/// <param name="instances">��Ҫ�����˵�ʵ������</param>
		/// <param name="role">��ɫ����</param>
		/// <param name="userIdentity">���ʶ���־</param>
		/// <returns>�������������ʵ���ų����ʵ�����϶���.</returns>
		InstanceCollection Filter(InstanceCollection instances, ApprovalRole role, IUserIdentity userIdentity);
	}
	/// <summary>
	/// �յĹ������������κι���
	/// </summary>
	public class EmptyFilter : InstanceFilter
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
			return false;
		}
	}
	/// <summary>
	/// �Ƿ��ƶ��˶������û�ר��Ĺ��������ж���
	/// </summary>
	public class AssignedToOtherFilter : InstanceFilter//, IInstanceCollectionFilter
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
			List<ApprovalAssignment> assignmentList = WorkflowRuntime.Current.GetService<IApprovalSaveService>().GetAssignmentByAssignToRole(instance.WorkflowName, role.Name, instance.Id, instance.StateName);
			if (assignmentList.Count == 0)
				return false;

			return IsOthers(userIdentity, assignmentList);
		}

		private static bool IsOthers(IUserIdentity userIdentity, List<ApprovalAssignment> assignmentList)
		{
			//�Ƿ���ָ��ͬ��ɫ������ר���ʶ
			bool assignToOther = false;
			foreach (ApprovalAssignment assignment in assignmentList)
			{
				if (assignment.ToUserId != null)
				{
					if (assignment.ToUserId.ToLower() == userIdentity.GetUserId().ToLower())
						return false;
					else
						assignToOther = true;
				}
			}
			return assignToOther;
		}

		#region IInstanceCollectionFilter Members

		/// <summary>
		/// ,��һ��������ʵ�����Ͻ��й���,��������������ȥ��
		/// </summary>
		/// <param name="instances">��Ҫ�����˵�ʵ������</param>
		/// <param name="role">��ɫ����</param>
		/// <param name="userIdentity">���ʶ���־</param>
		/// <returns>�������������ʵ���ų����ʵ�����϶���.</returns>
		public InstanceCollection Filter(InstanceCollection instances, ApprovalRole role, IUserIdentity userIdentity)
		{
			List<ApprovalAssignment> assignmentList = WorkflowRuntime.Current.GetService<IApprovalSaveService>().GetAssignmentByAssignToRole(role.Name, instances);
			for (int i = instances.Count - 1; i >= 0; i--)
			{
				if (IsOthers(userIdentity, GetAssignment(instances[i], assignmentList)))
					instances.RemoveAt(i);
			}
			return instances;
		}

		private List<ApprovalAssignment> GetAssignment(InstanceWithRole instance, List<ApprovalAssignment> assignmentList)
		{
			List<ApprovalAssignment> rtn = new List<ApprovalAssignment>();
			foreach (ApprovalAssignment one in assignmentList)
			{
				if (one.WorkflowInstanceId.Equals(instance.Instance.Id))
					rtn.Add(one);
			}
			return rtn;
		}
		#endregion
	}

	/// <summary>
	/// �ų�������������λ��ʵ��
	/// </summary>
	public class AssignedToOtherUnitFilter : InstanceFilter, IInstanceCollectionFilter
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
			List<ApprovalAssignment> assignmentList = WorkflowRuntime.Current.GetService<IApprovalSaveService>().GetAssignment(instance.Id);
			if (assignmentList.Count == 0)
				return false;

			return IsOthers(userIdentity, assignmentList);
		}

		/// <summary>
		/// Determines whether the specified user identity is others.
		/// </summary>
		/// <param name="userIdentity">The user identity.</param>
		/// <param name="assignmentList">The assignment list.</param>
		/// <returns>
		/// 	<c>true</c> if the specified user identity is others; otherwise, <c>false</c>.
		/// </returns>
		private static bool IsOthers(IUserIdentity userIdentity, List<ApprovalAssignment> assignmentList)
		{
			//�Ƿ���ָ��ͬ��ɫ������ר���ʶ
			bool assignToOther = false;
			foreach (ApprovalAssignment assignment in assignmentList)
			{
				if (assignment.ToUserId == null)
				{
					if (assignment.ToUnitCode.Trim() == userIdentity.GetUserUnitCode().Trim())
						return false;
					else
						assignToOther = true;
				}
			}
			return assignToOther;
		}



		#region IInstanceCollectionFilter Members

		/// <summary>
		/// ��һ��������ʵ�����Ͻ��й���,������������ȥ��
		/// </summary>
		/// <param name="instances">��Ҫ�����˵�ʵ������</param>
		/// <param name="role">��ɫ����</param>
		/// <param name="userIdentity">���ʶ���־</param>
		/// <returns>�������������ʵ���ų����ʵ�����϶���.</returns>
		public InstanceCollection Filter(InstanceCollection instances, ApprovalRole role, IUserIdentity userIdentity)
		{
			List<ApprovalAssignment> assignmentList = WorkflowRuntime.Current.GetService<IApprovalSaveService>().GetAssignment(instances.InstanceIDs);
			for (int i = instances.Count - 1; i >= 0; i--)
			{
				if(IsOthers(userIdentity,GetAssignment(instances[i],assignmentList)))
					instances.RemoveAt(i);
			}
			return instances;
		}

		private List<ApprovalAssignment> GetAssignment(InstanceWithRole instance, List<ApprovalAssignment> assignmentList)
		{
			List<ApprovalAssignment> rtn = new List<ApprovalAssignment>();
			foreach (ApprovalAssignment one in assignmentList)
			{
				if (one.WorkflowInstanceId.Equals(instance.Instance.Id))
					rtn.Add(one);
			}
			return rtn;
		}
		#endregion
	}

	/// <summary>
	/// �����û�������Ҫ���˵�
	/// </summary>
	public class OtherUserLastProceedFilter : InstanceFilter
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
			//���һ��Activity��ִ�����Ƿ�Ϊ��ǰ�û�����Ƿ���false
			if (instance == null)
				throw new ArgumentNullException("instance");
			StateMachineWorkflowInstance stateMachine = (StateMachineWorkflowInstance)WorkflowRuntime.Current.GetInstance(instance.Id);
			//���һ��activityΪ�� ����true
			if (stateMachine.LastActivity == null)
				return true;
			//���һ��activityִ�������û���ݺͽ�ɫƥ�䣬����false
			if (string.Equals(stateMachine.LastActivity.UserId.Trim(), userIdentity.GetUserId().Trim(), StringComparison.OrdinalIgnoreCase)
				&& string.Equals(stateMachine.LastActivity.UserApprovalRole, role.Name))
				return false;
			return true;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class IsMyTaskFilter : InstanceFilter
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
			return WorkflowRuntime.Current.GetService<IWorkflowSecurityService>().IsMyTaskInstance(instance, userIdentity.GetUserId());
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class NotProceedFilter : InstanceFilter
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
			List<ApprovalRecord> records = WorkflowRuntime.Current.SaveService.GetRecord(instance.WorkflowName, instance.EaId);
			foreach (ApprovalRecord record in records)
			{
				if (record.OperatorId == userIdentity.GetUserId()
					&& record.OperatorRole == role.Name)
					return false;
			}
			return true;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class CompletedFilter : InstanceFilter
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
			return instance.IsEnd();
		}
	}

	/// <summary>
	/// ���˵��Ѿ���ֹ������.
	/// </summary>
	public class TerminatedFilter : InstanceFilter
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
			return instance.IsTerminated();
		}
	}
}

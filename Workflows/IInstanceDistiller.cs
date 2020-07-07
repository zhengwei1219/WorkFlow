using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;

namespace OilDigital.Workflows
{
	/// <summary>
	/// ʵ����ȡ���ӿ�
	/// </summary>
	public abstract class InstanceDistiller : ICloneable
	{
		/// <summary>
		/// ��ȡָ������������ɫ��ʱ��ε�ʵ������
		/// </summary>
		/// <param name="workflowName">��������</param>
		/// <param name="userIdentity">�û����</param>
		/// <param name="role">������ɫ</param>
		/// <param name="startDate">ʱ�����ʼʱ��</param>
		/// <param name="endDate">ʱ��ν�ֹʱ��</param>
		/// <returns></returns>
		protected abstract InstanceCollection Distill(string workflowName, IUserIdentity userIdentity, ApprovalRole role, DateTime startDate, DateTime endDate);
		/// <summary>
		/// ��ȡָ������������ɫ��ʱ��ε�ʵ������
		/// </summary>
		/// <param name="workflowName">��������</param>
		/// <param name="userIdentity">�û����</param>
		/// <param name="role">������ɫ</param>
		/// <param name="startDate">ʱ�����ʼʱ��</param>
		/// <param name="endDate">ʱ��ν�ֹʱ��</param>
		/// <returns></returns>
		internal InstanceCollection InternalDistill(string workflowName, IUserIdentity userIdentity, ApprovalRole role, DateTime startDate, DateTime endDate)
		{
			if (workflowName == null)
				throw new ArgumentNullException("workflowName");
			if (userIdentity == null)
				throw new ArgumentNullException("userIdentity");
			if (role == null)
				throw new ArgumentNullException("role");
			return Distill(workflowName, userIdentity, role, startDate, endDate);
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
		internal InstanceDistiller Clone()
		{
			return this;
		}

		#endregion
	}
	/// <summary>
	/// ��׼ʵ����ȡ��:���ݸ����Ľ�ɫ��ȡ����ִ�е�ʵ��.����ȡֻ�ǰ�װ��ɫ������ȡ,û���κι���.
	/// </summary>
	public class StandardDistiller : InstanceDistiller
	{
		/// <summary>
		/// ��ȡָ������������ɫ��ʱ��ε�ʵ������
		/// </summary>
		/// <param name="workflowName">��������</param>
		/// <param name="userIdentity">�û����</param>
		/// <param name="role">������ɫ</param>
		/// <param name="startDate">ʱ�����ʼʱ��</param>
		/// <param name="endDate">ʱ��ν�ֹʱ��</param>
		/// <returns></returns>
		protected override InstanceCollection Distill(string workflowName, IUserIdentity userIdentity, ApprovalRole role, DateTime startDate, DateTime endDate)
		{
			InstanceCollection instances = new InstanceCollection();
			StateMachineWorkflow workflow = (StateMachineWorkflow)WorkflowRuntime.Current.GetService<IWorkFlowDefinePersistService>().GetWorkflowDefine(workflowName);
			//�����������е�ÿһ��״̬,��ȡ���Դ����״̬��Ӧ��ʵ��.
			List<string> canDoStates = new List<string>();
			foreach (ApprovalState oneState in workflow.States)
			{
				if (oneState.IsApprovalState && InstanceDistillerHelper.IsMineICanDo(oneState, role))
					canDoStates.Add(oneState.Name);
			}

			if (canDoStates.Count == 0) return instances;
			List<StateMachineWorkflowInstance> list = WorkflowRuntime.Current.GetListByState(workflowName, canDoStates.ToArray());
			//��ȡָ��������λ�����ʵ��
			foreach (StateMachineWorkflowInstance instance in list)
			{
				if (instance.PersistTime >= startDate 
					&& instance.PersistTime <= endDate)
					instances.Add(new InstanceWithRole(instance, role, true));
			}
			
			return instances;
		}
	}
	/// <summary>
	/// ��ȡ�û��Լ���ִ������
	/// </summary>
	public class UserOwnerDistiller : InstanceDistiller
	{
		private bool ignoreUnit;
		/// <summary>
		/// Gets or sets a value indicating whether [ignore unit].
		/// </summary>
		public bool IgnoreUnit
		{
			private get { return ignoreUnit; }
			set { ignoreUnit = value; }
		}
		/// <summary>
		/// ��ȡָ������������ɫ��ʱ��ε�ʵ������
		/// </summary>
		/// <param name="workflowName">��������</param>
		/// <param name="userIdentity">�û����</param>
		/// <param name="role">������ɫ</param>
		/// <param name="startDate">ʱ�����ʼʱ��</param>
		/// <param name="endDate">ʱ��ν�ֹʱ��</param>
		/// <returns></returns>
		protected override InstanceCollection Distill(string workflowName, IUserIdentity userIdentity, ApprovalRole role, DateTime startDate, DateTime endDate)
		{
			string userId = userIdentity.GetUserId();
			string unitCode = IgnoreUnit ? "" : userIdentity.GetUserUnitCode();
			InstanceCollection instances = new InstanceCollection();
			StateMachineWorkflow workflow = (StateMachineWorkflow)WorkflowRuntime.Current.GetService<IWorkFlowDefinePersistService>().GetWorkflowDefine(workflowName);
			//�����������е�ÿһ��״̬,��ȡ���Դ����״̬��Ӧ��ʵ��.
			List<string> canDoStates = new List<string>();
			foreach (ApprovalState oneState in workflow.States)
			{
				if (oneState.IsApprovalState &&  InstanceDistillerHelper.IsMineICanDo(oneState, role))
					canDoStates.Add(oneState.Name);
			}

			if (canDoStates.Count == 0) return instances;
			List<StateMachineWorkflowInstance> list = WorkflowRuntime.Current.PersistService.GetWorkflowInstance(workflowName, canDoStates.ToArray(), userId, unitCode);
			//��ȡָ��������λ�����ʵ��
			foreach (StateMachineWorkflowInstance instance in list)
			{
				if (instance.PersistTime >= startDate
					&& instance.PersistTime <= endDate)
					instances.Add(new InstanceWithRole(instance, role, true));
			}

			return instances;
		}
	}

	/// <summary>
	/// ����λ�Լ�������ʵ����ȡ��
	/// </summary>
	public class UnitOwnerDistiller : InstanceDistiller
	{
		/// <summary>
		/// ��ȡָ������������ɫ��ʱ��ε�ʵ������
		/// </summary>
		/// <param name="workflowName">��������</param>
		/// <param name="userIdentity">�û����</param>
		/// <param name="role">������ɫ</param>
		/// <param name="startDate">ʱ�����ʼʱ��</param>
		/// <param name="endDate">ʱ��ν�ֹʱ��</param>
		/// <returns></returns>
		protected override InstanceCollection Distill(string workflowName, IUserIdentity userIdentity, ApprovalRole role, DateTime startDate, DateTime endDate)
		{
			InstanceCollection instances = new InstanceCollection();
			StateMachineWorkflow workflow = (StateMachineWorkflow)WorkflowRuntime.Current.GetService<IWorkFlowDefinePersistService>().GetWorkflowDefine(workflowName);
			//�����������е�ÿһ��״̬,��ȡ���Դ����״̬��Ӧ��ʵ��.
			List<string> canDoStates = new List<string>();
			foreach (ApprovalState oneState in workflow.States)
			{
				if (oneState.IsApprovalState && InstanceDistillerHelper.IsMineICanDo(oneState, role))
					canDoStates.Add(oneState.Name);
			}

			if (canDoStates.Count == 0) return instances;

			List<StateMachineWorkflowInstance> list = WorkflowRuntime.Current.GetUnitList(workflowName, canDoStates.ToArray(), userIdentity.GetUserUnitCode());
			//��ȡָ��������λ�����ʵ��
			foreach (StateMachineWorkflowInstance instance in list)
			{
				if (instance.PersistTime >= startDate
					&& instance.PersistTime <= endDate)
					instances.Add(new InstanceWithRole(instance, role, true));
			}

			return instances;
		}
	}

	/// <summary>
	/// ����λ�����ӵ�λ������ʵ����ȡ��
	/// </summary>
	public class ChildrenUnitDistiller : InstanceDistiller
	{
		/// <summary>
		/// ��ȡָ������������ɫ��ʱ��ε�ʵ������
		/// </summary>
		/// <param name="workflowName">��������</param>
		/// <param name="userIdentity">�û����</param>
		/// <param name="role">������ɫ</param>
		/// <param name="startDate">ʱ�����ʼʱ��</param>
		/// <param name="endDate">ʱ��ν�ֹʱ��</param>
		/// <returns></returns>
		protected override InstanceCollection Distill(string workflowName, IUserIdentity userIdentity, ApprovalRole role, DateTime startDate, DateTime endDate)
		{
			InstanceCollection instances = new InstanceCollection();
			StateMachineWorkflow workflow = (StateMachineWorkflow)WorkflowRuntime.Current.GetService<IWorkFlowDefinePersistService>().GetWorkflowDefine(workflowName);
			//�����������е�ÿһ��״̬,��ȡ���Դ����״̬��Ӧ��ʵ��.
			List<string> canDoStates = new List<string>();
			foreach (ApprovalState oneState in workflow.States)
			{
				if (oneState.IsApprovalState && InstanceDistillerHelper.IsMineICanDo(oneState, role))
					canDoStates.Add(oneState.Name);
			}

			if (canDoStates.Count == 0) return instances;
			string unitCode =userIdentity.GetUserUnitCode();
			unitCode= string.IsNullOrEmpty(unitCode)?"  ":(unitCode.Trim() + "%");
			List<StateMachineWorkflowInstance> list = WorkflowRuntime.Current.GetUnitList(workflowName, canDoStates.ToArray(), unitCode);
			//��ȡָ��������λ�����ʵ��
			foreach (StateMachineWorkflowInstance instance in list)
			{
				if (instance.PersistTime >= startDate
					&& instance.PersistTime <= endDate)
					instances.Add(new InstanceWithRole(instance, role, true));
			}

			return instances;
		}
	}

	/// <summary>
	/// ���������λ����ȡ��.
	/// </summary>
	public class AssignedByUnitDistiller : InstanceDistiller
	{
		/// <summary>
		/// ��ȡָ������������ɫ��ʱ��ε�ʵ������
		/// </summary>
		/// <param name="workflowName">��������</param>
		/// <param name="userIdentity">�û����</param>
		/// <param name="role">������ɫ</param>
		/// <param name="startDate">ʱ�����ʼʱ��</param>
		/// <param name="endDate">ʱ��ν�ֹʱ��</param>
		/// <returns></returns>
		protected override InstanceCollection Distill(string workflowName, IUserIdentity userIdentity, ApprovalRole role, DateTime startDate, DateTime endDate)
		{
			List<StateMachineWorkflowInstance> instances = new List<StateMachineWorkflowInstance>();
			List<ApprovalAssignment> assignmentList = WorkflowRuntime.Current.GetService<IApprovalSaveService>().GetAssignmentByToUnit(workflowName, userIdentity.GetUserUnitCode());
			List<Guid> ids = new List<Guid>();
			foreach (ApprovalAssignment assignment in assignmentList)
			{
				if (!string.IsNullOrEmpty(assignment.ToUserId))
					continue;
				if (!ids.Contains(assignment.WorkflowInstanceId))
					ids.Add(assignment.WorkflowInstanceId);
			}
			foreach (Guid id in ids)
			{
				instances.Add((StateMachineWorkflowInstance)WorkflowRuntime.Current.GetInstance(id));
			}
			InstanceCollection collection = new InstanceCollection();
			foreach (StateMachineWorkflowInstance instance in instances)
			{
				if (InstanceDistillerHelper.IsAssignedICanDo(instance.CurrentState, role)
					&& instance.PersistTime >= startDate
					&& instance.PersistTime <= endDate)
					collection.Add(new InstanceWithRole(instance, role, false));
			}
			return collection;
		}
	}

	/// <summary>
	/// ���ض���״̬��ȡ������ʵ��
	/// </summary>
	public class StatesDistiller : InstanceDistiller
	{
		private string[] states;
		/// <summary>
		/// Sets the states string.
		/// </summary>
		/// <value>The states string.</value>
		public string StatesString
		{
			set { this.states = value.Split(",;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries); }
		}

		/// <summary>
		/// ��ȡָ������������ɫ��ʱ��ε�ʵ������
		/// </summary>
		/// <param name="workflowName">��������</param>
		/// <param name="userIdentity">�û����</param>
		/// <param name="role">������ɫ</param>
		/// <param name="startDate">ʱ�����ʼʱ��</param>
		/// <param name="endDate">ʱ��ν�ֹʱ��</param>
		/// <returns></returns>
		protected override InstanceCollection Distill(string workflowName, IUserIdentity userIdentity, ApprovalRole role, DateTime startDate, DateTime endDate)
		{
			InstanceCollection instances = new InstanceCollection();

			if (this.states != null)
			{

				List<StateMachineWorkflowInstance> list = WorkflowRuntime.Current.GetListByState(workflowName, this.states);
				//��ȡָ��������λ�����ʵ��
				foreach (StateMachineWorkflowInstance instance in list)
				{
					if (instance.PersistTime >= startDate
						&& instance.PersistTime <= endDate)
						instances.Add(new InstanceWithRole(instance, role, true));
				}
			}

			return instances;
		}
	}

	/// <summary>
	/// ��ȡ���п��Գ����Ĺ�����ʵ����ȡ��,���ݹ�����״̬�����е�����,��ȡ����״̬�������Ĺ�����ʵ��.
	/// </summary>
	public class StandardAllowedCancelDistiller : InstanceDistiller
	{
		/// <summary>
		/// ��ȡָ������������ɫ��ʱ��ε�ʵ������
		/// </summary>
		/// <param name="workflowName">��������</param>
		/// <param name="userIdentity">�û����</param>
		/// <param name="role">������ɫ</param>
		/// <param name="startDate">ʱ�����ʼʱ��</param>
		/// <param name="endDate">ʱ��ν�ֹʱ��</param>
		/// <returns></returns>
		protected override InstanceCollection Distill(string workflowName, IUserIdentity userIdentity, ApprovalRole role, DateTime startDate, DateTime endDate)
		{
			InstanceCollection collection = new InstanceCollection();
			List<StateMachineWorkflowInstance> instances = new List<StateMachineWorkflowInstance>();
			List<string> states = InstanceDistillerHelper.GetMineICanCancelStates(workflowName, role);

			if (states.Count == 0) return collection;
			List<StateMachineWorkflowInstance> list = WorkflowRuntime.Current.GetListByState(workflowName, states.ToArray());
			instances.AddRange(list);
			
			foreach (StateMachineWorkflowInstance instance in instances)
			{
				if (instance.PersistTime >= startDate
					&& instance.PersistTime <= endDate)
					collection.Add(new InstanceWithRole(instance, role, true));
			}
			return collection;
		}
	}

	/// <summary>
	/// ��ȡָ���û��Լ��������Ĺ�����ʵ��
	/// </summary>
	public class UserOwnerAllowedCancelDistiller : InstanceDistiller
	{
		/// <summary>
		/// ��ȡָ������������ɫ��ʱ��ε�ʵ������
		/// </summary>
		/// <param name="workflowName">��������</param>
		/// <param name="userIdentity">�û����</param>
		/// <param name="role">������ɫ</param>
		/// <param name="startDate">ʱ�����ʼʱ��</param>
		/// <param name="endDate">ʱ��ν�ֹʱ��</param>
		/// <returns></returns>
		protected override InstanceCollection Distill(string workflowName, IUserIdentity userIdentity, ApprovalRole role, DateTime startDate, DateTime endDate)
		{
			InstanceCollection collection = new InstanceCollection();
			List<StateMachineWorkflowInstance> instances = new List<StateMachineWorkflowInstance>();
			List<string> states = InstanceDistillerHelper.GetMineICanCancelStates(workflowName, role);

			if (states.Count == 0) return collection;
			instances.AddRange(WorkflowRuntime.Current.GetOwnerList(workflowName, states.ToArray(), userIdentity.GetUserId(), userIdentity.GetUserUnitCode()));

			
			foreach (StateMachineWorkflowInstance instance in instances)
			{
				if (instance.PersistTime >= startDate
					&& instance.PersistTime <= endDate)
					collection.Add(new InstanceWithRole(instance, role, true));
			}
			return collection;
		}
	}

	/// <summary>
	/// ��ȡָ����λ�ܹ������Ĺ�����ʵ��
	/// </summary>
	public class UnitOwnerAllowedCancelDistiller : InstanceDistiller
	{
		/// <summary>
		/// ��ȡָ������������ɫ��ʱ��ε�ʵ������
		/// </summary>
		/// <param name="workflowName">��������</param>
		/// <param name="userIdentity">�û����</param>
		/// <param name="role">������ɫ</param>
		/// <param name="startDate">ʱ�����ʼʱ��</param>
		/// <param name="endDate">ʱ��ν�ֹʱ��</param>
		/// <returns></returns>
		protected override InstanceCollection Distill(string workflowName, IUserIdentity userIdentity, ApprovalRole role, DateTime startDate, DateTime endDate)
		{
			InstanceCollection collection = new InstanceCollection();
			List<StateMachineWorkflowInstance> instances = new List<StateMachineWorkflowInstance>();
			List<string> states = InstanceDistillerHelper.GetMineICanCancelStates(workflowName, role);
			if (states.Count == 0) return collection;

			instances.AddRange(WorkflowRuntime.Current.GetUnitList(workflowName, states.ToArray(), userIdentity.GetUserUnitCode()));
			foreach (StateMachineWorkflowInstance instance in instances)
			{
				if (instance.PersistTime >= startDate
					&& instance.PersistTime <= endDate)
					collection.Add(new InstanceWithRole(instance, role, true));
			}
			return collection;
		}
	}

	/// <summary>
	/// ��ȡָ����λ�����ӵ�λ�ܹ������Ĺ�����ʵ��
	/// </summary>
	public class ChildrenUnitAllowedCancelDistiller : InstanceDistiller
	{
		/// <summary>
		/// ��ȡָ������������ɫ��ʱ��ε�ʵ������
		/// </summary>
		/// <param name="workflowName">��������</param>
		/// <param name="userIdentity">�û����</param>
		/// <param name="role">������ɫ</param>
		/// <param name="startDate">ʱ�����ʼʱ��</param>
		/// <param name="endDate">ʱ��ν�ֹʱ��</param>
		/// <returns></returns>
		protected override InstanceCollection Distill(string workflowName, IUserIdentity userIdentity, ApprovalRole role, DateTime startDate, DateTime endDate)
		{
			InstanceCollection collection = new InstanceCollection();
			List<StateMachineWorkflowInstance> instances = new List<StateMachineWorkflowInstance>();
			List<string> states = InstanceDistillerHelper.GetMineICanCancelStates(workflowName, role);
			if (states.Count == 0) return collection;

			string unitCode = userIdentity.GetUserUnitCode();
			unitCode = string.IsNullOrEmpty(unitCode) ? "  " : (unitCode.Trim() + "%");
			instances.AddRange(WorkflowRuntime.Current.GetUnitList(workflowName, states.ToArray(), unitCode));
			foreach (StateMachineWorkflowInstance instance in instances)
			{
				if (instance.PersistTime >= startDate
					&& instance.PersistTime <= endDate)
					collection.Add(new InstanceWithRole(instance, role, true));
			}
			return collection;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class AssignedByUnitAllowedCancelDistiller : InstanceDistiller
	{
		/// <summary>
		/// ��ȡָ������������ɫ��ʱ��ε�ʵ������
		/// </summary>
		/// <param name="workflowName">��������</param>
		/// <param name="userIdentity">�û����</param>
		/// <param name="role">������ɫ</param>
		/// <param name="startDate">ʱ�����ʼʱ��</param>
		/// <param name="endDate">ʱ��ν�ֹʱ��</param>
		/// <returns></returns>
		protected override InstanceCollection Distill(string workflowName, IUserIdentity userIdentity, ApprovalRole role, DateTime startDate, DateTime endDate)
		{
			List<StateMachineWorkflowInstance> instances = new List<StateMachineWorkflowInstance>();
			List<string> states = InstanceDistillerHelper.GetAssignedICanCancelStates(workflowName, role);
			if (states.Count == 0) return new InstanceCollection();
			List<ApprovalAssignment> assignmentList = WorkflowRuntime.Current.GetService<IApprovalSaveService>().GetAssignmentByToUnit(workflowName, userIdentity.GetUserUnitCode());
			List<Guid> ids = new List<Guid>();
			foreach (ApprovalAssignment assignment in assignmentList)
			{
				if (!string.IsNullOrEmpty(assignment.ToUserId))
					continue;
				if (!ids.Contains(assignment.WorkflowInstanceId))
					ids.Add(assignment.WorkflowInstanceId);
			}
			foreach (Guid id in ids)
			{
				instances.Add((StateMachineWorkflowInstance)WorkflowRuntime.Current.GetInstance(id));
			}
			InstanceCollection collection = new InstanceCollection();
			foreach (StateMachineWorkflowInstance instance in instances)
			{
				if (states.Contains(instance.CurrentState.Name)
					&& instance.PersistTime >= startDate
					&& instance.PersistTime <= endDate)
					collection.Add(new InstanceWithRole(instance, role, false));
			}
			return collection;
		}
	}

	/// <summary>
	/// ��ȡ�û��Ѱ����������ʵ����ȡ��:ͨ�����Ұ����¼��ȡ�����û��Ѿ��������ʵ��
	/// </summary>
	public class ProceedDistiller : InstanceDistiller
	{
		private bool isMatchUser = true;
		/// <summary>
		/// Sets a value indicating whether this instance is match user.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is match user; otherwise, <c>false</c>.
		/// </value>
		public bool IsMatchUser
		{
			set { this.isMatchUser = value; }
		}

		private bool isMatchUnit = false;
		/// <summary>
		/// Sets a value indicating whether this instance is match unit.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is match unit; otherwise, <c>false</c>.
		/// </value>
		public bool IsMatchUnit
		{
			set { this.isMatchUnit = value; }
		}

		private bool isMatchRole = true;
		/// <summary>
		/// Sets a value indicating whether this instance is match role.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is match role; otherwise, <c>false</c>.
		/// </value>
		public bool IsMatchRole
		{
			set { this.isMatchRole = value; }
		}

		/// <summary>
		/// ��ȡָ������������ɫ��ʱ��ε�ʵ������
		/// </summary>
		/// <param name="workflowName">��������</param>
		/// <param name="userIdentity">�û����</param>
		/// <param name="role">������ɫ</param>
		/// <param name="startDate">ʱ�����ʼʱ��</param>
		/// <param name="endDate">ʱ��ν�ֹʱ��</param>
		/// <returns></returns>
		protected override InstanceCollection Distill(string workflowName, IUserIdentity userIdentity, ApprovalRole role, DateTime startDate, DateTime endDate)
		{
			StateMachineWorkflow workflow = WorkflowRuntime.Current.GetService<IWorkFlowDefinePersistService>().GetWorkflowDefine(workflowName) as StateMachineWorkflow;
			string userId, unitCode, roleName;
			userId = unitCode = roleName = "";
			if (isMatchUser)
				userId = userIdentity.GetUserId();
			if (isMatchUnit)
				unitCode = userIdentity.GetUserUnitCode();
			if (isMatchRole)
				roleName = role.Name;
			List<ApprovalRecord> records = WorkflowRuntime.Current.GetService<IApprovalSaveService>().GetRecord(workflowName, startDate, endDate, userId, unitCode, roleName);
			List<Guid> ids = new List<Guid>();
			foreach (ApprovalRecord record in records)
			{
				if (!ids.Contains(record.WorkflowInstanceId))
					ids.Add(record.WorkflowInstanceId);
			}
			List<StateMachineWorkflowInstance> instances = new List<StateMachineWorkflowInstance>();
			foreach (Guid id in ids)
			{
				instances.Add((StateMachineWorkflowInstance)WorkflowRuntime.Current.GetInstance(id));
			}
			InstanceCollection collection = new InstanceCollection();
			foreach (StateMachineWorkflowInstance instance in instances)
			{
				collection.Add(new InstanceWithRole(instance, role, false));
			}
			return collection;
		}
	}

	/// <summary>
	/// ��ȡָ����λ����Ĺ�����.
	/// </summary>
	public class UnitAssignedAllDistiller : InstanceDistiller
	{
		/// <summary>
		/// ��ȡָ������������ɫ��ʱ��ε�ʵ������
		/// </summary>
		/// <param name="workflowName">��������</param>
		/// <param name="userIdentity">�û����</param>
		/// <param name="role">������ɫ</param>
		/// <param name="startDate">ʱ�����ʼʱ��</param>
		/// <param name="endDate">ʱ��ν�ֹʱ��</param>
		/// <returns></returns>
		protected override InstanceCollection Distill(string workflowName, IUserIdentity userIdentity, ApprovalRole role, DateTime startDate, DateTime endDate)
		{
			InstanceCollection collection = new InstanceCollection();
			List<ApprovalAssignment> assignments = WorkflowRuntime.Current.SaveService.GetAssignmentByToUnit(workflowName, userIdentity.GetUserUnitCode());
			List<Guid> ids = new List<Guid>();
			collection = new InstanceCollection();
			foreach (ApprovalAssignment assignment in assignments)
			{
				if (string.IsNullOrEmpty(assignment.ToUserId))
				{
					if (!ids.Contains(assignment.WorkflowInstanceId))
						ids.Add(assignment.WorkflowInstanceId);
				}
			}
			foreach (Guid id in ids)
			{
				StateMachineWorkflowInstance instance = (StateMachineWorkflowInstance)WorkflowRuntime.Current.GetInstance(id);
				if (instance.PersistTime > startDate && instance.PersistTime < endDate)
					collection.Add(new InstanceWithRole(instance, role, false));
			}
			return collection;
		}
	}

	/// <summary>
	/// ��ȡ���а��������ȡ��.
	/// </summary>
	public class StandardCompletedDistiller : InstanceDistiller
	{
		/// <summary>
		/// ��ȡָ������������ɫ��ʱ��ε�ʵ������
		/// </summary>
		/// <param name="workflowName">��������</param>
		/// <param name="userIdentity">�û����</param>
		/// <param name="role">������ɫ</param>
		/// <param name="startDate">ʱ�����ʼʱ��</param>
		/// <param name="endDate">ʱ��ν�ֹʱ��</param>
		/// <returns></returns>
		protected override InstanceCollection Distill(string workflowName, IUserIdentity userIdentity, ApprovalRole role, DateTime startDate, DateTime endDate)
		{
			StateMachineWorkflow workflow = (StateMachineWorkflow)WorkflowRuntime.Current.GetService<IWorkFlowDefinePersistService>().GetWorkflowDefine(workflowName);
			List<StateMachineWorkflowInstance> instances = WorkflowRuntime.Current.GetInstance(workflowName, startDate, endDate, new string[] { workflow.EndState }, "");
			InstanceCollection collection = new InstanceCollection();
			foreach (StateMachineWorkflowInstance instance in instances)
			{
				collection.Add(new InstanceWithRole(instance, role, false));
			}
			return collection;
		}
	}

	/// <summary>
	/// ����λ�Ѿ���ɵĹ�����ʵ��
	/// </summary>
	public class UnitOwnerCompletedDistiller : InstanceDistiller
	{
		/// <summary>
		/// ��ȡָ������������ɫ��ʱ��ε�ʵ������
		/// </summary>
		/// <param name="workflowName">��������</param>
		/// <param name="userIdentity">�û����</param>
		/// <param name="role">������ɫ</param>
		/// <param name="startDate">ʱ�����ʼʱ��</param>
		/// <param name="endDate">ʱ��ν�ֹʱ��</param>
		/// <returns></returns>
		protected override InstanceCollection Distill(string workflowName, IUserIdentity userIdentity, ApprovalRole role, DateTime startDate, DateTime endDate)
		{
			StateMachineWorkflow workflow = (StateMachineWorkflow)WorkflowRuntime.Current.GetService<IWorkFlowDefinePersistService>().GetWorkflowDefine(workflowName);
			List<StateMachineWorkflowInstance> instances = WorkflowRuntime.Current.GetInstance(workflowName, startDate, endDate, new string[] { workflow.EndState }, userIdentity.GetUserUnitCode());
			InstanceCollection collection = new InstanceCollection();
			foreach (StateMachineWorkflowInstance instance in instances)
			{
				collection.Add(new InstanceWithRole(instance, role, false));
			}
			return collection;
		}
	}

	/// <summary>
	/// ��׼�����й�����ʵ����ȡ��:��ȡ����״̬���Ϊtrue��״̬������ʵ��
	/// </summary>
	public class StandardProcessingDistiller : InstanceDistiller
	{
		/// <summary>
		/// ��ȡָ������������ɫ��ʱ��ε�ʵ������
		/// </summary>
		/// <param name="workflowName">��������</param>
		/// <param name="userIdentity">�û����</param>
		/// <param name="role">������ɫ</param>
		/// <param name="startDate">ʱ�����ʼʱ��</param>
		/// <param name="endDate">ʱ��ν�ֹʱ��</param>
		/// <returns></returns>
		protected override InstanceCollection Distill(string workflowName, IUserIdentity userIdentity, ApprovalRole role, DateTime startDate, DateTime endDate)
		{
			List<StateMachineWorkflowInstance> instances = new List<StateMachineWorkflowInstance>();
			StateMachineWorkflow workflow = (StateMachineWorkflow)WorkflowRuntime.Current.GetService<IWorkFlowDefinePersistService>().GetWorkflowDefine(workflowName);
			Dictionary<string, int> stateIndex = new Dictionary<string, int>();
			List<string> allStates = new List<string>();
			for (int i = 0; i < workflow.States.Length; i++)
			{
				ApprovalState state = workflow.States[i];
				if (state.IsApprovalState && !state.Name.Equals(workflow.InitState))
				{
					stateIndex.Add(state.Name, i);
					allStates.Add(state.Name);
				}
			}
			if (allStates.Count == 0) return new InstanceCollection();

			instances.AddRange(WorkflowRuntime.Current.GetListByState(workflowName, allStates.ToArray()));
			InstanceCollection collection = new InstanceCollection();
			foreach (StateMachineWorkflowInstance one in instances)
			{
				InstanceWithRole o = new InstanceWithRole(one, role, true);
				o.TaskName = ((char)((int)'a' + stateIndex[one.CurrentState.Name])) + "." + one.CurrentState.Description;
				collection.Add(o);
			}
			return collection;
		}
	}

	/// <summary>
	/// ��ȡָ�����û��������й�����ʵ��
	/// </summary>
	public class UserOwnerProcessingDistiller : InstanceDistiller
	{
		/// <summary>
		/// ��ȡָ������������ɫ��ʱ��ε�ʵ������
		/// </summary>
		/// <param name="workflowName">��������</param>
		/// <param name="userIdentity">�û����</param>
		/// <param name="role">������ɫ</param>
		/// <param name="startDate">ʱ�����ʼʱ��</param>
		/// <param name="endDate">ʱ��ν�ֹʱ��</param>
		/// <returns></returns>
		protected override InstanceCollection Distill(string workflowName, IUserIdentity userIdentity, ApprovalRole role, DateTime startDate, DateTime endDate)
		{
			List<StateMachineWorkflowInstance> instances = new List<StateMachineWorkflowInstance>();
			StateMachineWorkflow workflow = (StateMachineWorkflow)WorkflowRuntime.Current.GetService<IWorkFlowDefinePersistService>().GetWorkflowDefine(workflowName);
			List<string> allStates = new List<string>();
			foreach (ApprovalState state in workflow.States)
			{
				if (state.IsApprovalState && !state.Name.Equals(workflow.InitState))
					allStates.Add(state.Name);
			}
			if (allStates.Count == 0) return new InstanceCollection();

			instances.AddRange(WorkflowRuntime.Current.GetOwnerList(workflowName, allStates.ToArray(), userIdentity.GetUserId(), userIdentity.GetUserUnitCode()));
			InstanceCollection collection = new InstanceCollection();

			foreach (StateMachineWorkflowInstance one in instances)
			{
				InstanceWithRole o = new InstanceWithRole(one, role, true);
				o.TaskName = "a." + one.CurrentState.Description;
				collection.Add(o);
			}
			return collection;
		}
	}

	/// <summary>
	/// ��ȡ����λ���д����еĹ�����ʵ��
	/// </summary>
	public class UnitOwnerProcessingDistiller : InstanceDistiller
	{
		/// <summary>
		/// ��ȡָ������������ɫ��ʱ��ε�ʵ������
		/// </summary>
		/// <param name="workflowName">��������</param>
		/// <param name="userIdentity">�û����</param>
		/// <param name="role">������ɫ</param>
		/// <param name="startDate">ʱ�����ʼʱ��</param>
		/// <param name="endDate">ʱ��ν�ֹʱ��</param>
		/// <returns></returns>
		protected override InstanceCollection Distill(string workflowName, IUserIdentity userIdentity, ApprovalRole role, DateTime startDate, DateTime endDate)
		{
			List<StateMachineWorkflowInstance> instances = new List<StateMachineWorkflowInstance>();
			StateMachineWorkflow workflow = (StateMachineWorkflow)WorkflowRuntime.Current.GetService<IWorkFlowDefinePersistService>().GetWorkflowDefine(workflowName);
			List<string> allStates = new List<string>();
			foreach (ApprovalState state in workflow.States)
			{
				if (state.IsApprovalState && !state.Name.Equals(workflow.InitState))
					allStates.Add(state.Name);
			}
			if (allStates.Count == 0) return new InstanceCollection();

			instances.AddRange(WorkflowRuntime.Current.GetUnitList(workflowName, allStates.ToArray(), userIdentity.GetUserUnitCode()));

			InstanceCollection collection = new InstanceCollection();
			foreach (StateMachineWorkflowInstance one in instances)
			{
				InstanceWithRole o = new InstanceWithRole(one, role, true);
				o.TaskName = "a." + one.CurrentState.Description;
				collection.Add(o);
			}
			return collection;
		}
	}


	/// <summary>
	/// ��ȡ����λ�����ӵ�λ�����еĹ�����ʵ��
	/// </summary>
	public class ChildrenUnitProcessingDistiller : InstanceDistiller
	{
		/// <summary>
		/// ��ȡָ������������ɫ��ʱ��ε�ʵ������
		/// </summary>
		/// <param name="workflowName">��������</param>
		/// <param name="userIdentity">�û����</param>
		/// <param name="role">������ɫ</param>
		/// <param name="startDate">ʱ�����ʼʱ��</param>
		/// <param name="endDate">ʱ��ν�ֹʱ��</param>
		/// <returns></returns>
		protected override InstanceCollection Distill(string workflowName, IUserIdentity userIdentity, ApprovalRole role, DateTime startDate, DateTime endDate)
		{
			List<StateMachineWorkflowInstance> instances = new List<StateMachineWorkflowInstance>();
			StateMachineWorkflow workflow = (StateMachineWorkflow)WorkflowRuntime.Current.GetService<IWorkFlowDefinePersistService>().GetWorkflowDefine(workflowName);
			List<string> allStates = new List<string>();
			foreach (ApprovalState state in workflow.States)
			{
				if (state.IsApprovalState && !state.Name.Equals(workflow.InitState))
					allStates.Add(state.Name);
			}
			if (allStates.Count == 0) return new InstanceCollection();

			string unitCode = userIdentity.GetUserUnitCode();
			unitCode = string.IsNullOrEmpty(unitCode) ? "  " : (unitCode.Trim() + "%");
			instances.AddRange(WorkflowRuntime.Current.GetUnitList(workflowName, allStates.ToArray(), unitCode));

			InstanceCollection collection = new InstanceCollection();
			foreach (StateMachineWorkflowInstance one in instances)
			{
				InstanceWithRole o = new InstanceWithRole(one, role, true);
				o.TaskName = "a." + one.CurrentState.Description;
				collection.Add(o);
			}
			return collection;
		}
	}

	/// <summary>
	/// ��ȡ�����������ȡ��
	/// </summary>
	public class AgentDistiller : InstanceDistiller
	{
		
		/// <summary>
		/// ��ȡָ������������ɫ��ʱ��ε�ʵ������
		/// </summary>
		/// <param name="workflowName">��������</param>
		/// <param name="userIdentity">�û����</param>
		/// <param name="role">������ɫ</param>
		/// <param name="startDate">ʱ�����ʼʱ��</param>
		/// <param name="endDate">ʱ��ν�ֹʱ��</param>
		/// <returns></returns>
		protected override InstanceCollection Distill(string workflowName, IUserIdentity userIdentity, ApprovalRole role, DateTime startDate, DateTime endDate)
		{
			List<StateMachineWorkflowInstance> instances = new List<StateMachineWorkflowInstance>();
			string userName = "";
			string fromUserId = "";
			//�ҵ�������Ŀ����ȡ��Ȩ��Id����Ȩ������
			List<ApprovalAgent> agentList = WorkflowRuntime.Current.GetService<IApprovalSaveService>().GetAgentInfoByToUser(userIdentity.GetUserId());
			foreach (ApprovalAgent agent in agentList)
			{
				if (agent.BeginDate == startDate && agent.EndDate == endDate)
				{
					userName = string.Format("{0}(��{1})", agent.ToUserName, agent.SetUserName);
					fromUserId = agent.SetUserId;
				}
			}
			//��ȡ�û�ʱ����ڵ�������¼���ҵ����ϴ�����Ϣ�ļ�¼����ȡ��Ӧ��ʵ��
			StateMachineWorkflow workflow = WorkflowRuntime.Current.GetService<IWorkFlowDefinePersistService>().GetWorkflowDefine(workflowName) as StateMachineWorkflow;
			List<ApprovalRecord> records = WorkflowRuntime.Current.GetService<IApprovalSaveService>().GetRecord(workflowName, startDate, endDate, fromUserId);
			List<Guid> ids = new List<Guid>();
			foreach (ApprovalRecord record in records)
			{
				if (record.OperatorName == userName)
					ids.Add(record.WorkflowInstanceId);
			}
			instances = new List<StateMachineWorkflowInstance>();
			foreach (Guid id in ids)
			{
				instances.Add((StateMachineWorkflowInstance)WorkflowRuntime.Current.GetInstance(id));
			}
			InstanceCollection collection = new InstanceCollection();
			foreach (StateMachineWorkflowInstance instance in instances)
			{
				if (instance.WorkflowName == workflowName)
					collection.Add(new InstanceWithRole(instance, role, false));
			}
			return collection;
		}
	}

	internal class InstanceDistillerHelper
	{
		internal static bool IsMineICanDo(ApprovalState state, ApprovalRole role)
		{
			foreach (ApprovalEvent oneEvent in state.Events)
			{
				foreach (EventRole one in oneEvent.Roles)
				{
					if (one.Name.Equals(role.Name, StringComparison.OrdinalIgnoreCase) &&
						(oneEvent.Authorization == Authorization.OwnerOnly.ToString() || oneEvent.Authorization == Authorization.All.ToString())) 
						return true;
				}
			}
			return false;
		}
		internal static bool IsAssignedICanDo(ApprovalState state, ApprovalRole role)
		{
			foreach (ApprovalEvent oneEvent in state.Events)
			{
				if (oneEvent.Authorization == Authorization.OwnerOnly.ToString()) continue;
				foreach (EventRole one in oneEvent.Roles)
				{
					if (one.Name.Equals(role.Name, StringComparison.OrdinalIgnoreCase) &&
						(oneEvent.Authorization == Authorization.All.ToString() || oneEvent.Authorization == Authorization.DenyOwner.ToString()))
						return true;
				}
			}
			return false;
		}
		internal static List<string> GetMineICanCancelStates(string workflowName, ApprovalRole role)
		{
			StateMachineWorkflow workflow = WorkflowRuntime.Current.GetService<IWorkFlowDefinePersistService>().GetWorkflowDefine(workflowName) as StateMachineWorkflow;
			List<string> states = new List<string>();
			//�õ�������ɫ���пɳ���״̬�б�
			foreach (ApprovalState state in workflow.States)
			{
				foreach (ApprovalEvent oneEvent in state.Events)
				{
					foreach (EventRole one in oneEvent.Roles)
					{
						if (one.Name.Equals(role.Name, StringComparison.OrdinalIgnoreCase) &&
						(oneEvent.Authorization == Authorization.OwnerOnly.ToString() || oneEvent.Authorization == Authorization.All.ToString()))
						{
							foreach (string nextStateName in oneEvent.NextStateNames)
							{
								if (workflow.GetStateByName(nextStateName).AllowedCancel)
								{
									if (!states.Contains(nextStateName))
										states.Add(nextStateName);
								}
							}
						}
					}
				}
			}
			return states;
		}
		internal static List<string> GetAssignedICanCancelStates(string workflowName, ApprovalRole role)
		{
			StateMachineWorkflow workflow = WorkflowRuntime.Current.GetService<IWorkFlowDefinePersistService>().GetWorkflowDefine(workflowName) as StateMachineWorkflow;
			List<string> states = new List<string>();
			//�õ�������ɫ���пɳ���״̬�б�
			foreach (ApprovalState state in workflow.States)
			{
				foreach (ApprovalEvent oneEvent in state.Events)
				{
					foreach (EventRole one in oneEvent.Roles)
					{
						if (one.Name.Equals(role.Name, StringComparison.OrdinalIgnoreCase) &&
						(oneEvent.Authorization == Authorization.All.ToString() || oneEvent.Authorization == Authorization.DenyOwner.ToString()))
						{
							foreach (string nextStateName in oneEvent.NextStateNames)
							{
								if (workflow.GetStateByName(nextStateName).AllowedCancel)
								{
									if (!states.Contains(nextStateName))
										states.Add(nextStateName);
								}
							}
						}
					}
				}
			}
			return states;
		}
	}
}

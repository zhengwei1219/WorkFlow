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
	/// 工作流实例的过滤器接口
	/// </summary>
	public abstract class InstanceFilter : ICloneable
	{
		/// <summary>
		/// 判断指定实例是否需要满足过滤条件，满足则返回True，否则返回False
		/// </summary>
		/// <param name="instance">工作流实例</param>
		/// <param name="role">用户的审批角色</param>
		/// <param name="userIdentity">用户的身份</param>
		/// <returns>
		/// 满足则返回True，否则返回False
		/// </returns>
		protected abstract bool IsMatch(StateMachineWorkflowInstance instance, ApprovalRole role, IUserIdentity userIdentity);
		/// <summary>
		/// 判断指定实例是否需要满足过滤条件，满足则返回True，否则返回False
		/// </summary>
		/// <param name="instance">工作流实例</param>
		/// <param name="role">用户的审批角色</param>
		/// <param name="userIdentity">用户的身份</param>
		/// <returns>
		/// 满足则返回True，否则返回False
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
	/// 实例集合过滤器,对一个工作流实例集合进行过滤,将满足条件的去处
	/// </summary>
	public interface IInstanceCollectionFilter
	{
		/// <summary>
		/// 对一个工作流实例集合进行过滤,将满足条件的去处
		/// </summary>
		/// <param name="instances">需要被过滤的实例集合</param>
		/// <param name="role">角色对象</param>
		/// <param name="userIdentity">身份识别标志</param>
		/// <returns>满足过滤条件的实例排除后的实例集合对象.</returns>
		InstanceCollection Filter(InstanceCollection instances, ApprovalRole role, IUserIdentity userIdentity);
	}
	/// <summary>
	/// 空的过滤器不进行任何过滤
	/// </summary>
	public class EmptyFilter : InstanceFilter
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
			return false;
		}
	}
	/// <summary>
	/// 是否制定了定其他用户专办的过滤条件判断器
	/// </summary>
	public class AssignedToOtherFilter : InstanceFilter//, IInstanceCollectionFilter
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
			List<ApprovalAssignment> assignmentList = WorkflowRuntime.Current.GetService<IApprovalSaveService>().GetAssignmentByAssignToRole(instance.WorkflowName, role.Name, instance.Id, instance.StateName);
			if (assignmentList.Count == 0)
				return false;

			return IsOthers(userIdentity, assignmentList);
		}

		private static bool IsOthers(IUserIdentity userIdentity, List<ApprovalAssignment> assignmentList)
		{
			//是否已指定同角色中他人专办标识
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
		/// ,对一个工作流实例集合进行过滤,将不满足条件的去处
		/// </summary>
		/// <param name="instances">需要被过滤的实例集合</param>
		/// <param name="role">角色对象</param>
		/// <param name="userIdentity">身份识别标志</param>
		/// <returns>满足过滤条件的实例排除后的实例集合对象.</returns>
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
	/// 排除交办外其他单位的实例
	/// </summary>
	public class AssignedToOtherUnitFilter : InstanceFilter, IInstanceCollectionFilter
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
			//是否已指定同角色中他人专办标识
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
		/// 对一个工作流实例集合进行过滤,将满足条件的去处
		/// </summary>
		/// <param name="instances">需要被过滤的实例集合</param>
		/// <param name="role">角色对象</param>
		/// <param name="userIdentity">身份识别标志</param>
		/// <returns>满足过滤条件的实例排除后的实例集合对象.</returns>
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
	/// 其他用户最后处理的要过滤掉
	/// </summary>
	public class OtherUserLastProceedFilter : InstanceFilter
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
			//最后一个Activity的执行者是否为当前用户如果是返回false
			if (instance == null)
				throw new ArgumentNullException("instance");
			StateMachineWorkflowInstance stateMachine = (StateMachineWorkflowInstance)WorkflowRuntime.Current.GetInstance(instance.Id);
			//最后一个activity为空 返回true
			if (stateMachine.LastActivity == null)
				return true;
			//最后一个activity执行者与用户身份和角色匹配，返回false
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
		/// 判断指定实例是否需要满足过滤条件，满足则返回True，否则返回False
		/// </summary>
		/// <param name="instance">工作流实例</param>
		/// <param name="role">用户的审批角色</param>
		/// <param name="userIdentity">用户的身份</param>
		/// <returns>满足则返回True，否则返回False</returns>
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
		/// 判断指定实例是否需要满足过滤条件，满足则返回True，否则返回False
		/// </summary>
		/// <param name="instance">工作流实例</param>
		/// <param name="role">用户的审批角色</param>
		/// <param name="userIdentity">用户的身份</param>
		/// <returns>满足则返回True，否则返回False</returns>
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
		/// 判断指定实例是否需要满足过滤条件，满足则返回True，否则返回False
		/// </summary>
		/// <param name="instance">工作流实例</param>
		/// <param name="role">用户的审批角色</param>
		/// <param name="userIdentity">用户的身份</param>
		/// <returns>满足则返回True，否则返回False</returns>
		protected override bool IsMatch(StateMachineWorkflowInstance instance, ApprovalRole role, IUserIdentity userIdentity)
		{
			return instance.IsEnd();
		}
	}

	/// <summary>
	/// 过滤掉已经中止的立项.
	/// </summary>
	public class TerminatedFilter : InstanceFilter
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
			return instance.IsTerminated();
		}
	}
}

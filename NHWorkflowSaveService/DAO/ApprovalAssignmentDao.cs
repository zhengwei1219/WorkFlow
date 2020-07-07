using System;
using System.Data;
using System.Configuration;
using OilDigital.Common.NHDAL.Core;
using System.Collections.Generic;
using NHibernate;
using System.Text;
using NHibernate.Criterion;

namespace OilDigital.Workflows.DAO
{
	internal class ApprovalAssignmentDao : GenericNHibernateDao<ApprovalAssignment, int>
	{
		internal List<ApprovalAssignment> GetAssignmentByAssignedRole(string workflowName, string toRoleName, Guid instanceId, string assignState)
		{
			StringBuilder queryHQL = new StringBuilder();
			queryHQL.Append("select assignment from " + persitentType.Name + " as assignment, NHStateMachineInstance as instance where assignment.WorkflowInstanceId=instance.Id and instance.WorkflowName='" + workflowName + "'");
			queryHQL.Append(" and assignment.WorkflowInstanceId='" + instanceId + "'");
			queryHQL.Append(" and assignment.ToRole='" + toRoleName + "'");
			queryHQL.Append(" and assignment.AssignState='" + assignState + "'");
			return ConvertToGenericList(session.CreateQuery(queryHQL.ToString()).List());
		}

		internal List<ApprovalAssignment> GetAssignmentByAssignedRole(string toRoleName, InstanceCollection instances)
		{
			StringBuilder queryHQL = new StringBuilder();
			queryHQL.Append("select assignment from " + persitentType.Name + " as assignment, NHStateMachineInstance as instance where");
			queryHQL.Append(" assignment.WorkflowInstanceId=instance.Id and assignment.ToRole='" + toRoleName + "'");

			StringBuilder temp = new StringBuilder();
			for (int i = 0; i < instances.Count; i++)
			{
				if (temp.Length > 0)
					temp.Append(" or ");
				temp.Append("(   instance.WorkflowName='" + instances[i].Instance.WorkflowName + "'");
				temp.Append(" and assignment.WorkflowInstanceId='" + instances[i].Instance.Id + "'");
				temp.Append(" and assignment.AssignState='" + instances[i].Instance.StateName + "')");
			}
			if (temp.Length > 0)
				queryHQL.AppendFormat(" and ({0})", temp.ToString());
			return ConvertToGenericList(session.CreateQuery(queryHQL.ToString()).List());
		}


		internal List<ApprovalAssignment> GetAssignmentByAssignState(Guid instanceId, string assignState)
		{
			ICriteria criteria = session.CreateCriteria(persitentType);
			criteria.Add(Expression.Eq("WorkflowInstanceId", instanceId));
			criteria.Add(Expression.Eq("AssignState", assignState));
			return ConvertToGenericList(criteria.List());
		}

		internal List<ApprovalAssignment> GetAssignmentByUserId(string workflowName, string assignToUserId)
		{
			StringBuilder queryHQL = new StringBuilder();
			queryHQL.Append("select assignment from " + persitentType.Name + " as assignment, NHStateMachineInstance as instance where assignment.WorkflowInstanceId=instance.Id and instance.WorkflowName='" + workflowName + "'");
			queryHQL.Append(" and assignment.ToUserId='" + assignToUserId + "'");
			return ConvertToGenericList(session.CreateQuery(queryHQL.ToString()).List());
		}

		internal List<ApprovalAssignment> GetAssignmentByToUnit(string workflowName, string unitCode)
		{
			StringBuilder queryHQL = new StringBuilder();
			queryHQL.Append("select assignment from " + persitentType.Name + " as assignment, NHStateMachineInstance as instance where assignment.WorkflowInstanceId=instance.Id and instance.WorkflowName='" + workflowName + "'");
			queryHQL.Append(" and assignment.ToUnitCode='" + unitCode + "'");
			return ConvertToGenericList(session.CreateQuery(queryHQL.ToString()).List());
		}

		internal List<ApprovalAssignment> GetAssignment(Guid instanceId)
		{
			ICriteria criteria = session.CreateCriteria(persitentType);
			criteria.Add(Expression.Eq("WorkflowInstanceId", instanceId));
			return ConvertToGenericList(criteria.List());
		}

		internal List<ApprovalAssignment> GetAssignment(Guid[] instanceIDs)
		{
			ICriteria criteria = session.CreateCriteria(persitentType);
			criteria.Add(Expression.In("WorkflowInstanceId", instanceIDs));
			return ConvertToGenericList(criteria.List());
		}

		internal List<ApprovalAssignment> GetAssignment(string workflowName, int eaId)
		{
			StringBuilder queryHQL = new StringBuilder();
			queryHQL.Append("select assignment from " + persitentType.Name + " as assignment, NHStateMachineInstance as instance where assignment.WorkflowInstanceId=instance.Id and instance.WorkflowName='" + workflowName + "'");
			queryHQL.Append(" and assignment.EaId='" + eaId + "'");
			return ConvertToGenericList(session.CreateQuery(queryHQL.ToString()).List());
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using OilDigital.Common.NHDAL.Core;
using NHibernate;
using NHibernate.Criterion;

namespace OilDigital.Workflows.DAO
{
	internal class ApprovalRecordDao : GenericNHibernateDao<ApprovalRecord, int>
	{
		internal List<ApprovalRecord> GetRecord(Guid instanceId)
		{
			ICriteria criteria = session.CreateCriteria(persitentType);
			criteria.Add(Expression.Eq("WorkflowInstanceId", instanceId));
			criteria.AddOrder(Order.Desc("OperatorTime"));
			return ConvertToGenericList(criteria.List());
		}

		internal List<ApprovalRecord> GetRecord(string workflowName, int eaId)
		{
			IQuery query = session.CreateQuery("select record from " + persitentType.Name + " as record, NHStateMachineInstance as instance where record.WorkflowInstanceId=instance.Id and instance.WorkflowName='" + workflowName + "' and record.EaId=" + eaId);
			return ConvertToGenericList(query.List());
		}

		internal List<ApprovalRecord> GetRecord(string workflowName, DateTime startDate, DateTime endDate, string userId)
		{
			StringBuilder queryHQL = new StringBuilder();
			queryHQL.Append("select record from " + persitentType.Name + " as record, NHStateMachineInstance as instance where record.WorkflowInstanceId=instance.Id and instance.WorkflowName='" + workflowName + "'");
			queryHQL.Append(" and record.OperatorTime>='" + startDate.ToString("yyyy-MM-dd HH:mm:ss") + "'");
			queryHQL.Append(" and record.OperatorTime<='" + endDate.ToString("yyyy-MM-dd HH:mm:ss") + "'");
			if (!string.IsNullOrEmpty(userId))
				queryHQL.Append(" and record.OperatorId='" + userId + "'");
			return ConvertToGenericList(session.CreateQuery(queryHQL.ToString()).List());
		}

		internal List<ApprovalRecord> GetRecord(string workflowName, DateTime startDate, DateTime endDate, string userId, string unitCode, string roleName)
		{
			StringBuilder queryHQL = new StringBuilder();
			queryHQL.Append("select record from " + persitentType.Name + " as record, NHStateMachineInstance as instance where record.WorkflowInstanceId=instance.Id and instance.WorkflowName='" + workflowName + "'");
			queryHQL.Append(" and record.OperatorTime>='" + startDate.ToString("yyyy-MM-dd HH:mm:ss") + "'");
			queryHQL.Append(" and record.OperatorTime<='" + endDate.ToString("yyyy-MM-dd HH:mm:ss") + "'");
			if (!string.IsNullOrEmpty(userId))
				queryHQL.Append(" and record.OperatorId='" + userId + "'");
			if (!string.IsNullOrEmpty(unitCode))
				queryHQL.Append(" and record.OperatorUnitCode='" + unitCode + "'");
			if (!string.IsNullOrEmpty(roleName))
				queryHQL.Append(" and record.OperatorRole='" + roleName + "'");
			return ConvertToGenericList(session.CreateQuery(queryHQL.ToString()).List());
		}
	}
}

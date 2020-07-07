using System;
using System.Collections.Generic;
using System.Text;
using OilDigital.Common.NHDAL.Core;
using NHibernate;
using NHibernate.Criterion;

namespace OilDigital.Workflows.DAO
{
	internal class ApprovalSolutionDao : GenericNHibernateDao<ApprovalSolution, int>
	{
		internal List<ApprovalSolution> GetSolution(Guid instanceId)
		{
			ICriteria criteria = session.CreateCriteria(persitentType);
			criteria.Add(Expression.Eq("WorkflowInstanceId", instanceId));
			criteria.AddOrder(Order.Desc("OperatorTime"));
			return ConvertToGenericList(criteria.List());
		}

		internal List<ApprovalSolution> GetSolution(string workflowName, int eaId)
		{
			IQuery query = session.CreateQuery("select solution from " + persitentType.Name + " as solution, NHStateMachineInstance as instance where solution.WorkflowInstanceId=instance.Id and instance.WorkflowName='" + workflowName + "' and solution.EaId=" + eaId);
			return ConvertToGenericList(query.List());
		}
	}
}
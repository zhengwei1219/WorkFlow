using System;
using System.Collections.Generic;
using System.Text;
using OilDigital.Common.NHDAL.Core;
using NHibernate;
using NHibernate.Criterion;

namespace OilDigital.Workflows.DAO
{
	internal class NHStateMachineInstanceDao : GenericNHibernateDao<NHStateMachineInstance, Guid>
	{
		internal List<NHStateMachineInstance> GetInstanceByStateName(string workflowName, string[] stateNames, string typeName)
		{
			ICriteria criteria = session.CreateCriteria(persitentType);
			criteria.Add(Expression.Eq("WorkflowName", workflowName));
			criteria.Add(Expression.In("StateName", stateNames));
			criteria.Add(Expression.Eq("TypeName", typeName));
			criteria.AddOrder(Order.Asc("StateName"));
			criteria.AddOrder(Order.Desc("PersistTime"));
			return ConvertToGenericList(criteria.List());
		}
		internal List<NHStateMachineInstance> GetInstanceById(Guid instanceId)
		{
			ICriteria criteria = session.CreateCriteria(persitentType);
			criteria.Add(Expression.Eq("Id", instanceId));
			criteria.AddOrder(Order.Desc("PersistTime"));
			return ConvertToGenericList(criteria.List());
		}

		internal List<NHStateMachineInstance> GetInstanceByPersisteTime(string workflowName, DateTime startDate, DateTime endDate, string[] stateNames)
		{
			ICriteria criteria = session.CreateCriteria(persitentType);
			criteria.Add(Expression.Eq("WorkflowName", workflowName));
			criteria.Add(Expression.Ge("PersistTime", startDate));
			criteria.Add(Expression.Le("PersistTime", endDate));
			if (stateNames!=null && stateNames.Length>0)
				criteria.Add(Expression.Eq("StateName", stateNames));
			criteria.AddOrder(Order.Asc("StateName"));
			criteria.AddOrder(Order.Desc("PersistTime"));
			return ConvertToGenericList(criteria.List());
		}

		internal List<NHStateMachineInstance> GetInstanceByPersisteTime(string workflowName, DateTime startDate, DateTime endDate, string[] stateNames, string unitCode)
		{
			ICriteria criteria = session.CreateCriteria(persitentType);
			criteria.Add(Expression.Eq("WorkflowName", workflowName));
			if (!string.IsNullOrEmpty(unitCode))
				criteria.Add(Expression.Like("CreaterUnit", unitCode));
			criteria.Add(Expression.Ge("PersistTime", startDate));
			criteria.Add(Expression.Le("PersistTime", endDate));
			if (stateNames != null && stateNames.Length > 0)
				criteria.Add(Expression.In("StateName", stateNames));
			criteria.AddOrder(Order.Asc("StateName"));
			criteria.AddOrder(Order.Desc("PersistTime"));
			return ConvertToGenericList(criteria.List());
		}

		internal List<NHStateMachineInstance> GetUserInstance(string workflowName, string[] stateNames, string unitCode)
		{
			ICriteria criteria = session.CreateCriteria(persitentType);
			criteria.Add(Expression.Eq("WorkflowName", workflowName));
			if (stateNames != null && stateNames.Length > 0)
				criteria.Add(Expression.In("StateName", stateNames));
			criteria.Add(Expression.Like("CreaterUnit", unitCode));
			criteria.AddOrder(Order.Asc("StateName"));
			criteria.AddOrder(Order.Desc("PersistTime"));
			return ConvertToGenericList(criteria.List());
		}

		internal List<NHStateMachineInstance> GetUserInstance(string workflowName, string[] stateNames, string createrUserId, string unitCode)
		{
			ICriteria criteria = session.CreateCriteria(persitentType);
			criteria.Add(Expression.Eq("WorkflowName", workflowName));
			if (stateNames != null && stateNames.Length > 0)
				criteria.Add(Expression.In("StateName", stateNames));
			criteria.Add(Expression.Eq("CreaterUserId", createrUserId));
			criteria.Add(Expression.Like("CreaterUnit", unitCode));
			criteria.AddOrder(Order.Asc("StateName"));
			criteria.AddOrder(Order.Desc("PersistTime"));
			return ConvertToGenericList(criteria.List());
		}

		internal List<NHStateMachineInstance> GetUserInstance(string workflowName, string createrUserId)
		{
			ICriteria criteria = session.CreateCriteria(persitentType);
			criteria.Add(Expression.Eq("WorkflowName", workflowName));
			criteria.Add(Expression.Eq("CreaterUserId", createrUserId));
			criteria.AddOrder(Order.Desc("PersistTime"));
			return ConvertToGenericList(criteria.List());
		}

		internal List<NHStateMachineInstance> GetUserInstance(string workflowName, int eaId)
		{
			ICriteria criteria = session.CreateCriteria(persitentType);
			criteria.Add(Expression.Eq("EaId", eaId));
			criteria.Add(Expression.Eq("WorkflowName", workflowName));
			criteria.AddOrder(Order.Desc("PersistTime"));
			return ConvertToGenericList(criteria.List());
		}
	}
}

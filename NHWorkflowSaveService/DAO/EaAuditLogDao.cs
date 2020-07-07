using System;
using System.Data;
using System.Configuration;
using OilDigital.Common.NHDAL.Core;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;

namespace OilDigital.Workflows.DAO
{
	/// <summary>
	/// 
	/// </summary>
	public class EaAuditLogDao : GenericNHibernateDao<EaAuditLog, long>
	{
		/// <summary>
		/// Gets the audit log by ea id.
		/// </summary>
		/// <param name="eaId">The ea id.</param>
		/// <returns></returns>
		public List<EaAuditLog> GetAuditLogByEaId(int eaId)
		{
			ICriteria criteria = session.CreateCriteria(persitentType);
			criteria.Add(Expression.Eq("ClassName", "БўПо"));
			criteria.Add(Expression.Eq("EaId", eaId));
			criteria.AddOrder(Order.Desc("UpdateTime"));
			return ConvertToGenericList(criteria.List());
		}
	}
}

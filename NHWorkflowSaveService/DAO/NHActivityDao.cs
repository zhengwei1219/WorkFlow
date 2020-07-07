using System;
using System.Collections.Generic;
using System.Text;
using OilDigital.Common.NHDAL.Core;
using NHibernate;
using NHibernate.Criterion;

namespace OilDigital.Workflows.DAO
{
	internal class NHActivityDao : GenericNHibernateDao<NHActivity, long>
	{
		internal List<NHActivity> GetNHActivity(string name)
		{
			ICriteria criteria = session.CreateCriteria(persitentType);
			criteria.Add(Expression.Eq("Name", name));
			return ConvertToGenericList(criteria.List());
		}

		internal List<NHActivity> GetNHActivity()
		{
			ICriteria criteria = session.CreateCriteria(persitentType);
			criteria.Add(Expression.Eq("ParentState", null));
			return ConvertToGenericList(criteria.List());
		}

		/// <summary>
		/// Gets the NH activitys.
		/// </summary>
		/// <param name="ids">The ids.</param>
		/// <returns></returns>
		internal List<NHActivity> GetNHActivitys(long[] ids)
		{
			ICriteria criteria = session.CreateCriteria(persitentType);
			criteria.Add(Expression.In("Id", ids));
			return ConvertToGenericList(criteria.List());
		}
	}
}

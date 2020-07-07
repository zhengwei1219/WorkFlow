using System;
using System.Data;
using System.Configuration;
using OilDigital.Common.NHDAL.Core;
using NHibernate;
using System.Collections.Generic;
using NHibernate.Criterion;

namespace OilDigital.Workflows.DAO
{
	/// <summary>
	/// 
	/// </summary>
	internal class ApprovalAgentDao : GenericNHibernateDao<ApprovalAgent, int>
	{
		/// <summary>
		/// Gets the approval agent by user id.
		/// </summary>
		/// <param name="setAgentUserId">The set agent user id.</param>
		/// <returns></returns>
		internal List<ApprovalAgent> GetApprovalAgentByUserId(string setAgentUserId)
		{
			ICriteria criteria = session.CreateCriteria(persitentType);
			criteria.Add(Expression.Eq("SetUserId", setAgentUserId));
			criteria.AddOrder(Order.Desc("EndDate"));
			return ConvertToGenericList(criteria.List());
		}
		/// <summary>
		/// Gets the valid agent info by to user.
		/// </summary>
		/// <param name="toUserId">To user id.</param>
		/// <returns></returns>
		internal List<ApprovalAgent> GetValidAgentInfoByToUser(string toUserId)
		{
			ICriteria criteria = session.CreateCriteria(persitentType);
			criteria.Add(Expression.Eq("ToUserId", toUserId));
			criteria.Add(Expression.Le("BeginDate", DateTime.Now));
			criteria.Add(Expression.Ge("EndDate", DateTime.Now));
			criteria.AddOrder(Order.Desc("EndDate"));
			return ConvertToGenericList(criteria.List());
		}
		/// <summary>
		/// Gets the agent info by to user.
		/// </summary>
		/// <param name="toUserId">To user id.</param>
		/// <returns></returns>
		internal List<ApprovalAgent> GetAgentInfoByToUser(string toUserId)
		{
			ICriteria criteria = session.CreateCriteria(persitentType);
			criteria.Add(Expression.Eq("ToUserId", toUserId));
			criteria.AddOrder(Order.Desc("EndDate"));
			return ConvertToGenericList(criteria.List());
		}
		/// <summary>
		/// Gets all agent info.
		/// </summary>
		/// <returns></returns>
		internal List<ApprovalAgent> GetAllAgentInfo()
		{
			ICriteria criteria = session.CreateCriteria(persitentType);
			criteria.Add(Expression.Ge("Id", 1));
			criteria.AddOrder(Order.Desc("EndDate"));
			return ConvertToGenericList(criteria.List());
		}
	}
}

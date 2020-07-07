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
	/// 基于Nhibernate定制意见的存取.
	/// </summary>
	internal class ApprovalCommentDao : GenericNHibernateDao<ApprovalComment, int>
	{
		/// <summary>
		/// Gets the user comment info.
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <param name="approvalType">Type of the approval.</param>
		/// <returns></returns>
		public List<ApprovalComment> GetUserCommentInfo(string userId, string approvalType)
		{
			ICriteria criteria = session.CreateCriteria(persitentType);
			criteria.Add(Expression.Eq("OwnerUserId", userId));
			criteria.Add(Expression.Eq("ApprovalType", approvalType));
			return ConvertToGenericList(criteria.List());
		}

		/// <summary>
		/// Gets the comment info.
		/// </summary>
		/// <param name="commentInfo">The comment info.</param>
		/// <param name="userId">The user id.</param>
		/// <param name="approvalType">Type of the approval.</param>
		/// <returns></returns>
		public List<ApprovalComment> GetCommentInfo(string commentInfo, string userId, string approvalType)
		{
			ICriteria criteria = session.CreateCriteria(persitentType);
			criteria.Add(Expression.Eq("CommentInfo", commentInfo));
			criteria.Add(Expression.Eq("OwnerUserId", userId));
			criteria.Add(Expression.Eq("ApprovalType", approvalType));
			return ConvertToGenericList(criteria.List());
		}
	}
}

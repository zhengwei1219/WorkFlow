using System;
using System.Collections.Generic;
using System.Text;
using OilDigital.Workflows;
using NHibernate;
using OilDigital.Common.NHDAL.Core;

namespace OilDigital.Workflows.DAO
{
	/// <summary>
	/// 
	/// </summary>
	public class NHTransactionService : ITransactionService
	{
		#region ITransactionService Members

		/// <summary>
		/// 开启一个事务
		/// </summary>
		public void BeginTransaction()
		{
			//NHibernateSessionManager.Instance.BeginTransaction();
		}

		/// <summary>
		/// 提交事务
		/// </summary>
		public void CommitTransaction()
		{
			NHibernateSessionManager.Instance.CommitTransaction();
		}

		/// <summary>
		/// 滚回事务
		/// </summary>
		public void RollbackTransaction()
		{
			NHibernateSessionManager.Instance.RollbackTransaction();
		}

		#endregion
	}
}

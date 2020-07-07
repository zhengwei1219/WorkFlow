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
		/// ����һ������
		/// </summary>
		public void BeginTransaction()
		{
			//NHibernateSessionManager.Instance.BeginTransaction();
		}

		/// <summary>
		/// �ύ����
		/// </summary>
		public void CommitTransaction()
		{
			NHibernateSessionManager.Instance.CommitTransaction();
		}

		/// <summary>
		/// ��������
		/// </summary>
		public void RollbackTransaction()
		{
			NHibernateSessionManager.Instance.RollbackTransaction();
		}

		#endregion
	}
}

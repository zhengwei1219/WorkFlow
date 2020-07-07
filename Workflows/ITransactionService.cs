using System;
using System.Collections.Generic;
using System.Text;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 
	/// </summary>
	public interface ITransactionService
	{
		/// <summary>
		/// 开启一个事务
		/// </summary>
		void BeginTransaction();

		/// <summary>
		/// 提交事务
		/// </summary>
		void CommitTransaction();

		/// <summary>
		/// 滚回事务
		/// </summary>
		void RollbackTransaction();
	}
}

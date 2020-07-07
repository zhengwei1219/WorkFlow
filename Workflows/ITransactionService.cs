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
		/// ����һ������
		/// </summary>
		void BeginTransaction();

		/// <summary>
		/// �ύ����
		/// </summary>
		void CommitTransaction();

		/// <summary>
		/// ��������
		/// </summary>
		void RollbackTransaction();
	}
}

using System;
using System.Collections.Generic;
using System.Text;

namespace OilDigital.Workflows
{
	/// <summary>
	/// ��֧�����ӿ�
	/// </summary>
	public interface IBranchAction
	{

		/// <summary>
		/// ��ȡ�ڶ���ִ������Ժ�,����ѡ����״̬
		/// </summary>
		/// <returns></returns>
		string GetSelectedState();
	}
}

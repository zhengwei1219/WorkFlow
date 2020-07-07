using System;
using System.Collections.Generic;
using System.Text;

namespace OilDigital.Workflows
{
	/// <summary>
	/// ���ڶ�����������в����ķ���
	/// <remarks>�����������г������̱����Լ�������ر��������޸���,������Ҫ��������������,�����޸����������״̬,ɾ�����������.</remarks>
	/// </summary>
	public interface IApprovalObjectService
	{
		/// <summary>
		/// ����ID��ȡ��������
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		ICanBeApproval GetByID(string id);

		/// <summary>
		/// ������������ĸĶ�.
		/// </summary>
		/// <param name="obj">The obj.</param>
		void Save(ICanBeApproval obj);


		/// <summary>
		/// ɾ������������.
		/// </summary>
		/// <param name="obj">The obj.</param>
		void Delete(ICanBeApproval obj);
	}
}

using System;
using System.Collections.Generic;
using System.Text;

namespace OilDigital.Workflows
{
	/// <summary>
	/// ��������ӿ�
	/// </summary>
	public interface IApprovalRules
	{
		/// <summary>
		/// ������ɫ
		/// </summary>
		ApprovalRole UserRole { get;}
		/// <summary>
		/// ������ɫ�������������Ľ�ɫ
		/// </summary>
		bool IsCreator();
		/// <summary>
		/// ��ɫ�Ƿ��Ƕ�����λ��ɫ
		/// </summary>
		bool IsSubUnitRole();
		/// <summary>
		/// �ж�������ɫ�Ƿ�����������λ��ɫ
		/// </summary>
		bool IsApprovalDepRole();
		/// <summary>
		/// ��ɫ�Ƿ���������ɫ
		/// </summary>
		bool IsApprovalRole();
		/// <summary>
		/// �Ƿ��ǽ���������ɫ
		/// </summary>
		bool IsFinisher();
	}
}

using System;
using System.Collections.Generic;
using System.Text;

namespace OilDigital.Workflows
{
	/// <summary>
	/// �ӹ�����ʵ����������ȡ����ִ�еĶ���
	/// </summary>
	public interface ITaskItemDistiller
	{
		
		/// <summary>
		/// ��ȡʵ���Ĳ�����Ŀ����
		/// </summary>
		/// <param name="collection">ʵ������:���ĳһ���ض�����������(���ձ�����������кϲ����,��������ʵ����Ӧ�Ĺ���������)</param>
		/// <param name="userIdentity">�û����</param>
		/// <returns></returns>
		ITaskItem Distill(InstanceCollection collection, IUserIdentity userIdentity);
		/// <summary>
		/// ��ȡʵ��������Ŀ����
		/// </summary>
		/// <param name="instanceWithRole">ʵ��</param>
		/// <param name="userIdentity">�û����</param>
		/// <returns></returns>
		//List<ITaskActionItem> Distill(InstanceWithRole instanceWithRole, IUserIdentity userIdentity);
	}

	/// <summary>
	/// �ɱ���ʼ����Distiller
	/// </summary>
	public interface IInitializableDistiller
	{
		/// <summary>
		/// �ڵ���Distill �������жԵ�������ʵ��������ȡǰ,��Ա�����ȡ���е�һЩ��ʼ������.
		/// �������ش˷���,����һЩ��ʼ������.
		/// </summary>
		/// <param name="ForRole">��ͬ��������ɫ�в�ͬ��ITaskItemDistiller,ForRole��ʾ��TaskDistill��Ӧ��������ɫ����,<seealso cref="ApprovalRole"/>��ʵ��</param>
		/// <param name="allCollection">������ȡ�����й�����ʵ���ļ���,ע��˼�����,������������������ж������ʵ��</param>
		/// <value>For role.</value>
		void Initialize(ApprovalRole ForRole, InstanceCollection allCollection);

	}
}

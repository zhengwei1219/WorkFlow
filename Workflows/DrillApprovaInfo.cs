using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;

namespace OilDigital.Workflows
{
	/// <summary>
	/// ��ȡ�û�������Ϣ��һ����
	/// </summary>
	/// <typeparam name="T">�����滻�����˵��ļ�¼������</typeparam>
	public class DrillApprovalInfo<T>
	{
		/// <summary>
		/// ����һ����ȡ������Ϣ��ͨ����,����ʾ�κα����˵��ļ�¼
		/// </summary>
		public DrillApprovalInfo() : this(false, default(T)) { }

		/// <summary>
		/// ����һ����ȡ������Ϣ��ͨ���ಢ���ض��ļ�¼���滻�������˵ļ�¼
		/// </summary>
		/// <param name="hidden">�����滻�����˵��ļ�¼���滻��¼</param>
		public DrillApprovalInfo(T hidden) : this( true, hidden) { }

		/// <summary>
		/// ����һ����ȡ������Ϣ��ͨ����
		/// </summary>
		/// <param name="showFiltered">�Ƿ�������ʾ�����˵��ļ�¼</param>
		/// <param name="hidden">�����滻�����˵��ļ�¼���滻��¼</param>
		public DrillApprovalInfo( bool showFiltered, T hidden)
		{
			
			this.showFiltered = showFiltered;
			this.hidden = hidden;
			//��ʼ����filter
			filters = new List<IFilter<T>>();
			//��ȡ�־û�ʵ��
			approvalService = WorkflowRuntime.Current.GetService<IApprovalSaveService>();
		}

		//�����־û���ʵ��
		/// <summary>
		/// 
		/// </summary>
		protected IApprovalSaveService approvalService;

		/// <summary>
		/// 
		/// </summary>
		protected List<IFilter<T>> filters;

		/// <summary>
		/// ���˽ӿڼ���
		/// </summary>
		public List<IFilter<T>> Filters
		{
			get { return filters; }
			set { filters = value; }
		}

		private bool showFiltered;

		/// <summary>
		/// �Ƿ���ʾ�����˵ļ�¼
		/// </summary>
		public bool ShowFiltered
		{
			get { return showFiltered; }
			set { showFiltered = value; }
		}

		/// <summary>
		/// ��ʾ�����˵ļ�¼��ʱ��,�滻��ʾ������
		/// </summary>
		private T hidden;
		/// <summary>
		/// ��ȡָ��������ʵ��Id��������¼
		/// </summary>
		/// <param name="applicationName">Name of the application.</param>
		/// <param name="eaId">The ea id.</param>
		/// <returns></returns>
		public List<T> GetInfo(string applicationName, int eaId)
		{
			if (eaId <= 0)
				throw new ArgumentException("eaid must be greater than zero in GetRecordInfo(eaid)", "eaid");

			//��ȡ����������¼
			List<T> recordList = GetAllApprovalInfoById(applicationName, eaId);

			for (int i = recordList.Count -1; i >=0;i--)
			{
				//û�ж��������,��ô����
				if(filters.Count==0) continue;

				bool isKept = false;
				//����ɸѡ������ɸѡ
				for (int j = 0; j < filters.Count; j++)
				{
					isKept = isKept || (!filters[j].IsFiltered(recordList[i]));
					if (isKept) break;
				}

				//�����˵���
				if (!isKept)
				{
					if (showFiltered)
					{
						//recordList[i] = GetHiddenOnShow(recordList[i]);
						recordList[i] = hidden;
					}
					else
						recordList.RemoveAt(i);
				}
			}
			return recordList;
		}

		/*
		protected virtual T GetHiddenOnShow(T record)
		{
 			if (record is IOperatorRelative)
			{
				((IOperatorRelative)hidden).OperatorTime=((IOperatorRelative)record).OperatorTime;
			}
			return hidden;
		}
		*/
		/// <summary>
		/// ��ȡȫ��������Ϣ�ķ���,���뱻������������
		/// </summary>
		/// <param name="applicationName">Name of the application.</param>
		/// <param name="eaid">The eaid.</param>
		/// <returns></returns>
		protected virtual List<T> GetAllApprovalInfoById(string applicationName, int eaid)
		{
			return null;
		}
	}
}

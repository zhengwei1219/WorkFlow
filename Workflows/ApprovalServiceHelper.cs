using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.IO;
using System.Xml.Serialization;

namespace OilDigital.Workflows
{
	/// <summary>
	/// Ӧ�ó���
	/// </summary>
	public class ApprovalServiceHelper
	{
		/// <summary>
		/// Initializes the <see cref="ApprovalService"/> class.
		/// </summary>
		static ApprovalServiceHelper()
		{
			string root = AppDomain.CurrentDomain.BaseDirectory;
			filePath = root + @"\config\Approval.config";
		}

		/// <summary>
		/// �������������ļ�����·��.ȱʡΪ:Ӧ�ó���Ŀ¼\config\Approval.config
		/// </summary>
		private static string filePath;
		/// <summary>
		/// �����ļ�����·��(����·��).
		/// </summary>
		/// <value>The file path.</value>
		public static void SetConfigFilePath(string configFilePath)
		{
			filePath = configFilePath;
		}

		private static ApprovalServicesCollection serviceList = new ApprovalServicesCollection();
		/// <summary>
		/// ����ϵͳ��������Ӧ�÷���ļ���
		/// </summary>
		/// <value>The services.</value>
		public static ApprovalServicesCollection Services
		{
			get
			{
				Init();
				return serviceList;
			}
		}
		/// <summary>
		/// �����ϴμ��ص�ʱ��.
		/// </summary>
		private static DateTime dateTime = DateTime.MinValue;
		private static void Init()
		{
			//�ж��ļ�ʱ������ȷ���Ƿ������½����ļ�
			if (!File.Exists(filePath))
				throw new ConfigurationErrorsException(filePath + "does't exsist��");
			DateTime date = File.GetLastWriteTime(filePath);
			if (date > dateTime)
			{
				ApprovalServiceConfig config = null;
				try
				{
					serviceList.Clear();
					XmlSerializer serializer = new XmlSerializer(typeof(ApprovalServiceConfig));
					using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
					{
						config = (ApprovalServiceConfig)serializer.Deserialize(fs);
						foreach (ApplicationConfig appConfig in config.Applications)
						{
							ApprovalService service = new ApprovalService(appConfig);
							serviceList.Add(service);
						}
					}
				}
				catch (Exception ep)
				{
					throw new ConfigurationErrorsException("ApprovalService configuration error! " + ep.Message);
				}

				dateTime = date;
			}
		}

		#region ��ȡ�����б���صķ���
		/// <summary>
		/// ���õĻ�ȡ��ǰ�û��Ĵ��������б�
		/// </summary>
		/// <param name="userId">�û���Id</param>
		/// <returns>���������б����ɵ�HTML�ַ���</returns>
		public static string GetTaskList(string userId)
		{
			StringBuilder list = new StringBuilder();
			foreach (ApprovalService service in Services)
			{
				list.Append(service.CurrentlyService.Distill(userId));
			}
			return list.ToString();
		}
		/// <summary>
		/// ��ȡί�д����˵������б�
		/// </summary>
		/// <param name="userId">������Id</param>
		/// <returns>���������б����ɵ�HTML�ַ���</returns>
		public static string GetAgentTaskList(string userId)
		{
			StringBuilder list = new StringBuilder();
			foreach (ApprovalService service in Services)
			{
				list.Append(service.CurrentlyService.Distill(userId));
			}
			return list.ToString();
		}
		/// <summary>
		/// ��ȡ��ǰ��¼�û��Ŀɳ��������б�
		/// </summary>
		/// <param name="userId">�û���Id</param>
		/// <returns>���������б����ɵ�HTML�ַ���</returns>
		public static string GetAllowedCancelTaskList(string userId)
		{
			StringBuilder list = new StringBuilder();
			foreach (ApprovalService service in Services)
			{
				list.Append(service.AllowedCancelService.Distill(userId));
			}
			return list.ToString();
		}
		/// <summary>
		/// ��ȡ�����û��Ŀɳ��������б�
		/// </summary>
		/// <param name="userId">�����˵�Id</param>
		/// <returns>���������б����ɵ�HTML�ַ���</returns>
		public static string GetAgentAllowedCancelTaskList(string userId)
		{
			StringBuilder list = new StringBuilder();
			foreach (ApprovalService service in Services)
			{
				list.Append(service.AllowedCancelService.Distill(userId));
			}
			return list.ToString();
		}
		/// <summary>
		/// ��ȡ���������������б�
		/// </summary>
		/// <param name="userId">�û���Id</param>
		/// <param name="startDate">����ʱ�䣨�ӣ�</param>
		/// <param name="endDate">����ʱ�䣨����</param>
		/// <returns>���������б����ɵ�HTML�ַ���</returns>
		public static string GetProcessedTaskList(string userId, DateTime startDate, DateTime endDate)
		{
			StringBuilder list = new StringBuilder();
			foreach (ApprovalService service in Services)
			{
				list.Append(service.ProceedService.Distill(userId, startDate, endDate));
			}
			return list.ToString();
		}
		/// <summary>
		/// ��ȡ���������������б�
		/// </summary>
		/// <param name="userId">�û���Id</param>
		/// <param name="startDate">����ʱ�䣨�ӣ�</param>
		/// <param name="endDate">����ʱ�䣨����</param>
		/// <returns>���������б����ɵ�HTML�ַ���</returns>
		public static string GetProcessingTaskList(string userId, DateTime startDate, DateTime endDate)
		{
			StringBuilder list = new StringBuilder();
			foreach (ApprovalService service in Services)
			{
				list.Append(service.ProcessingService.Distill(userId, startDate, endDate));
			}
			return list.ToString();
		}
		/// <summary>
		/// ��ȡ���������������б�
		/// </summary>
		/// <param name="userId">�û���Id</param>
		/// <param name="startDate">����ʱ�䣨�ӣ�</param>
		/// <param name="endDate">����ʱ�䣨����</param>
		/// <returns>���������б����ɵ�HTML�ַ���</returns>
		public static string GetCompletedTaskList(string userId, DateTime startDate, DateTime endDate)
		{
			StringBuilder list = new StringBuilder();
			foreach (ApprovalService service in Services)
			{
				list.Append(service.CompletedService.Distill(userId, startDate, endDate));
			}
			return list.ToString();
		}
		#endregion


		#region ������صķ���
		/// <summary>
		///ɾ���Ѿ����ڵĴ�����Ϣ
		/// </summary>
		/// <param name="agentList">The agent list.</param>
		private static void RemoveExpiredAgent(List<ApprovalAgent> agentList)
		{
			DateTime now = DateTime.Now;
			for (int i = agentList.Count - 1; i >= 0; i--)
			{
				if (agentList[i].EndDate < now)
					agentList.RemoveAt(i);
			}
		}
		/// <summary>
		/// ��ȡ����ָ����Ա������ļ�¼
		/// </summary>
		/// <param name="userId">���������Ա,��A��Ȩ��Bʱ,ΪB</param>
		/// <param name="showExpired">if set to <c>true</c> [show expired].</param>
		/// <returns></returns>
		public static List<ApprovalAgent> GetAgentsToUser(string toUser, bool showExpired)
		{
			List<ApprovalAgent> rtn = WorkflowRuntime.Current.GetService<IApprovalSaveService>().GetAgentInfoByToUser(toUser);
			if (!showExpired) RemoveExpiredAgent(rtn);
			return rtn;
		}

		/// <summary>
		/// ��ȡ����ָ����Աί�ɸ����˵ļ�¼
		/// </summary>
		/// <param name="fromUser">���������ں���,A��Ȩ��B����,��ôΪA</param>
		/// <param name="showExpired">�Ƿ���ʾ�Ѿ����ڵ���Ȩ</param>
		/// <returns></returns>
		public static List<ApprovalAgent> GetAgentsFromUser(string fromUser, bool showExpired)
		{
			List<ApprovalAgent> agentList = WorkflowRuntime.Current.GetService<IApprovalSaveService>().GetAgentInfoByFromUser(fromUser);
			if (!showExpired) RemoveExpiredAgent(agentList);
			return agentList;
		}

		/// <summary>
		/// ɾ��ĳ��Ȩ��Ϣ:1)�������������Ѿ���ʼ��Ч,����δ��ʵЧ����,��ô����������ʧЧʱ������Ϊ��ǰ����.
		/// 2)����������������Ѿ���ʼ��Ч,���Թ���ʵЧ����,��ôʲôҲ����.
		/// 3)����ô�����δ��Ч,��ôֱ��ɾ��.
		/// </summary>
		/// <param name="agentInfo">The agent info.</param>
		public static void DeleteAgentInfo(ApprovalAgent agentInfo)
		{
			IApprovalSaveService service = WorkflowRuntime.Current.GetService<IApprovalSaveService>();
			if (agentInfo.BeginDate >= DateTime.Now)
			{
				service.DeleteAgentInfoById(agentInfo.Id);
			}
			else if (agentInfo.EndDate > DateTime.Now)
			{
				agentInfo.EndDate = DateTime.Now;
				service.UpdateAgentInfo(agentInfo);
			}
		}
		#endregion
	}
}

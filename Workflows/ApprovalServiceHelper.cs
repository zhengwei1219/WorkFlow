using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.IO;
using System.Xml.Serialization;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 应用程序
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
		/// 审批服务配置文件所在路径.缺省为:应用程序目录\config\Approval.config
		/// </summary>
		private static string filePath;
		/// <summary>
		/// 配置文件所在路径(物理路径).
		/// </summary>
		/// <value>The file path.</value>
		public static void SetConfigFilePath(string configFilePath)
		{
			filePath = configFilePath;
		}

		private static ApprovalServicesCollection serviceList = new ApprovalServicesCollection();
		/// <summary>
		/// 包含系统所有审批应用服务的集合
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
		/// 保留上次加载的时间.
		/// </summary>
		private static DateTime dateTime = DateTime.MinValue;
		private static void Init()
		{
			//判断文件时间属性确定是否需重新解析文件
			if (!File.Exists(filePath))
				throw new ConfigurationErrorsException(filePath + "does't exsist！");
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

		#region 获取任务列表相关的方法
		/// <summary>
		/// 调用的获取当前用户的待办任务列表
		/// </summary>
		/// <param name="userId">用户的Id</param>
		/// <returns>返回任务列表生成的HTML字符串</returns>
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
		/// 获取委托代办人的任务列表
		/// </summary>
		/// <param name="userId">代办人Id</param>
		/// <returns>返回任务列表生成的HTML字符串</returns>
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
		/// 获取当前登录用户的可撤销任务列表
		/// </summary>
		/// <param name="userId">用户的Id</param>
		/// <returns>返回任务列表生成的HTML字符串</returns>
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
		/// 获取代办用户的可撤销任务列表
		/// </summary>
		/// <param name="userId">代办人的Id</param>
		/// <returns>返回任务列表生成的HTML字符串</returns>
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
		/// 获取已审批过的立项列表
		/// </summary>
		/// <param name="userId">用户的Id</param>
		/// <param name="startDate">办理时间（从）</param>
		/// <param name="endDate">办理时间（到）</param>
		/// <returns>返回任务列表生成的HTML字符串</returns>
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
		/// 获取正在审批中立项列表
		/// </summary>
		/// <param name="userId">用户的Id</param>
		/// <param name="startDate">办理时间（从）</param>
		/// <param name="endDate">办理时间（到）</param>
		/// <returns>返回任务列表生成的HTML字符串</returns>
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
		/// 获取审批结束的立项列表
		/// </summary>
		/// <param name="userId">用户的Id</param>
		/// <param name="startDate">办理时间（从）</param>
		/// <param name="endDate">办理时间（到）</param>
		/// <returns>返回任务列表生成的HTML字符串</returns>
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


		#region 代理相关的方法
		/// <summary>
		///删除已经过期的代理信息
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
		/// 获取参数指定人员被代理的记录
		/// </summary>
		/// <param name="userId">被代理的人员,如A授权给B时,为B</param>
		/// <param name="showExpired">if set to <c>true</c> [show expired].</param>
		/// <returns></returns>
		public static List<ApprovalAgent> GetAgentsToUser(string toUser, bool showExpired)
		{
			List<ApprovalAgent> rtn = WorkflowRuntime.Current.GetService<IApprovalSaveService>().GetAgentInfoByToUser(toUser);
			if (!showExpired) RemoveExpiredAgent(rtn);
			return rtn;
		}

		/// <summary>
		/// 获取参数指定人员委派给他人的记录
		/// </summary>
		/// <param name="fromUser">代理来自于和人,A授权被B代办,那么为A</param>
		/// <param name="showExpired">是否显示已经过期的授权</param>
		/// <returns></returns>
		public static List<ApprovalAgent> GetAgentsFromUser(string fromUser, bool showExpired)
		{
			List<ApprovalAgent> agentList = WorkflowRuntime.Current.GetService<IApprovalSaveService>().GetAgentInfoByFromUser(fromUser);
			if (!showExpired) RemoveExpiredAgent(agentList);
			return agentList;
		}

		/// <summary>
		/// 删除某授权信息:1)如果该项代理尚已经开始生效,但是未到实效日期,那么将该项代理的失效时间设置为当前日期.
		/// 2)如果如果该项代理尚已经开始生效,且以过了实效日期,那么什么也不做.
		/// 3)如果该代理尚未生效,那么直接删除.
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

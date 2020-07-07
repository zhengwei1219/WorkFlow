using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;
using System.Web.Caching;
using System.Xml.Serialization;

namespace OilDigital.Workflows
{
	/// <summary>
	/// ����XML�Ĺ���������־û�����
	/// </summary>
	public class XmlWorkFlowDefinePersistService : IWorkFlowDefinePersistService
	{
		/// <summary>
		/// xml�ļ�����Ŀ¼
		/// </summary>
		private string xmlFilesPath;

		private const string cacheNamePre="wf_";

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlWorkFlowDefinePersistService"/> class.
		/// </summary>
		public XmlWorkFlowDefinePersistService() { }
		/// <summary>
		/// ����һ����ȡxml����Ĺ���������ķ���
		/// </summary>
		/// <param name="xmlFilesPath">xml�ļ����ڵ�����Ŀ¼</param>
		public XmlWorkFlowDefinePersistService(string xmlFilesPath)
		{
			if (string.IsNullOrEmpty(xmlFilesPath)) throw new WorkflowExecuteExeception("û�ж��幤���������ļ���Ŀ¼");

			//ȷ����\��β
			this.xmlFilesPath = xmlFilesPath.EndsWith("\\")?xmlFilesPath:xmlFilesPath+"\\";
		}

		/// <summary>
		/// Sets the file path.
		/// </summary>
		/// <value>The file path.</value>
		public string FilePath
		{
			set 
			{
				string root = AppDomain.CurrentDomain.BaseDirectory;
				string path = value;
				if (path.StartsWith("~"))
					path = path.Replace("/", "\\").Replace("~", root.TrimEnd("\\".ToCharArray()));
				else
					path = path.StartsWith("\\") ? root + path : root + "\\" + path;
				this.xmlFilesPath = path.EndsWith("\\") ? path : path + "\\";
			}
		}
		/// <summary>
		/// ���չ��������ƻ�ȡ����������
		/// </summary>
		/// <param name="workflowName">����������</param>
		/// <returns></returns>
		public WorkFlowDefine GetWorkflowDefine(string workflowName)
		{
			if (HttpContext.Current == null) //��httpӦ��,�޷��ӻ����л�ȡ,ֻ�ܶ�ȡ.
			{
				//List<WorkflowDefineConfig> defines = new List<WorkflowDefineConfig>();
				WorkflowConfig config = GetConfig();
				foreach (WorkflowApplication app in config.Applications)
				{
					for (int i = 0; i < app.Defines.Count; i++)
					{
						if (app.Defines[i].Name.Equals(workflowName, StringComparison.OrdinalIgnoreCase))
						{
							string fullFileName = GetFileName(app.Defines[i].File);
							return ReadFromFile(app, fullFileName);
						}
					}
				}
				throw new ArgumentException(string.Format("workflowName '{0}' must in config", workflowName));
			}


			//�л������,��ӻ�������л�ȡ
			//��ȡ��������
			string cacheName = GetCacheName(workflowName);
			//�ӻ����ж�ȡ
			if (HttpContext.Current.Cache[cacheName] == null)
			{
				WorkflowConfig config = GetConfig();
				bool found = false;
				foreach (WorkflowApplication app in config.Applications)
				{
					for (int i = 0; i < app.Defines.Count; i++)
					{
						if (app.Defines[i].Name.Equals(workflowName, StringComparison.OrdinalIgnoreCase))
						{
							string fullFileName = GetFileName(app.Defines[i].File);
							CacheDependency fileDependency = new System.Web.Caching.CacheDependency(fullFileName);
							HttpContext.Current.Cache.Add(cacheName, ReadFromFile(app,fullFileName), fileDependency, DateTime.MaxValue, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
							found = true;
							break;
						}
					}
				}
				if(!found)
					throw new ArgumentException(string.Format("workflowName '{0}' must in config", workflowName));
			}
			return (WorkFlowDefine)HttpContext.Current.Cache[cacheName];
		}

		private string GetCacheName(string name)
		{
			return string.Format("{0}{1}", cacheNamePre, name);
		}

		/// <summary>
		/// ��ȡ������·��
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		private string GetFileName(string fileName)
		{
			return string.Format("{0}{1}", xmlFilesPath, fileName);
		}

		/// <summary>
		///�����ļ�����ȡ�������������
		/// </summary>
		/// <param name="app">The app.</param>
		/// <param name="fileName">Name of the file.</param>
		/// <returns></returns>
		private WorkFlowDefine ReadFromFile(WorkflowApplication app,string fileName)
		{
			StateMachineWorkflow stateMachine = new StateMachineWorkflow();
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(StateMachineWorkflow));
			using (XmlReader reader = XmlReader.Create(fileName))
			{
				stateMachine = (StateMachineWorkflow)xmlSerializer.Deserialize(reader);
			}
			string errorMsg;
			if (!stateMachine.Validate(out errorMsg))
				throw new ApplicationException(errorMsg);

			//������Ӧ�ó������
			stateMachine.Application = app;
			return stateMachine;
		}

		#region IWorkFlowDefinePersistService Members


		/// <summary>
		/// Gets the root.
		/// </summary>
		/// <param name="applicationName">Name of the application.</param>
		/// <returns></returns>
		public string GetRoot(string applicationName)
		{
			WorkflowConfig config = GetConfig();
			if (config == null)
				throw new ArgumentException("workflow config xml file is miss or wrong format");
			List<string> names = new List<string>();
			WorkflowApplication application = config.Get(applicationName);
			return application.Root;
		}
		/// <summary>
		/// �������п��õĹ�������������
		/// </summary>
		/// <returns></returns>
		public string[] GetAllWorkflowDefineName(string applicationName)
		{
			WorkflowConfig config = GetConfig();
			if (config == null)
				throw new ArgumentException("workflow config xml file is miss or wrong format");
			List<string> names = new List<string>();
			WorkflowApplication application = config.Get(applicationName);
			if (application == null)
				throw new NullReferenceException(string.Format("{0} cannot found", applicationName));
			foreach (WorkflowDefineConfig one in application.Defines)
			{
				if (one.Enabled) names.Add(one.Name);
			}
			return names.ToArray();
		}

		/// <summary>
		/// ��ȡ����������
		/// </summary>
		/// <returns></returns>
		public WorkflowConfig GetConfig()
		{
			string cacheName="workflowconfig";
			if (HttpContext.Current == null)
			{
				string fileName = GetFileName("workflowconfig.xml");
				WorkflowConfig config = ReadConfig(fileName);
				return config;
			}
			if (HttpContext.Current.Cache[cacheName] == null)
			{
				string fileName = GetFileName("workflowconfig.xml");
				WorkflowConfig config = ReadConfig(fileName);
				CacheDependency fileDependency = new System.Web.Caching.CacheDependency(fileName);
				HttpContext.Current.Cache.Add(cacheName, config, fileDependency, DateTime.MaxValue, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
			}
			return (WorkflowConfig)HttpContext.Current.Cache[cacheName];
		}

		private WorkflowConfig ReadConfig(string fileName)
		{
			WorkflowConfig config = null;
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(WorkflowConfig));
			using (XmlReader reader = XmlReader.Create(fileName))
			{
				config = (WorkflowConfig)xmlSerializer.Deserialize(reader);
			}
			return config;

		}

		/// <summary>
		/// Gets all workflow.
		/// </summary>
		/// <returns></returns>
		public WorkFlowDefine[] GetAllWorkflow(string applicationName)
		{
			WorkflowConfig config = GetConfig();
			if (config == null)
				throw new ArgumentException("workflow config xml file is miss or wrong format");
			List<WorkFlowDefine> workflows = new List<WorkFlowDefine>();
			WorkflowApplication application = config.Get(applicationName);
			if (application == null)
				throw new NullReferenceException(string.Format("{0} cannot found", applicationName));
			foreach (WorkflowDefineConfig one in application.Defines)
			{
				if (one.Enabled)
					workflows.Add(GetWorkflowDefine(one.Name));
			}
			return workflows.ToArray();
		}

		/// <summary>
		/// Gets all workflow.
		/// </summary>
		/// <returns></returns>
		public string[] GetAllWorkflowDefineName()
		{
			WorkflowConfig config = GetConfig();
			if (config == null)
				throw new ArgumentException("workflow config xml file is miss or wrong format");
			List<string> workflows = new List<string>();
			List<WorkflowDefineConfig> configs = new List<WorkflowDefineConfig>();
			foreach (WorkflowApplication app in config.Applications)
			{
				configs.AddRange(app.Defines);
			}
			foreach (WorkflowDefineConfig one in configs)
			{
				if (one.Enabled)
					workflows.Add(one.Name);
			}
			return workflows.ToArray();
		}

		/// <summary>
		/// Saves the specified XML.
		/// </summary>
		/// <param name="xml">The XML.</param>
		/// <param name="applicationName">Name of the application.</param>
		public void Save(string xml, string applicationName)
		{

		}

		/// <summary>
		/// Saves the specified workflow.
		/// </summary>
		/// <param name="workflow">The workflow.</param>
		/// <param name="applicationName">Name of the application.</param>
		public void Save(WorkFlowDefine workflow, string applicationName)
		{

		}

		/// <summary>
		/// Deletes the specified id.
		/// </summary>
		/// <param name="id">The id.</param>
		public void Delete(Guid id)
		{
			
		}

		/// <summary>
		/// Gets the specified id.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		public WorkFlowDefine Get(Guid id)
		{
			return null;
		}

		#endregion
	}
}

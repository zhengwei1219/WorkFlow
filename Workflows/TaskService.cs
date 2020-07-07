using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Configuration;
using System.Text;

namespace OilDigital.Workflows
{
	/// <summary>
	/// ���������,�����ȡָ���û������̡���ɫ��ʵ������������ֱ�ӳ���ΪHtml��
	/// </summary>
	public class TaskService : ICloneable
	{
		private TaskService()
		{
			this.identityService = WorkflowRuntime.Current.GetService<IIdentityService>();
			this.userInRole = identityService.GetUserInRole();
		}
		/// <summary>
		/// ��RoleConfg��Ŀ�����ɷ���
		/// </summary>
		/// <param name="config">The config.</param>
		/// <param name="applicationName">��TaskServiceʵ����Ӧ��Ӧ������.</param>
		internal TaskService(ServiceConfig config, string applicationName)
			: this()
		{
			//��ȡ��Ӧ�õ����й���������.
			IWorkFlowDefinePersistService defineService = WorkflowRuntime.Current.GetService<IWorkFlowDefinePersistService>();
			this.workflowNames = defineService.GetAllWorkflowDefineName(applicationName);

			this.taskName = config.TaskName;
			this.name = config.Name;
			this.mergeRoles = config.AllMerge;
			if (config.DefaultItem == null)
				throw new ConfigurationErrorsException(string.Format("Cann't found Default roleConfig!"));
			this.defaultDistiller = InitTaskDistiller(config.DefaultItem);
			if (defaultDistiller.InstanceDistillers == null)
				throw new ConfigurationErrorsException(string.Format("default distillers \"{0}\" can not be empty!", config.Name));
			if (defaultDistiller.Filters == null)
				throw new ConfigurationErrorsException(string.Format("default filters \"{0}\" can not be empty!", config.Name));
			if (defaultDistiller.ItemDistiller == null)
				throw new ConfigurationErrorsException(string.Format("default itemDistiller \"{0}\" is not exist!", config.Name));
			if (defaultDistiller.Render == null)
				throw new ConfigurationErrorsException(string.Format("default taskRender \"{0}\" is not exist!", config.Name));
			distillersMap = new Dictionary<string, TaskDistiller>();
			if (config.Items != null && config.Items.Length > 0)
			{
				//����ÿ����ɫ������Ŀ������ȡ�ߣ������ȡ��
				foreach (RoleConfigItem item in config.Items)
				{
					TaskDistiller distiller = InitTaskDistiller(item);
					//�����ȡ�߼���Ϊ�գ���ʹ��ȱʡ��ȡ�߼���
					if (distiller.InstanceDistillers == null || distiller.InstanceDistillers.Length == 0)
						distiller.InstanceDistillers = defaultDistiller.InstanceDistillers;
					//���������Ϊ�գ���ʹ��ȱʡ����������
					if (distiller.Filters == null || distiller.Filters.Length == 0)
						distiller.Filters = defaultDistiller.Filters;
					if (distiller.ItemDistiller == null)
						distiller.ItemDistiller = defaultDistiller.ItemDistiller;
					if (distiller.Render == null)
						distiller.Render = defaultDistiller.Render;
					if (string.IsNullOrEmpty(item.Name))
						throw new ConfigurationErrorsException("");
					//�ѽ�ɫ�ִ����Ϊ��ɫ����
					string[] o = item.Name.Split(";,����".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
					//��ӵ���ɫ-��ȡ���ֵ���
					foreach (string s in o)
					{
						if (!distillersMap.ContainsKey(s))
							distillersMap[s] = distiller;
					}
				}
			}
			//У���ɫ�ֵ䣬�Բ����ڽ�ɫ�׳��쳣
			List<string> roles = new List<string>(WorkflowUtility.GetAllRoles(applicationName));
			foreach (string key in distillersMap.Keys)
			{
				if (!roles.Contains(key))
					throw new ConfigurationErrorsException(string.Format("There isn't a role called '{0}' in workflow's definiton files", key));
			}
		}
		private TaskDistiller InitTaskDistiller(RoleConfigItem config)
		{
			TaskDistiller taskDistiller = new TaskDistiller();
			List<InstanceDistiller> distillers = new List<InstanceDistiller>();
			if (config.Distillers != null && config.Distillers.Length != 0)
			{
				//����ÿ��ʵ����ȡ�߲�������ȡ�߼���
				foreach (TypeItem item in config.Distillers)
				{
					InstanceDistiller distiller = (InstanceDistiller)ConfigurationHelper.LoadType(item);
					if (distiller == null)
						throw new ConfigurationErrorsException(string.Format("{0} isn't the type IInstanceDistiller", item.Type));
					distillers.Add(distiller);
				}
			}
			taskDistiller.InstanceDistillers = distillers.ToArray();
			//����ÿ�����������������������
			List<InstanceFilter> filters = new List<InstanceFilter>();
			if (config.Filters != null && config.Filters.Length != 0)
			{
				foreach (TypeItem item in config.Filters)
				{
					InstanceFilter filler = (InstanceFilter)ConfigurationHelper.LoadType(item);
					if (filler == null)
						throw new ConfigurationErrorsException(string.Format("{0} isn't the type IInstanceFilter", item.Type));
					filters.Add(filler);
				}
			}
			taskDistiller.Filters = filters.ToArray();
			//ʵ����������Ŀ��ȡ��
			if (config.TaskItemDistiller != null)
			{
				object obj = ConfigurationHelper.LoadType(config.TaskItemDistiller);
				if (obj.GetType().GetInterface(typeof(ITaskItemDistiller).Name) == null)
					throw new TypeLoadException(string.Format("Type \"{0}\" does not implement interface ITaskItemDistiller", obj.GetType().FullName));
				taskDistiller.ItemDistiller = (ITaskItemDistiller)obj;
			}
			//ʵ����������
			if (config.TaskRender != null)
			{
				object obj = ConfigurationHelper.LoadType(config.TaskRender);
				if (obj.GetType().GetInterface(typeof(ITaskRender).Name) == null)
					throw new TypeLoadException(string.Format("Type \"{0}\" does not implement interface ITaskRender", obj.GetType().FullName));
				taskDistiller.Render = (ITaskRender)obj;
			}
			return taskDistiller;
		}
		/// <summary>
		/// ��ȡĳ�û�������
		/// </summary>
		/// <param name="userId">�û�Id</param>
		/// <returns>�����б�</returns>
		public string Distill(string userId)
		{
			return Distill(userId, DateTime.MinValue, DateTime.MaxValue);
		}
		/// <summary>
		/// ��ȡĳ�û���ĳ���̵������б�
		/// </summary>
		/// <param name="userId">�û�Id</param>
		/// <returns></returns>
		public InstanceCollection DistillInstances(string userId)
		{
			return DistillInstances(userId, DateTime.MinValue, DateTime.MaxValue);
		}
		/// <summary>
		/// ��ȡĳ�û��Ľ�ɫ-�����б����ֵ�
		/// </summary>
		/// <param name="userId">�û�Id</param>
		/// <returns>�����û�ÿ����ɫ��Ӧ�������б���.</returns>
		public Dictionary<string, TaskListCollection> DistillTasklists(string userId)
		{
			return ConvertToTasklists(userId, DistillInstances(userId));
		}
		/// <summary>
		/// ��ȡĳ�û�������
		/// </summary>
		/// <param name="userId">�û�Id</param>
		/// <param name="startDate">��ʼʱ��</param>
		/// <param name="endDate">��ֹʱ��</param>
		/// <returns></returns>
		public string Distill(string userId, DateTime startDate, DateTime endDate)
		{
			return Render(ConvertToTasklists(userId, DistillInstances(userId, startDate, endDate)));
		}
		/// <summary>
		/// ��ȡĳ�û���ĳ���̵������б�
		/// </summary>
		/// <param name="userId">�û�Id</param>
		/// <param name="startDate">��ʵʱ��</param>
		/// <param name="endDate">��ֹʱ��</param>
		/// <returns></returns>
		public InstanceCollection DistillInstances(string userId, DateTime startDate, DateTime endDate)
		{
			InstanceCollection instances = new InstanceCollection();
			foreach (string workflowName in this.workflowNames)
			{
				//��ÿһ��������ÿһ����ɫ�����õ�ʵ�����Ϻ�ϲ����ɫ-ʵ�������ֵ�
				List<ApprovalRole> roles = WorkflowUtility.GetUserRoles(workflowName, userId);
				foreach (ApprovalRole role in roles)
				{
					instances.AddRange(DistillInstances(workflowName, userId, role.Name, startDate, endDate));
				}
			}
			return instances;
		}
		/// <summary>
		/// ��ȡĳ�û��Ľ�ɫ-�����б����ֵ�
		/// </summary>
		/// <param name="userId">�û�Id</param>
		/// <param name="startDate">��ʼʱ��</param>
		/// <param name="endDate">��ֹʱ��</param>
		/// <returns></returns>
		public Dictionary<string, TaskListCollection> DistillTasklists(string userId, DateTime startDate, DateTime endDate)
		{
			return ConvertToTasklists(userId, DistillInstances(userId, startDate, endDate));
		}
		/// <summary>
		/// ��ȡĳ�û���ĳ���̵������б�
		/// </summary>
		/// <param name="workflowName">��������</param>
		/// <param name="userId">�û�Id</param>
		/// <returns></returns>
		public string Distill(string workflowName, string userId)
		{
			return Distill(workflowName, userId, DateTime.MinValue, DateTime.MaxValue);
		}
		/// <summary>
		/// ��ȡĳ�û���ĳ���̵Ľ�ɫ-ʵ�������ֵ�
		/// </summary>
		/// <param name="workflowName">��������</param>
		/// <param name="userId">�û�Id</param>
		/// <returns></returns>
		private InstanceCollection DistillInstances(string workflowName, string userId)
		{
			return DistillInstances(workflowName, userId, DateTime.MinValue, DateTime.MaxValue);
		}

		/// <summary>
		/// ��ȡĳ�û��Ľ�ɫ-�����б����ֵ�
		/// </summary>
		/// <param name="workflowName">����������</param>
		/// <param name="userId">�û�Id</param>
		/// <returns></returns>
		private Dictionary<string, TaskListCollection> DistillTaskLists(string workflowName, string userId)
		{
			return ConvertToTasklists(userId, DistillInstances(workflowName, userId));
		}
		/// <summary>
		/// ��ȡĳ�û���ĳ���̵������б�
		/// </summary>
		/// <param name="workflowName">��������</param>
		/// <param name="userId">�û�Id</param>
		/// <param name="startDate">��ʵʱ��</param>
		/// <param name="endDate">��ֹʱ��</param>
		/// <returns></returns>
		private string Distill(string workflowName, string userId, DateTime startDate, DateTime endDate)
		{
			return Render(ConvertToTasklists(userId, DistillInstances(workflowName, userId, startDate, endDate)));
		}

		/// <summary>
		/// ��ȡĳ�û���ĳ���̵Ľ�ɫ-ʵ�������ֵ�
		/// </summary>
		/// <param name="workflowName">��������</param>
		/// <param name="userId">�û�Id</param>
		/// <param name="startDate">��ʵʱ��</param>
		/// <param name="endDate">��ֹʱ��</param>
		/// <returns></returns>
		private  InstanceCollection DistillInstances(string workflowName, string userId, DateTime startDate, DateTime endDate)
		{
			List<ApprovalRole> roles = WorkflowUtility.GetUserRoles(workflowName, userId);
			IUserIdentity userIdentity = identityService.GetUserIdentity(userId);
			InstanceCollection rtn = new InstanceCollection();
			//�Ա�������ÿһ����ɫ�����õ�ʵ�����Ϻ�ϲ����ɫ-ʵ�������ֵ�
			foreach (ApprovalRole role in roles)
			{
				rtn.AddRange(DistillInstances(workflowName, userId, role.Name, startDate, endDate));
			}
			return rtn;
		}

		/// <summary>
		/// ��ȡĳ�û��Ľ�ɫ-�����б����ֵ�
		/// </summary>
		/// <param name="workflowName">��������</param>
		/// <param name="userId">�û�Id</param>
		/// <param name="startDate">��ʼʱ��</param>
		/// <param name="endDate">��ֹʱ��</param>
		/// <returns></returns>
		public Dictionary<string, TaskListCollection> DistillTasklists(string workflowName, string userId, DateTime startDate, DateTime endDate)
		{
			return ConvertToTasklists(userId, DistillInstances(workflowName, userId, startDate, endDate));
		}
		/// <summary>
		/// ��ȡĳ�û���ĳ���̵������б�
		/// </summary>
		/// <param name="workflowName">��������</param>
		/// <param name="userId">�û�Id</param>
		/// <param name="roleName">�û��Ľ�ɫ����</param>
		/// <returns></returns>
		public string Distill(string workflowName, string userId, string roleName)
		{
			return Distill(workflowName, userId, roleName, DateTime.MinValue, DateTime.MaxValue);
		}
		/// <summary>
		/// ��ȡ��ɫ�Ͷ�Ӧʵ�����ϵļ�-ֵ��
		/// </summary>
		/// <param name="workflowName">��������</param>
		/// <param name="userId">�û�Id</param>
		/// <param name="roleName">��ɫ����</param>
		/// <returns></returns>
		public InstanceCollection DistillInstances(string workflowName, string userId, string roleName)
		{
			return DistillInstances(workflowName, userId, roleName, DateTime.MinValue, DateTime.MaxValue);
		}
		/// <summary>
		/// ��ȡĳ�û��Ľ�ɫ-�����б����ֵ�
		/// </summary>
		/// <param name="workflowName">��������</param>
		/// <param name="userId">�û�Id</param>
		/// <param name="roleName">��ɫ����</param>
		/// <returns></returns>
		public Dictionary<string, TaskListCollection> DistillTasklists(string workflowName, string userId, string roleName)
		{
			return ConvertToTasklists(userId, DistillInstances(workflowName, userId, roleName));
		}
		/// <summary>
		/// ��ȡĳ�û���ĳ���̵������б�
		/// </summary>
		/// <param name="workflowName">��������</param>
		/// <param name="userId">�û�Id</param>
		/// <param name="roleName">��ɫ����</param>
		/// <param name="startDate">��ʼʱ��</param>
		/// <param name="endDate">��ֹʱ��</param>
		/// <returns></returns>
		public string Distill(string workflowName, string userId, string roleName, DateTime startDate, DateTime endDate)
		{
			InstanceCollection instances = DistillInstances(workflowName, userId, roleName, startDate, endDate);
			return Render(ConvertToTasklists(userId, instances));
		}
		/// <summary>
		/// ��ȡ��ɫ�Ͷ�Ӧʵ�����ϵļ�-ֵ��
		/// </summary>
		/// <param name="workflowName">��������</param>
		/// <param name="userId">�û�Id</param>
		/// <param name="roleName">��ɫ����</param>
		/// <param name="startDate">ʱ�����ʼʱ��</param>
		/// <param name="endDate">ʱ��ν�ֹʱ��</param>
		/// <returns></returns>
		private InstanceCollection DistillInstances(string workflowName, string userId, string roleName, DateTime startDate, DateTime endDate)
		{
			IUserIdentity userIdentity = identityService.GetUserIdentity(userId);
			//��ȡ���̶�Ӧ��������ɫ���󣬲��ӽ�ɫ-��ȡ���ֵ��л�ȡ��ɫ��Ӧ����ȡ��
			InstanceCollection instances = new InstanceCollection();
			ApprovalRole role = WorkflowUtility.GetUserRoleByName(workflowName, roleName);
			if (role != null)
			{
				//�û����ڸý�ɫ
				if (!userInRole.IsUserInRole(userIdentity.GetUserId(), role.Name))
					throw new ApplicationException(string.Format("{0} is not in role {1}", userIdentity.GetUserId(), role.Name));
				//ȡ�ý�ɫ��������ȡ��
				TaskDistiller distiller = GetRoleDistiller(roleName);
				instances.AddRange(distiller.Distill(workflowName, userIdentity, role, startDate, endDate));
			}
			return instances;
		}

		/// <summary>
		/// ��ȡĳ�û��Ľ�ɫ-�����б����ֵ�
		/// </summary>
		/// <param name="workflowName">��������</param>
		/// <param name="userId">�û�Id</param>
		/// <param name="roleName">��ɫ����</param>
		/// <param name="startDate">��ʼʱ��</param>
		/// <param name="endDate">��ֹʱ��</param>
		/// <returns></returns>
		public Dictionary<string, TaskListCollection> DistillTasklists(string workflowName, string userId, string roleName, DateTime startDate, DateTime endDate)
		{
			return ConvertToTasklists(userId, DistillInstances(workflowName, userId, roleName, startDate, endDate));
		}

		/// <summary>
		/// ����ɫ-ʵ������ת��Ϊ�û��������б�
		/// </summary>
		/// <param name="userId">�û�Id.</param>
		/// <param name="instances">��ɫ-ʵ������.</param>
		/// <returns></returns>
		private Dictionary<string, TaskListCollection> ConvertToTasklists(string userId,  InstanceCollection instances)
		{
			Dictionary<string, TaskListCollection> taskLists = new Dictionary<string, TaskListCollection>();
			IUserIdentity userIdentity = identityService.GetUserIdentity(userId);
			List<ApprovalRole> allRoles = instances.GetAllRoles();

			//����ɫ�ֱ���ֽ�ɫ��Ӧ�������б�
			foreach (ApprovalRole role in allRoles)
			{
				InstanceCollection c = instances.GetByRole(role);

				TaskListCollection lists = new TaskListCollection();
				//��ȡ�ý�ɫ��Ӧ����ȡ������
				TaskDistiller distiller = GetRoleDistiller(role.Name);
				distiller.ForRole = role;
				if (distiller.ItemDistiller is IInitializableDistiller)
				{
					((IInitializableDistiller)(distiller.ItemDistiller)).Initialize(role, c);
				}

				List<InstanceCollection> collection = c.SplitByEaId();
				foreach (InstanceCollection one in collection)
				{
					List<ITaskItem> items = new List<ITaskItem>();
					
					ITaskItem taskItem = distiller.ItemDistiller.Distill(one, userIdentity);
					items.Add(taskItem);
					TaskList list = new TaskList(one.TaskName, items);
					//�������б����Ƹ�ֵ
					if (!string.IsNullOrEmpty(this.taskName))
						list.Cagegory = this.taskName;
					lists.Merge(list);
				}
				taskLists.Add(role.Name, lists);
			}
			return taskLists;
		}
		/// <summary>
		/// ���ƶ��Ľ�ɫ-�����б����ΪHtml
		/// </summary>
		/// <param name="taskLists">��ɫ-�����б���</param>
		/// <returns></returns>
		private string Render(Dictionary<string, TaskListCollection> taskLists)
		{
			return Render(taskLists, mergeRoles);
		}

		/// <summary>
		/// �������б����ΪHTML.
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <param name="instances">The instances.</param>
		/// <param name="mergeRoles">if set to <c>true</c> [merge roles].</param>
		/// <returns></returns>
		public string Render(string userId,InstanceCollection instances, bool mergeRoles)
		{
			return Render(ConvertToTasklists(userId, instances), mergeRoles);
		}
		/// <summary>
		/// Renders the specified task lists.
		/// </summary>
		/// <param name="taskLists">The task lists.</param>
		/// <param name="mergeRoles">if set to <c>true</c> [merge roles].</param>
		/// <returns></returns>
		private string Render(Dictionary<string, TaskListCollection> taskLists, bool mergeRoles)
		{
			Dictionary<TaskDistiller, TaskListCollection> distillers = new Dictionary<TaskDistiller, TaskListCollection>();
			//����ɫ�ֱ���ֽ�ɫ��Ӧ�������б�
			StringBuilder sb = new StringBuilder();
			if (mergeRoles)
			{
				TaskDistiller distiller = null;
				TaskListCollection list = new TaskListCollection();
				foreach (string roleName in taskLists.Keys)
				{
					distiller = GetRoleDistiller(roleName);
					list.AddRange(taskLists[roleName]);
				}
				sb.Append(distiller.Render.TaskListsToHtml(list.ToArray()));
			}
			else
			{
				foreach (string roleName in taskLists.Keys)
				{
					TaskDistiller distiller = GetRoleDistiller(roleName);
					//�ϲ�����
					sb.Append(distiller.Render.TaskListsToHtml(taskLists[roleName].ToArray()));
				}
			}
			return sb.ToString();
		}


		private IIdentityService identityService;
		private IUserInRole userInRole;
		/// <summary>
		/// �趨�������б������.���γ�һ�����������б�ʱ,�Ƿ��ƶ������б������,���Ϊnull,��ô��ʹ�������б��Լ�������.
		/// һ���ںϹ����н�ɫʱ����ʹ��.
		/// </summary>
		private string taskName;
		/// <summary>
		/// �Ƿ�ϲ���ʾ���������б�.trueʱ��ֻ��ʾһ�����������б�,falseʱ,�����û�������ɫ��ʾ�����б�.ȱʡΪfalse.
		/// </summary>
		private bool mergeRoles = false;
		private string name;

		/// <summary>
		///  �÷����Ӧ��Ӧ�����еĹ���������.
		/// </summary>
		private string[] workflowNames;

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		private Dictionary<string, TaskDistiller> distillersMap;
		private TaskDistiller defaultDistiller;
		/// <summary>
		/// ȱʡ����ȡ��
		/// </summary>
		public TaskDistiller DefalutDistiller
		{
			get { return (TaskDistiller)defaultDistiller.Clone(); }
		}
		/// <summary>
		/// ��ȡĳ��ɫ��������ȡ��
		/// </summary>
		/// <param name="roleName">��ɫ����</param>
		/// <returns></returns>
		public TaskDistiller GetRoleDistiller(string roleName)
		{
			if (distillersMap.ContainsKey(roleName))
				return (TaskDistiller)distillersMap[roleName].Clone();
			return (TaskDistiller)defaultDistiller.Clone();
		}
		#region ICloneable Members

		/// <summary>
		/// ��¡���񣬱�������ȫ�ֶ���
		/// </summary>
		public object Clone()
		{
			TaskService o = this.MemberwiseClone() as TaskService;
			o.identityService = WorkflowRuntime.Current.GetService<IIdentityService>();
			o.userInRole = o.identityService.GetUserInRole();
			o.defaultDistiller = this.defaultDistiller.Clone() as TaskDistiller;
			o.distillersMap = new Dictionary<string, TaskDistiller>();

			foreach (string key in this.distillersMap.Keys)
			{
				o.distillersMap.Add(key, (TaskDistiller)this.distillersMap[key].Clone());
			}
			return o;
		}

		#endregion
	}
}

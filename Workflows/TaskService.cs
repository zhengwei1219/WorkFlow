using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Configuration;
using System.Text;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 任务服务类,负责获取指定用户、流程、角色的实例或任务类表或直接呈现为Html串
	/// </summary>
	public class TaskService : ICloneable
	{
		private TaskService()
		{
			this.identityService = WorkflowRuntime.Current.GetService<IIdentityService>();
			this.userInRole = identityService.GetUserInRole();
		}
		/// <summary>
		/// 从RoleConfg项目中生成服务
		/// </summary>
		/// <param name="config">The config.</param>
		/// <param name="applicationName">此TaskService实例对应的应用名称.</param>
		internal TaskService(ServiceConfig config, string applicationName)
			: this()
		{
			//获取该应用的所有工作流名称.
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
				//遍历每个角色配置项目生成提取者，如果提取者
				foreach (RoleConfigItem item in config.Items)
				{
					TaskDistiller distiller = InitTaskDistiller(item);
					//如果提取者集合为空，则使用缺省提取者集合
					if (distiller.InstanceDistillers == null || distiller.InstanceDistillers.Length == 0)
						distiller.InstanceDistillers = defaultDistiller.InstanceDistillers;
					//如果过滤器为空，则使用缺省过滤器集合
					if (distiller.Filters == null || distiller.Filters.Length == 0)
						distiller.Filters = defaultDistiller.Filters;
					if (distiller.ItemDistiller == null)
						distiller.ItemDistiller = defaultDistiller.ItemDistiller;
					if (distiller.Render == null)
						distiller.Render = defaultDistiller.Render;
					if (string.IsNullOrEmpty(item.Name))
						throw new ConfigurationErrorsException("");
					//把角色字串拆解为角色数组
					string[] o = item.Name.Split(";,；，".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
					//添加到角色-提取器字典中
					foreach (string s in o)
					{
						if (!distillersMap.ContainsKey(s))
							distillersMap[s] = distiller;
					}
				}
			}
			//校验角色字典，对不存在角色抛出异常
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
				//遍历每个实例提取者并加入提取者集合
				foreach (TypeItem item in config.Distillers)
				{
					InstanceDistiller distiller = (InstanceDistiller)ConfigurationHelper.LoadType(item);
					if (distiller == null)
						throw new ConfigurationErrorsException(string.Format("{0} isn't the type IInstanceDistiller", item.Type));
					distillers.Add(distiller);
				}
			}
			taskDistiller.InstanceDistillers = distillers.ToArray();
			//遍历每个过滤器并加入过滤器集合
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
			//实例化任务项目提取器
			if (config.TaskItemDistiller != null)
			{
				object obj = ConfigurationHelper.LoadType(config.TaskItemDistiller);
				if (obj.GetType().GetInterface(typeof(ITaskItemDistiller).Name) == null)
					throw new TypeLoadException(string.Format("Type \"{0}\" does not implement interface ITaskItemDistiller", obj.GetType().FullName));
				taskDistiller.ItemDistiller = (ITaskItemDistiller)obj;
			}
			//实例化呈现器
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
		/// 获取某用户的任务
		/// </summary>
		/// <param name="userId">用户Id</param>
		/// <returns>任务列表</returns>
		public string Distill(string userId)
		{
			return Distill(userId, DateTime.MinValue, DateTime.MaxValue);
		}
		/// <summary>
		/// 获取某用户对某流程的任务列表
		/// </summary>
		/// <param name="userId">用户Id</param>
		/// <returns></returns>
		public InstanceCollection DistillInstances(string userId)
		{
			return DistillInstances(userId, DateTime.MinValue, DateTime.MaxValue);
		}
		/// <summary>
		/// 获取某用户的角色-任务列表结合字典
		/// </summary>
		/// <param name="userId">用户Id</param>
		/// <returns>返回用户每个角色对应的任务列表集合.</returns>
		public Dictionary<string, TaskListCollection> DistillTasklists(string userId)
		{
			return ConvertToTasklists(userId, DistillInstances(userId));
		}
		/// <summary>
		/// 获取某用户的任务
		/// </summary>
		/// <param name="userId">用户Id</param>
		/// <param name="startDate">起始时间</param>
		/// <param name="endDate">终止时间</param>
		/// <returns></returns>
		public string Distill(string userId, DateTime startDate, DateTime endDate)
		{
			return Render(ConvertToTasklists(userId, DistillInstances(userId, startDate, endDate)));
		}
		/// <summary>
		/// 获取某用户对某流程的任务列表
		/// </summary>
		/// <param name="userId">用户Id</param>
		/// <param name="startDate">其实时间</param>
		/// <param name="endDate">终止时间</param>
		/// <returns></returns>
		public InstanceCollection DistillInstances(string userId, DateTime startDate, DateTime endDate)
		{
			InstanceCollection instances = new InstanceCollection();
			foreach (string workflowName in this.workflowNames)
			{
				//对每一个流程中每一个角色遍历得到实例集合后合并入角色-实例集合字典
				List<ApprovalRole> roles = WorkflowUtility.GetUserRoles(workflowName, userId);
				foreach (ApprovalRole role in roles)
				{
					instances.AddRange(DistillInstances(workflowName, userId, role.Name, startDate, endDate));
				}
			}
			return instances;
		}
		/// <summary>
		/// 获取某用户的角色-任务列表结合字典
		/// </summary>
		/// <param name="userId">用户Id</param>
		/// <param name="startDate">起始时间</param>
		/// <param name="endDate">终止时间</param>
		/// <returns></returns>
		public Dictionary<string, TaskListCollection> DistillTasklists(string userId, DateTime startDate, DateTime endDate)
		{
			return ConvertToTasklists(userId, DistillInstances(userId, startDate, endDate));
		}
		/// <summary>
		/// 获取某用户对某流程的任务列表
		/// </summary>
		/// <param name="workflowName">流程名称</param>
		/// <param name="userId">用户Id</param>
		/// <returns></returns>
		public string Distill(string workflowName, string userId)
		{
			return Distill(workflowName, userId, DateTime.MinValue, DateTime.MaxValue);
		}
		/// <summary>
		/// 获取某用户对某流程的角色-实例集合字典
		/// </summary>
		/// <param name="workflowName">流程名称</param>
		/// <param name="userId">用户Id</param>
		/// <returns></returns>
		private InstanceCollection DistillInstances(string workflowName, string userId)
		{
			return DistillInstances(workflowName, userId, DateTime.MinValue, DateTime.MaxValue);
		}

		/// <summary>
		/// 获取某用户的角色-任务列表结合字典
		/// </summary>
		/// <param name="workflowName">工作流名称</param>
		/// <param name="userId">用户Id</param>
		/// <returns></returns>
		private Dictionary<string, TaskListCollection> DistillTaskLists(string workflowName, string userId)
		{
			return ConvertToTasklists(userId, DistillInstances(workflowName, userId));
		}
		/// <summary>
		/// 获取某用户对某流程的任务列表
		/// </summary>
		/// <param name="workflowName">流程名称</param>
		/// <param name="userId">用户Id</param>
		/// <param name="startDate">其实时间</param>
		/// <param name="endDate">终止时间</param>
		/// <returns></returns>
		private string Distill(string workflowName, string userId, DateTime startDate, DateTime endDate)
		{
			return Render(ConvertToTasklists(userId, DistillInstances(workflowName, userId, startDate, endDate)));
		}

		/// <summary>
		/// 获取某用户对某流程的角色-实例集合字典
		/// </summary>
		/// <param name="workflowName">流程名称</param>
		/// <param name="userId">用户Id</param>
		/// <param name="startDate">其实时间</param>
		/// <param name="endDate">终止时间</param>
		/// <returns></returns>
		private  InstanceCollection DistillInstances(string workflowName, string userId, DateTime startDate, DateTime endDate)
		{
			List<ApprovalRole> roles = WorkflowUtility.GetUserRoles(workflowName, userId);
			IUserIdentity userIdentity = identityService.GetUserIdentity(userId);
			InstanceCollection rtn = new InstanceCollection();
			//对本流程中每一个角色遍历得到实例集合后合并入角色-实例集合字典
			foreach (ApprovalRole role in roles)
			{
				rtn.AddRange(DistillInstances(workflowName, userId, role.Name, startDate, endDate));
			}
			return rtn;
		}

		/// <summary>
		/// 获取某用户的角色-任务列表结合字典
		/// </summary>
		/// <param name="workflowName">流程名称</param>
		/// <param name="userId">用户Id</param>
		/// <param name="startDate">起始时间</param>
		/// <param name="endDate">终止时间</param>
		/// <returns></returns>
		public Dictionary<string, TaskListCollection> DistillTasklists(string workflowName, string userId, DateTime startDate, DateTime endDate)
		{
			return ConvertToTasklists(userId, DistillInstances(workflowName, userId, startDate, endDate));
		}
		/// <summary>
		/// 获取某用户对某流程的任务列表
		/// </summary>
		/// <param name="workflowName">流程名称</param>
		/// <param name="userId">用户Id</param>
		/// <param name="roleName">用户的角色名称</param>
		/// <returns></returns>
		public string Distill(string workflowName, string userId, string roleName)
		{
			return Distill(workflowName, userId, roleName, DateTime.MinValue, DateTime.MaxValue);
		}
		/// <summary>
		/// 获取角色和对应实例集合的键-值对
		/// </summary>
		/// <param name="workflowName">流程名称</param>
		/// <param name="userId">用户Id</param>
		/// <param name="roleName">角色名称</param>
		/// <returns></returns>
		public InstanceCollection DistillInstances(string workflowName, string userId, string roleName)
		{
			return DistillInstances(workflowName, userId, roleName, DateTime.MinValue, DateTime.MaxValue);
		}
		/// <summary>
		/// 获取某用户的角色-任务列表结合字典
		/// </summary>
		/// <param name="workflowName">流程名称</param>
		/// <param name="userId">用户Id</param>
		/// <param name="roleName">角色名称</param>
		/// <returns></returns>
		public Dictionary<string, TaskListCollection> DistillTasklists(string workflowName, string userId, string roleName)
		{
			return ConvertToTasklists(userId, DistillInstances(workflowName, userId, roleName));
		}
		/// <summary>
		/// 获取某用户对某流程的任务列表
		/// </summary>
		/// <param name="workflowName">流程名称</param>
		/// <param name="userId">用户Id</param>
		/// <param name="roleName">角色名称</param>
		/// <param name="startDate">起始时间</param>
		/// <param name="endDate">终止时间</param>
		/// <returns></returns>
		public string Distill(string workflowName, string userId, string roleName, DateTime startDate, DateTime endDate)
		{
			InstanceCollection instances = DistillInstances(workflowName, userId, roleName, startDate, endDate);
			return Render(ConvertToTasklists(userId, instances));
		}
		/// <summary>
		/// 获取角色和对应实例集合的键-值对
		/// </summary>
		/// <param name="workflowName">流程名称</param>
		/// <param name="userId">用户Id</param>
		/// <param name="roleName">角色名称</param>
		/// <param name="startDate">时间段起始时间</param>
		/// <param name="endDate">时间段截止时间</param>
		/// <returns></returns>
		private InstanceCollection DistillInstances(string workflowName, string userId, string roleName, DateTime startDate, DateTime endDate)
		{
			IUserIdentity userIdentity = identityService.GetUserIdentity(userId);
			//获取流程对应的审批角色对象，并从角色-提取者字典中获取角色对应的提取者
			InstanceCollection instances = new InstanceCollection();
			ApprovalRole role = WorkflowUtility.GetUserRoleByName(workflowName, roleName);
			if (role != null)
			{
				//用户不在该角色
				if (!userInRole.IsUserInRole(userIdentity.GetUserId(), role.Name))
					throw new ApplicationException(string.Format("{0} is not in role {1}", userIdentity.GetUserId(), role.Name));
				//取得角色的任务提取器
				TaskDistiller distiller = GetRoleDistiller(roleName);
				instances.AddRange(distiller.Distill(workflowName, userIdentity, role, startDate, endDate));
			}
			return instances;
		}

		/// <summary>
		/// 获取某用户的角色-任务列表结合字典
		/// </summary>
		/// <param name="workflowName">流程名称</param>
		/// <param name="userId">用户Id</param>
		/// <param name="roleName">角色名称</param>
		/// <param name="startDate">起始时间</param>
		/// <param name="endDate">终止时间</param>
		/// <returns></returns>
		public Dictionary<string, TaskListCollection> DistillTasklists(string workflowName, string userId, string roleName, DateTime startDate, DateTime endDate)
		{
			return ConvertToTasklists(userId, DistillInstances(workflowName, userId, roleName, startDate, endDate));
		}

		/// <summary>
		/// 将角色-实例集合转化为用户的任务列表
		/// </summary>
		/// <param name="userId">用户Id.</param>
		/// <param name="instances">角色-实例集合.</param>
		/// <returns></returns>
		private Dictionary<string, TaskListCollection> ConvertToTasklists(string userId,  InstanceCollection instances)
		{
			Dictionary<string, TaskListCollection> taskLists = new Dictionary<string, TaskListCollection>();
			IUserIdentity userIdentity = identityService.GetUserIdentity(userId);
			List<ApprovalRole> allRoles = instances.GetAllRoles();

			//按角色分别呈现角色对应的任务列表
			foreach (ApprovalRole role in allRoles)
			{
				InstanceCollection c = instances.GetByRole(role);

				TaskListCollection lists = new TaskListCollection();
				//获取该角色对应的提取器对象
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
					//给任务列表名称赋值
					if (!string.IsNullOrEmpty(this.taskName))
						list.Cagegory = this.taskName;
					lists.Merge(list);
				}
				taskLists.Add(role.Name, lists);
			}
			return taskLists;
		}
		/// <summary>
		/// 将制定的角色-任务列表呈现为Html
		/// </summary>
		/// <param name="taskLists">角色-任务列表集合</param>
		/// <returns></returns>
		private string Render(Dictionary<string, TaskListCollection> taskLists)
		{
			return Render(taskLists, mergeRoles);
		}

		/// <summary>
		/// 将任务列表呈现为HTML.
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
			//按角色分别呈现角色对应的任务列表
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
					//合并呈现
					sb.Append(distiller.Render.TaskListsToHtml(taskLists[roleName].ToArray()));
				}
			}
			return sb.ToString();
		}


		private IIdentityService identityService;
		private IUserInRole userInRole;
		/// <summary>
		/// 设定任务列列表的名称.在形成一个或多个任务列表时,是否制定任务列表的名称,如果为null,那么将使用任务列表自己的名称.
		/// 一般在合共所有角色时配置使用.
		/// </summary>
		private string taskName;
		/// <summary>
		/// 是否合并显示所有任务列表.true时将只显示一个标题任务列表,false时,将按用户审批角色显示任务列表.缺省为false.
		/// </summary>
		private bool mergeRoles = false;
		private string name;

		/// <summary>
		///  该服务对应的应用所有的工作流名称.
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
		/// 缺省的提取器
		/// </summary>
		public TaskDistiller DefalutDistiller
		{
			get { return (TaskDistiller)defaultDistiller.Clone(); }
		}
		/// <summary>
		/// 获取某角色的任务提取器
		/// </summary>
		/// <param name="roleName">角色名称</param>
		/// <returns></returns>
		public TaskDistiller GetRoleDistiller(string roleName)
		{
			if (distillersMap.ContainsKey(roleName))
				return (TaskDistiller)distillersMap[roleName].Clone();
			return (TaskDistiller)defaultDistiller.Clone();
		}
		#region ICloneable Members

		/// <summary>
		/// 克隆服务，避免重用全局对象
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

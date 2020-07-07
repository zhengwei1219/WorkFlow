using System;
using System.Collections.Generic;
using System.Text;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 工作流存取服务接口
	/// </summary>
	public interface IWorkflowPersistService
	{
		#region 工作流实例存取方法
		/// <summary>
		/// 保存工作流实例
		/// </summary>
		/// <param name="instance"></param>
		StateMachineWorkflowInstance SaveWorkflowInstance(StateMachineWorkflowInstance instance);
		/// <summary>
		/// 插入一条新的工作流
		/// </summary>
		/// <param name="instance"></param>
		StateMachineWorkflowInstance InsertWorkflowInstance(StateMachineWorkflowInstance instance);
		/// <summary>
		/// 获取处于某状态下的所有状态机工作流实例
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="stateName">状态名称</param>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		List<StateMachineWorkflowInstance> GetWorkflowInstance(string workflowName, string[] stateNames, Type type);
		/// <summary>
		/// 按创建者单位，Id，和实例状态获取实例
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="stateName">实例状态名称</param>
		/// <param name="createUserId">创建者Id</param>
		/// <param name="unitCode">创建者单位代码</param>
		/// <returns></returns>
		List<StateMachineWorkflowInstance> GetWorkflowInstance(string workflowName, string[] stateNames, string createUserId, string unitCode);
		/// <summary>
		/// 获取某用户创建的实例
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="createUserId">创建用户的Id</param>
		/// <returns></returns>
		List<StateMachineWorkflowInstance> GetWorkflowInstance(string workflowName, string createUserId);
		/// <summary>
		/// 按实例的状态和实例创建者的单位代码获取实例
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="stateName">实例状态名称</param>
		/// <param name="unitCode">创建该实例的用户单位代码</param>
		/// <returns></returns>
		List<StateMachineWorkflowInstance> GetWorkflowInstance(string workflowName, string[] stateNames, string unitCode);
		/// <summary>
		/// 根据Id获取某状态机工作流实例
		/// </summary>
		/// <param name="id">实例Id</param>
		/// <returns></returns>
		StateMachineWorkflowInstance GetWorkflowInstance(Guid id);
		/// <summary>
		/// 获取某实例
		/// </summary>
		/// <param name="id">实例Id</param>
		/// <param name="isAttachMore">是否装载更多信息</param>
		/// <returns></returns>
		StateMachineWorkflowInstance GetWorkflowInstance(Guid id, bool isAttachMore);
		/// <summary>
		/// 获取某立项对应的实例
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="eaId">立项Id</param>
		/// <param name="isAttachMore">是否装载更多信息</param>
		/// <returns></returns>
		List<StateMachineWorkflowInstance> GetWorkflowInstance(string workflowName, int eaId, bool isAttachMore);
		/// <summary>
		/// 获取指定最后操作时间段和指定状态下的工作流实例
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="startDate">起始日期</param>
		/// <param name="endDate">终止日期</param>
		/// <param name="stateName">实例所处状态</param>
		/// <returns></returns>
		List<StateMachineWorkflowInstance> GetWorkflowInstance(string workflowName, DateTime startDate, DateTime endDate, string[] stateNames);
		/// <summary>
		/// 获取指定最后操作时间段和指定状态下的工作流实例
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="startDate">起始日期</param>
		/// <param name="endDate">终止日期</param>
		/// <param name="stateName">实例所处状态</param>
		/// <param name="unitCode">单位代码.</param>
		/// <returns></returns>
		List<StateMachineWorkflowInstance> GetWorkflowInstance(string workflowName, DateTime startDate, DateTime endDate, string[] stateNames, string unitCode);
		/// <summary>
		/// 删除指定工作流
		/// </summary>
		/// <param name="instance"></param>
		void DeleteWorkflowInstance(WorkflowInstance instance);
		
		#endregion
	}

	/// <summary>
	/// 
	/// </summary>
	public abstract class WorkflowPersistService : IWorkflowPersistService
	{
		#region IWorkflowPersistService Members
		/// <summary>
		/// 保存工作流实例
		/// </summary>
		/// <param name="instance"></param>
		protected abstract StateMachineWorkflowInstance SaveWorkflowInstance(StateMachineWorkflowInstance instance);
		/// <summary>
		/// 保存工作流实例
		/// </summary>
		/// <param name="instance"></param>
		StateMachineWorkflowInstance IWorkflowPersistService.SaveWorkflowInstance(StateMachineWorkflowInstance instance)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");
			return SaveWorkflowInstance(instance);
		}
		/// <summary>
		/// 插入一条新的工作流
		/// </summary>
		/// <param name="instance"></param>
		protected abstract StateMachineWorkflowInstance InsertWorkflowInstance(StateMachineWorkflowInstance instance);
		/// <summary>
		/// 插入一条新的工作流
		/// </summary>
		/// <param name="instance"></param>
		StateMachineWorkflowInstance IWorkflowPersistService.InsertWorkflowInstance(StateMachineWorkflowInstance instance)
		{
			return InsertWorkflowInstance(instance);
		}
		/// <summary>
		/// 获取处于某状态下的所有状态机工作流实例
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="stateName">状态名称</param>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		protected abstract List<StateMachineWorkflowInstance> GetWorkflowInstance(string workflowName, string[] stateNames, Type type);
		/// <summary>
		/// 获取处于某状态下的所有状态机工作流实例
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="stateName">状态名称</param>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		List<StateMachineWorkflowInstance> IWorkflowPersistService.GetWorkflowInstance(string workflowName, string[] stateNames, Type type)
		{
			if (string.IsNullOrEmpty(workflowName))
				throw new ArgumentNullException("workflowName");
			if (stateNames==null || stateNames.Length==0)
				throw new ArgumentNullException("stateNames");
			if (type == null)
				throw new ArgumentNullException("type");
			return GetWorkflowInstance(workflowName, stateNames, type);
		}
		/// <summary>
		/// 按创建者单位，Id，和实例状态获取实例
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="stateName">实例状态名称</param>
		/// <param name="createUserId">创建者Id</param>
		/// <param name="unitCode">创建者单位代码</param>
		/// <returns></returns>
		protected abstract List<StateMachineWorkflowInstance> GetWorkflowInstance(string workflowName, string[] stateNames, string createUserId, string unitCode);
		/// <summary>
		/// 按创建者单位，Id，和实例状态获取实例
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="stateName">实例状态名称</param>
		/// <param name="createUserId">创建者Id</param>
		/// <param name="unitCode">创建者单位代码</param>
		/// <returns></returns>
		List<StateMachineWorkflowInstance> IWorkflowPersistService.GetWorkflowInstance(string workflowName, string[] stateNames, string createUserId, string unitCode)
		{
			if (string.IsNullOrEmpty(workflowName))
				throw new ArgumentNullException("workflowName");
			if (stateNames == null || stateNames.Length == 0)
				throw new ArgumentNullException("stateNames");
			if (string.IsNullOrEmpty(createUserId))
				throw new ArgumentNullException("createUserId");
			return GetWorkflowInstance(workflowName, stateNames, createUserId, unitCode);
		}
		/// <summary>
		/// 获取某用户创建的实例
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="createUserId">创建用户的Id</param>
		/// <returns></returns>
		protected abstract List<StateMachineWorkflowInstance> GetWorkflowInstance(string workflowName, string createUserId);
		/// <summary>
		/// 获取某用户创建的实例
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="createUserId">创建用户的Id</param>
		/// <returns></returns>
		List<StateMachineWorkflowInstance> IWorkflowPersistService.GetWorkflowInstance(string workflowName, string createUserId)
		{
			if (string.IsNullOrEmpty(workflowName))
				throw new ArgumentNullException("workflowName");
			if (string.IsNullOrEmpty(createUserId))
				throw new ArgumentNullException("createUserId");
			return GetWorkflowInstance(workflowName, createUserId);
		}
		/// <summary>
		/// 按实例的状态和实例创建者的单位代码获取实例
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="stateName">实例状态名称</param>
		/// <param name="unitCode">创建该实例的用户单位代码</param>
		/// <returns></returns>
		protected abstract List<StateMachineWorkflowInstance> GetWorkflowInstance(string workflowName, string[] stateNames, string unitCode);
		/// <summary>
		/// 按实例的状态和实例创建者的单位代码获取实例
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="stateName">实例状态名称</param>
		/// <param name="unitCode">创建该实例的用户单位代码</param>
		/// <returns></returns>
		List<StateMachineWorkflowInstance> IWorkflowPersistService.GetWorkflowInstance(string workflowName, string[] stateNames, string unitCode)
		{
			if (string.IsNullOrEmpty(workflowName))
				throw new ArgumentNullException("workflowName");
			if (stateNames == null || stateNames.Length == 0)
				throw new ArgumentNullException("stateNames");
			return GetWorkflowInstance(workflowName, stateNames, unitCode);
		}
		/// <summary>
		/// 根据Id获取某状态机工作流实例
		/// </summary>
		/// <param name="id">实例Id</param>
		/// <returns></returns>
		protected abstract StateMachineWorkflowInstance GetWorkflowInstance(Guid id);
		/// <summary>
		/// 根据Id获取某状态机工作流实例
		/// </summary>
		/// <param name="id">实例Id</param>
		/// <returns></returns>
		StateMachineWorkflowInstance IWorkflowPersistService.GetWorkflowInstance(Guid id)
		{
			return GetWorkflowInstance(id);
		}
		/// <summary>
		/// 获取某实例
		/// </summary>
		/// <param name="id">实例Id</param>
		/// <param name="isAttachMore">是否装载更多信息</param>
		/// <returns></returns>
		protected abstract StateMachineWorkflowInstance GetWorkflowInstance(Guid id, bool isAttachMore);
		/// <summary>
		/// 获取某实例
		/// </summary>
		/// <param name="id">实例Id</param>
		/// <param name="isAttachMore">是否装载更多信息</param>
		/// <returns></returns>
		StateMachineWorkflowInstance IWorkflowPersistService.GetWorkflowInstance(Guid id, bool isAttachMore)
		{
			return GetWorkflowInstance(id, isAttachMore);
		}
		/// <summary>
		/// 获取某立项对应的实例
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="eaId">立项Id</param>
		/// <param name="isAttachMore">是否装载更多信息</param>
		/// <returns></returns>
		protected abstract List<StateMachineWorkflowInstance> GetWorkflowInstance(string workflowName, int eaId, bool isAttachMore);
		/// <summary>
		/// 获取某立项对应的实例
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="eaId">立项Id</param>
		/// <param name="isAttachMore">是否装载更多信息</param>
		/// <returns></returns>
		List<StateMachineWorkflowInstance> IWorkflowPersistService.GetWorkflowInstance(string workflowName, int eaId, bool isAttachMore)
		{
			return GetWorkflowInstance(workflowName, eaId, isAttachMore);
		}
		/// <summary>
		/// 获取指定最后操作时间段和指定状态下的工作流实例
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="startDate">起始日期</param>
		/// <param name="endDate">终止日期</param>
		/// <param name="stateName">实例所处状态</param>
		/// <returns></returns>
		protected abstract List<StateMachineWorkflowInstance> GetWorkflowInstance(string workflowName, DateTime startDate, DateTime endDate, string[] stateNames);
		/// <summary>
		/// 获取指定最后操作时间段和指定状态下的工作流实例
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="startDate">起始日期</param>
		/// <param name="endDate">终止日期</param>
		/// <param name="stateName">实例所处状态</param>
		/// <returns></returns>
		List<StateMachineWorkflowInstance> IWorkflowPersistService.GetWorkflowInstance(string workflowName, DateTime startDate, DateTime endDate, string[] stateNames)
		{
			if (string.IsNullOrEmpty(workflowName))
				throw new ArgumentNullException("workflowName");
			if (stateNames == null || stateNames.Length == 0)
				throw new ArgumentNullException("stateNames");
			return GetWorkflowInstance(workflowName, startDate, endDate, stateNames);
		}
		/// <summary>
		/// 获取指定最后操作时间段和指定状态下的工作流实例
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="startDate">起始日期</param>
		/// <param name="endDate">终止日期</param>
		/// <param name="stateName">实例所处状态</param>
		/// <param name="unitCode">单位代码.</param>
		/// <returns></returns>
		protected abstract List<StateMachineWorkflowInstance> GetWorkflowInstance(string workflowName, DateTime startDate, DateTime endDate, string[] stateNames, string unitCode);
		/// <summary>
		/// 获取指定最后操作时间段和指定状态下的工作流实例
		/// </summary>
		/// <param name="workflowName">Name of the workflow.</param>
		/// <param name="startDate">起始日期</param>
		/// <param name="endDate">终止日期</param>
		/// <param name="stateName">实例所处状态</param>
		/// <param name="unitCode">单位代码.</param>
		/// <returns></returns>
		List<StateMachineWorkflowInstance> IWorkflowPersistService.GetWorkflowInstance(string workflowName, DateTime startDate, DateTime endDate, string[] stateNames, string unitCode)
		{
			if (string.IsNullOrEmpty(workflowName))
				throw new ArgumentNullException("workflowName");
			if (stateNames == null || stateNames.Length == 0)
				throw new ArgumentNullException("stateNames");
			return GetWorkflowInstance(workflowName, startDate, endDate, stateNames, unitCode);
		}
		/// <summary>
		/// 删除指定工作流
		/// </summary>
		/// <param name="instance"></param>
		protected abstract void DeleteWorkflowInstance(WorkflowInstance instance);
		/// <summary>
		/// 删除指定工作流
		/// </summary>
		/// <param name="instance"></param>
		void IWorkflowPersistService.DeleteWorkflowInstance(WorkflowInstance instance)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");
			DeleteWorkflowInstance(instance);
		}

		#endregion
	}
}

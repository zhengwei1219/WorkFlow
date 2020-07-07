using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 工作流定义持久化服务接口
	/// </summary>
	public interface IWorkFlowDefinePersistService
	{
		/// <summary>
		///获取工作流配置对象
		/// </summary>
		/// <returns></returns>
		WorkflowConfig GetConfig();
		/// <summary>
		/// Gets Root(起始)工作流名称
		/// </summary>
		/// <param name="applicationName">Name of the application.</param>
		/// <returns></returns>
		string GetRoot(string applicationName);
		/// <summary>
		/// 根据工作流名称获取一个工作流定义实例
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		WorkFlowDefine GetWorkflowDefine(string name);
		/// <summary>
		/// 获取所有有效工作流定义的名字
		/// </summary>
		/// <returns></returns>
		string[] GetAllWorkflowDefineName(string applicationName);
		/// <summary>
		/// Gets all workflow.
		/// </summary>
		/// <returns></returns>
		WorkFlowDefine[] GetAllWorkflow(string applicationName);
		/// <summary>
		/// Saves the specified XML.
		/// </summary>
		/// <param name="xml">The XML.</param>
		/// <param name="applicationName">Name of the application.</param>
		void Save(string xml, string applicationName);
		/// <summary>
		/// Saves the specified workflow.
		/// </summary>
		/// <param name="workflow">The workflow.</param>
		/// <param name="applicationName">Name of the application.</param>
		void Save(WorkFlowDefine workflow, string applicationName);
		/// <summary>
		/// Deletes the specified id.
		/// </summary>
		/// <param name="id">The id.</param>
		void Delete(Guid id);
		/// <summary>
		/// Gets the specified id.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		WorkFlowDefine Get(Guid id);
	}
}

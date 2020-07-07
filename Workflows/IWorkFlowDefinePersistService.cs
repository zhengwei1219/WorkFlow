using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace OilDigital.Workflows
{
	/// <summary>
	/// ����������־û�����ӿ�
	/// </summary>
	public interface IWorkFlowDefinePersistService
	{
		/// <summary>
		///��ȡ���������ö���
		/// </summary>
		/// <returns></returns>
		WorkflowConfig GetConfig();
		/// <summary>
		/// Gets Root(��ʼ)����������
		/// </summary>
		/// <param name="applicationName">Name of the application.</param>
		/// <returns></returns>
		string GetRoot(string applicationName);
		/// <summary>
		/// ���ݹ��������ƻ�ȡһ������������ʵ��
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		WorkFlowDefine GetWorkflowDefine(string name);
		/// <summary>
		/// ��ȡ������Ч���������������
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

using System;
using System.Collections.Generic;
using System.Text;

namespace OilDigital.Workflows
{
	/// <summary>
	/// �����б���ֳ�html�ӿ�
	/// </summary>
	public interface ITaskRender
	{
		/// <summary>
		/// �����������ѯΪhtml
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		string TaskListsToHtml(TaskList[] list);
		/// <summary>
		/// ��һ���������ΪHtml
		/// </summary>
		/// <param name="one">The one.</param>
		/// <param name="listID">The list ID.</param>
		/// <returns></returns>
		string TaskListToHtml(TaskList one, string listID);
	}
}

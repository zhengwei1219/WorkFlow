using System;
using System.Collections.Generic;
using System.Text;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 任务列表呈现成html接口
	/// </summary>
	public interface ITaskRender
	{
		/// <summary>
		/// 将多类任务查询为html
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		string TaskListsToHtml(TaskList[] list);
		/// <summary>
		/// 将一类任务呈现为Html
		/// </summary>
		/// <param name="one">The one.</param>
		/// <param name="listID">The list ID.</param>
		/// <returns></returns>
		string TaskListToHtml(TaskList one, string listID);
	}
}

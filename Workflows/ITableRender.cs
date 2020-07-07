using System;
using System.Collections.Generic;
namespace OilDigital.Workflows
{
	/// <summary>
	/// 将任务列表呈现为具体的html表格的表头和内容的接口
	/// </summary>
	public interface ITableTaskRender
	{
		/// <summary>
		/// 呈现表头:&gt;thead标记,以及tr,th标记
		/// </summary>
		/// <returns></returns>
		string RenderTablehead();
		/// <summary>
		/// Gets or sets the option.
		/// </summary>
		/// <value>The option.</value>
		TaskListRendingOption Option { get;}
		/// <summary>
		/// Renders the table row.
		/// </summary>
		/// <param name="category">任务类别</param>
		/// <param name="oneItem">单位任务项目</param>
		/// <param name="seqNo">当前行的序号</param>
		/// <returns></returns>
		string RenderTableRow(string category, ITaskItem oneItem, int seqNo);

		/// <summary>
		/// 单任务列表的项目为空时的呈现方式
		/// </summary>
		/// <returns></returns>
		string RendEmptyList();

		/// <summary>
		/// 将一个单项操作呈现为一个链接
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns></returns>
		string TaskActionItemToLink(ITaskActionItem item);
		/// <summary>
		/// Tasks the action items to HTML.
		/// </summary>
		/// <param name="actionItems">The action items.</param>
		/// <returns></returns>
		string TaskActionItemsToHtml(List<ITaskActionItem> actionItems);
	}
}

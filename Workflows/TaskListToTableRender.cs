using System;
using System.Collections.Generic;
using System.Text;
using OilDigital.Workflows;


namespace  OilDigital.Workflows
{
	/// <summary>
	/// 呈现任务列表的方法为带标题的表格
	/// </summary>
	public class TaskListToTableRender : ITaskRender
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TaskListToTableRender"/> class.
		/// </summary>
		public TaskListToTableRender() { }
		private TaskListRendingOption option;

		private ITableTaskRender tableRender;
		/// <summary>
		/// Sets the table render.
		/// </summary>
		/// <value>The table render.</value>
		public ITableTaskRender TableRender
		{
			set 
			{
				this.tableRender = value;
				this.option = tableRender.Option;
			}
		}
		/// <summary>
		/// 构造一个html程序实例,不显示状态列,但在鼠标移动到立项名称上时显示审批意见
		/// </summary>
		public TaskListToTableRender(ITableTaskRender tableRender)
		{
			if (tableRender == null)
				throw new ArgumentNullException("tableRender 参数不能为空");
			this.tableRender = tableRender;
			this.option = tableRender.Option;
		}


		#region ITaskRender Members

		/// <summary>
		/// 将多类任务查询为html
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		public string TaskListsToHtml(TaskList[] list)
		{
			Array.Sort(list);
			StringBuilder html = new StringBuilder();
			//判断是否合并显示所有任务列表
			if (option.MergeAllTaskList)
			{
				//判断是否所有列表都为空
				bool allEmpty = true;
				for (int i = 0; i < list.Length; i++)
				{
					if (list[i].Items.Length > 0)
					{
						allEmpty = false;
						break;
					}
				}

				//如果所有列表都为空,那么在不RendEmptyTaskList=false 时返回空
				if (allEmpty && !this.option.RendEmptyTaskList) 
					return "";

				//获取总条目数
				int count = 0;
				for (int i = 0; i < list.Length; i++)
				{
					count = count + list[i].Items.Length;
				}
				RendTaskListTitleStart(option.CategoryNameWhenMergingAll, "_" + option.CategoryNameWhenMergingAll + "_0", count, html);

				if(allEmpty)
					html.Append(tableRender.RendEmptyList());
				else
				{
					int seq = 0;
					for (int i = 0; i < list.Length; i++)
					{
						RendTaskListList(list[i], html, seq);
						seq = seq + list[i].Items.Length;
					}
				}
				RendTaskListTitleEnd(html);
				return html.ToString();
			}
			else
			{

				for (int i = 0; i < list.Length; i++)
				{
					html.Append(TaskListToHtml(list[i], "_" + list[i].Cagegory + "_" + i.ToString()));
				}
				return html.ToString();
			}
		}


		/// <summary>
		/// 将一类任务呈现为Html
		/// </summary>
		/// <param name="one">The one.</param>
		/// <param name="listID">The list ID.</param>
		/// <returns></returns>
		public string TaskListToHtml(TaskList one, string listID)
		{
			if (!this.option.RendEmptyTaskList 
				&& (one == null || one.Items.Length == 0))
				return "";
			StringBuilder html = new StringBuilder();
			
			//呈现标题
			RendTaskListTitleStart(one.Cagegory, listID, one.Items.Length, html);

			//呈现列表
			if (one.Items.Length == 0)
				html.Append(tableRender.RendEmptyList());
			else
				RendTaskListList(one, html);
			
			//呈现结束部分
			RendTaskListTitleEnd(html);

			return html.ToString();
		}

		private void RendTaskListTitleStart(string category, string listID, int itemCount,StringBuilder html)
		{
			html.AppendFormat("<DIV class='{0}'>", this.option.CssClassName);
			if (option.ShowTitle && !string.IsNullOrEmpty(category))
			{
				html.Append("<div class='tasklisttitle'><div></div>");
				if (option.IsCollapsable)
					html.AppendFormat("<SPAN style='FLOAT: right'>&nbsp;&nbsp;<A onclick=\"return toggle_collapse('tasklist{0}')\" href='javascript:void(0);'><IMG id=collapseimg_tasklist{0} alt=收缩或者展开 src='../images/collapse_1tcat.gif' border=0></A></SPAN>", listID);
				html.Append("<span class='categoryname'>");
				html.Append(category.Length >= 2 ? category.Substring(2, category.Length - 2) : category);

				//显示条数
				if (option.ShowTaskCountAtCategory)
					html.AppendFormat("({0}条)", itemCount);
				html.Append("</span>");
				html.Append("</div>");
			}

			//生成表头
			html.AppendFormat("<DIV class='tasklistDiv' id=collapseobj_tasklist{0}>", listID);
			html.AppendFormat("<table id='taskTable{0}' cellspacing=1 cellpadding=2px width=100% class='tasklist tablesorter'> ", listID);
			html.Append(tableRender.RenderTablehead());
			html.Append("<tbody>");
		}

		private void RendTaskListTitleEnd(StringBuilder html)
		{
			html.Append("</tbody>");
			html.Append("</table>");
			html.Append("</DIV>");
			html.Append("</DIV>");
		}


		private void RendTaskListList(TaskList one, StringBuilder html)
		{
			RendTaskListList(one, html, 0);
		}

		/// <summary>
		/// 将一类任务的所有条目依次生成为表格的行.
		/// </summary>
		/// <param name="one">任务列表对象的实例</param>
		/// <param name="html">The HTML.</param>
		/// <param name="seqNo">The seq no.</param>
		private void RendTaskListList(TaskList one, StringBuilder html, int seqNo)
		{
			for (int i = 0; i < one.Items.Length; i++)
			{
				ITaskItem item = one.Items[i];
				html.Append(tableRender.RenderTableRow(one.Cagegory, item, i + seqNo));
			}
		}

		/// <summary>
		/// 将任务项目呈现为工具条的样式.主要用于打开具体审批件时生成操作工具条.
		/// </summary>
		/// <param name="actionItems">The action items.</param>
		/// <returns></returns>
		public string TaskActionItemListToToolsBar(List<ITaskActionItem> actionItems)
		{
			StringBuilder html = new StringBuilder();
			foreach (TaskActionItem action in actionItems)
			{
				if (action.Name.IndexOf("打印") >= 0)
					continue;
				html.Append(tableRender.TaskActionItemToLink(action));
			}
			return html.ToString();
		}

		#endregion
	}
}

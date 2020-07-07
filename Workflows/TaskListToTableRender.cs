using System;
using System.Collections.Generic;
using System.Text;
using OilDigital.Workflows;


namespace  OilDigital.Workflows
{
	/// <summary>
	/// ���������б�ķ���Ϊ������ı��
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
		/// ����һ��html����ʵ��,����ʾ״̬��,��������ƶ�������������ʱ��ʾ�������
		/// </summary>
		public TaskListToTableRender(ITableTaskRender tableRender)
		{
			if (tableRender == null)
				throw new ArgumentNullException("tableRender ��������Ϊ��");
			this.tableRender = tableRender;
			this.option = tableRender.Option;
		}


		#region ITaskRender Members

		/// <summary>
		/// �����������ѯΪhtml
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		public string TaskListsToHtml(TaskList[] list)
		{
			Array.Sort(list);
			StringBuilder html = new StringBuilder();
			//�ж��Ƿ�ϲ���ʾ���������б�
			if (option.MergeAllTaskList)
			{
				//�ж��Ƿ������б�Ϊ��
				bool allEmpty = true;
				for (int i = 0; i < list.Length; i++)
				{
					if (list[i].Items.Length > 0)
					{
						allEmpty = false;
						break;
					}
				}

				//��������б�Ϊ��,��ô�ڲ�RendEmptyTaskList=false ʱ���ؿ�
				if (allEmpty && !this.option.RendEmptyTaskList) 
					return "";

				//��ȡ����Ŀ��
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
		/// ��һ���������ΪHtml
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
			
			//���ֱ���
			RendTaskListTitleStart(one.Cagegory, listID, one.Items.Length, html);

			//�����б�
			if (one.Items.Length == 0)
				html.Append(tableRender.RendEmptyList());
			else
				RendTaskListList(one, html);
			
			//���ֽ�������
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
					html.AppendFormat("<SPAN style='FLOAT: right'>&nbsp;&nbsp;<A onclick=\"return toggle_collapse('tasklist{0}')\" href='javascript:void(0);'><IMG id=collapseimg_tasklist{0} alt=��������չ�� src='../images/collapse_1tcat.gif' border=0></A></SPAN>", listID);
				html.Append("<span class='categoryname'>");
				html.Append(category.Length >= 2 ? category.Substring(2, category.Length - 2) : category);

				//��ʾ����
				if (option.ShowTaskCountAtCategory)
					html.AppendFormat("({0}��)", itemCount);
				html.Append("</span>");
				html.Append("</div>");
			}

			//���ɱ�ͷ
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
		/// ��һ�������������Ŀ��������Ϊ������.
		/// </summary>
		/// <param name="one">�����б�����ʵ��</param>
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
		/// ��������Ŀ����Ϊ����������ʽ.��Ҫ���ڴ򿪾���������ʱ���ɲ���������.
		/// </summary>
		/// <param name="actionItems">The action items.</param>
		/// <returns></returns>
		public string TaskActionItemListToToolsBar(List<ITaskActionItem> actionItems)
		{
			StringBuilder html = new StringBuilder();
			foreach (TaskActionItem action in actionItems)
			{
				if (action.Name.IndexOf("��ӡ") >= 0)
					continue;
				html.Append(tableRender.TaskActionItemToLink(action));
			}
			return html.ToString();
		}

		#endregion
	}
}

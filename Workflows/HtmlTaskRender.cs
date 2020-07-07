using System;
using System.Collections.Generic;
using System.Text;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 
	/// </summary>
	public class HtmlTaskRender : ITaskRender
	{
		private bool showState;

		/// <summary>
		/// 是否显示立项的当前状态
		/// </summary>
		public bool ShowState
		{
			get { return showState; }
			set { showState = value; }
		}

		private bool showApplyUnit;

		/// <summary>
		/// Gets or sets a value indicating whether [show apply unit].
		/// </summary>
		/// <value><c>true</c> if [show apply unit]; otherwise, <c>false</c>.</value>
		public bool ShowApplyUnit
		{
			get { return showApplyUnit; }
			set { showApplyUnit = value; }
		}

		private bool showUnitTitleCode;

		/// <summary>
		/// Gets or sets a value indicating whether [show unit title code].
		/// </summary>
		/// <value><c>true</c> if [show unit title code]; otherwise, <c>false</c>.</value>
		public bool ShowUnitTitleCode
		{
			get { return showUnitTitleCode; }
			set { showUnitTitleCode = value; }
		}

		private bool showSolutionOnMouseOver;

		/// <summary>
		/// 是否在鼠标移动到立项名称上面时显示立项的审批意见
		/// </summary>
		public bool ShowSolutionOnMouseOver
		{
			get { return showSolutionOnMouseOver; }
			set { showSolutionOnMouseOver = value; }
		}


		/// <summary>
		/// 构造一个html程序实例,不显示状态列,但在鼠标移动到立项名称上时显示审批意见
		/// </summary>
		public HtmlTaskRender() : this(false, true) { }

		/// <summary>
		/// 构造一个html程序实例
		/// </summary>
		/// <param name="showState">是否显示状态列</param>
		/// <param name="showSolutionOnMouseOver">是否在鼠标移动到立项名称上时显示审批意见</param>
		public HtmlTaskRender(bool showState, bool showSolutionOnMouseOver)
		{
			this.showState = showState;
			this.showApplyUnit = true;
			this.showSolutionOnMouseOver = showSolutionOnMouseOver;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HtmlTaskRender"/> class.
		/// </summary>
		/// <param name="showState">if set to <c>true</c> [show state].</param>
		/// <param name="showApplyUnit">if set to <c>true</c> [show apply unit].</param>
		/// <param name="showUnitTitleCode">if set to <c>true</c> [show unit title code].</param>
		public HtmlTaskRender(bool showState, bool showApplyUnit, bool showUnitTitleCode)
		{
			this.showState = showState;
			this.showUnitTitleCode = showUnitTitleCode;
			this.showApplyUnit = showApplyUnit;
			this.showSolutionOnMouseOver = true;
		}

		#region ITaskRender Members

		/// <summary>
		/// 将多类任务查询为html
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		public string TaskListsToHtml(TaskList[] list)
		{
			StringBuilder html = new StringBuilder();
			for (int i = 0; i < list.Length; i++)
			{
				html.Append(TaskListToHtml(list[i], "_" + list[i].Cagegory + "_" + i.ToString()));
			}
			return html.ToString();
		}


		/// <summary>
		/// Tasks the list to HTML.
		/// </summary>
		/// <param name="one">The one.</param>
		/// <param name="listID">The list ID.</param>
		/// <returns></returns>
		public string TaskListToHtml(TaskList one, string listID)
		{
			StringBuilder html = new StringBuilder();
			html.Append("<DIV class='oneblock'>");
			html.Append(@"<TABLE cellSpacing=0 cellPadding=0 width='100%'><TBODY><TR>
				<TD><B class=rnd><B class=rnd1 ></B><B class=rnd2></B></B></TD></TR></TBODY></table>");
			html.Append("<DIV class=tcat style='PADDING-RIGHT: 5px; PADDING-LEFT: 5px'>");
			html.AppendFormat("<SPAN style='FLOAT: right'>&nbsp;&nbsp;<A onclick=\"return toggle_collapse('tasklist{0}')\" href='javascript:void(0);'><IMG id=collapseimg_tasklist{0} alt=收缩或者展开 src='../images/collapse_tcat.gif' border=0></A>", listID);
			html.AppendFormat("</SPAN>{0}({1}条)</DIV>", one.Cagegory.Substring(2, one.Cagegory.Length - 2), one.Items.Length);
			html.AppendFormat("<DIV class='tlist' id=collapseobj_tasklist{0}>", listID);
			html.AppendFormat("<table id='taskTable{0}' cellspacing=0 cellpadding=2px width=100%>", listID);
			html.Append("<thead><tr>");
			html.AppendFormat("<th onclick=sortTable('taskTable{0}',0) style='width:30%;cursor:pointer;'>项目</th>", listID);
			html.AppendFormat("<th onclick=sortTable('taskTable{0}',1) style='cursor:pointer;'>操作</th>", listID);
			if (showApplyUnit)
				html.AppendFormat("<th onclick=sortTable('taskTable{0}',2); style='width:15%;cursor:pointer;'>申报单位</th>", listID);
			if (showUnitTitleCode)
				html.AppendFormat("<th onclick=sortTable('taskTable{0}',3); style='width:15%;cursor:pointer;'>申报文号</th>", listID);
			html.AppendFormat("<th onclick=sortTable('taskTable{0}',4); style='width:10%;cursor:pointer;'>收文编号</th>", listID);
			html.AppendFormat("<th onclick=sortTable('taskTable{0}',5); style='width:15%;cursor:pointer;'>最后处理时间</th>", listID);
			if (showState)
				html.AppendFormat("<th onclick=sortTable('taskTable{0}',6); style='cursor:pointer;'>当前状态</th>", listID);
			html.Append("</tr></thead>");
			html.Append("<tbody>");
			foreach (TaskItem oneItem in one.Items)
			{
				html.Append(TaskItemToHtml(oneItem));
			}
			html.Append("</tbody>");
			html.Append("</table>");
			html.Append("</DIV></div>");
			return html.ToString();
		}

		/// <summary>
		/// Tasks the item to HTML.
		/// </summary>
		/// <param name="taskItem">The task item.</param>
		/// <returns></returns>
		public string TaskItemToHtml(ITaskItem taskItem)
		{
			TaskItem oneItem = (TaskItem)taskItem;
			StringBuilder html = new StringBuilder();
			html.Append("<tr>");
			html.Append("<td>");
			
			//if(showSolutionOnMouseOver)		html.AppendFormat("<span class=articleLink onmouseover=\"showInfo(this,{0});\" onmouseout=\"closeInfo({0})\">", oneItem.EaId);

			html.Append(TitleHtml(oneItem));
			
			//if(showSolutionOnMouseOver) 	html.Append("</span>");
			//html.AppendFormat("&nbsp;<a class='approvalsolution' onclick='openSolutionView({0},\"{1}\")' href='javascript:void(0)' rel='../approval/ViewApproval_S.aspx?id={0}&instanceId={1}&type=solution' title='审批意见'><img src='../images/eaSolution.gif' /></a>", oneItem.EaId, oneItem.InstanceId.ToString());
			html.AppendFormat("&nbsp;<a class='approvalsolution' onclick='openApprovalRecordView({0},\"{1}\")' href='javascript:void(0)' rel='../approval/ViewApproval_S.aspx?id={0}&instanceId={1}&type=record' title='审批记录'><img src='../images/earecord.gif' /></a>", oneItem.EaId, oneItem.InstanceId.ToString());
			html.Append("</td>");
			html.Append("<td nowrap>");
			foreach (ITaskActionItem action in oneItem.ActionItems)
			{
				html.Append(TaskActionItemToHtml(action));
			}
			html.Append("</td>");
			if (showApplyUnit)
				html.AppendFormat("<td nowrap>{0}</td>", oneItem.ApplyUnitName);
			if (showUnitTitleCode)
				html.AppendFormat("<td nowrap>{0}</td>", oneItem.ApplyLetterNumber);
			html.AppendFormat("<td nowrap>{0}</td>", oneItem.ReceivedLetterNumber);
			html.AppendFormat("<td nowrap >{0:yyyy/MM/dd HH:mm}</td>", oneItem.PersistTime);
			if (showState)
			{
				html.AppendFormat("<td nowrap>{0}</td>", oneItem.CurrentState);
			}
			html.Append("</tr>");
			return html.ToString();
		}

		/// <summary>
		/// Tasks the action item to HTML.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns></returns>
		public string TaskActionItemToHtml(ITaskActionItem item)
		{
			return string.Format("<a class='view' href=\"{0}\" onclick=\"{1}\" title='{3}'>{2}</a>",
				string.IsNullOrEmpty(item.ActionUrl) ? "javascript:void(0)" : item.ActionUrl,
				item.ActionScript,
				item.Name,
				item.Description);
		}
		/// <summary>
		/// Tasks the action item to HTML.
		/// </summary>
		/// <param name="oneItem">The one item.</param>
		/// <returns></returns>
		public string TitleHtml(ITaskItem oneItem)
		{
			TaskActionItem item= oneItem.ViewAction;
			return string.Format("<a class='{5}' href=\"{0}\" onclick=\"{1}\" title='{6}' rel='../approval/ViewApproval_S.aspx?id={3}&instanceId={4}&type=solution' title='审批意见'>{2}</a>",
				string.IsNullOrEmpty(item.ActionUrl) ? "javascript:void(0)" :  item.ActionUrl,
				item.ActionScript,
				item.Name,
				oneItem.EaId,
				oneItem.InstanceId,
				this.showSolutionOnMouseOver?"view approvalsolution":"view",
				this.showSolutionOnMouseOver?"审批意见": item.Description
				);
		}

		/// <summary>
		/// Tasks the action item list to tools bar.
		/// </summary>
		/// <param name="actionItems">The action items.</param>
		/// <returns></returns>
		public string TaskActionItemListToToolsBar(List<ITaskActionItem> actionItems)
		{
			StringBuilder html = new StringBuilder();
			foreach (ITaskActionItem action in actionItems)
			{
				html.Append(TaskActionItemToHtml(action));
			}
			return html.ToString();
		}

		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using OilDigital.Workflows;
using System.Xml.Serialization;


namespace OilDigital.Workflows
{
	/// <summary>
	/// 任务列表呈现为Html时,一些配置项目
	/// </summary>
    [Serializable]
    public class TaskListRendingOption
	{
		private bool renderEmptyTaskList;

		private bool mergeAllTaskList;

		/// <summary>
		/// Gets or sets 是否合并显示所有的任务列表
		/// </summary>
		/// <value><c>true</c> if [merge all task list]; otherwise, <c>false</c>.</value>
        [XmlElement("MergeAllTaskList")]
		public bool MergeAllTaskList
		{
			get { return mergeAllTaskList; }
			set { mergeAllTaskList = value; }
		}

		private bool showTitle;
		/// <summary>
		/// 合并显示任务列表时是否显示标题
		/// </summary>
		/// <value><c>true</c> if [show title when merge]; otherwise, <c>false</c>.</value>
        [XmlElement("ShowTitle")]
        public bool ShowTitle
		{
			get { return showTitle; }
			set { showTitle = value; }
		}

		private bool showTaskCountAtCategory;

		/// <summary>
		/// Gets or sets a value 是否在任务标题中显示任务的数量
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [show task count at category]; otherwise, <c>false</c>.
		/// </value>
		[XmlElement("ShowTaskCountAtCategory")]
		public bool ShowTaskCountAtCategory
		{
			get { return showTaskCountAtCategory; }
			set { showTaskCountAtCategory = value; }
		}


		private string categoryWhenMergingAll = "a.待处理的报批件";

		/// <summary>
		/// Gets or sets 当将所有任务列表合并显示时,显示的类别名称,缺省为:待处理的报批件
		/// </summary>
		/// <value>The category when merging all.</value>
        [XmlElement("CategoryNameWhenMergingAll")]
        public string CategoryNameWhenMergingAll
		{
			get { return categoryWhenMergingAll; }
			set { categoryWhenMergingAll = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating 分类显示时,是否显示没有任务项的任务列表.
		/// </summary>
		/// <value><c>true</c> if [rend empty task list]; otherwise, <c>false</c>.</value>
        [XmlElement("RendEmptyTaskList")]
        public bool RendEmptyTaskList
		{
			get { return renderEmptyTaskList; }
			set { renderEmptyTaskList = value; }
		}


		private bool showStatusIcon;

		/// <summary>
		/// Gets or sets 是否在标题前面显示不同的图标,以表示不同的状态,比如"退回"的状态用退回图标标识.
		/// </summary>
		/// <value><c>true</c> if [display status icon]; otherwise, <c>false</c>.</value>
		[XmlElement("ShowStatusIcon")]
        public bool ShowStatusIcon
		{
			get { return showStatusIcon; }
			set { showStatusIcon = value; }
		}


		private bool showState;

		/// <summary>
		/// Gets or sets a value indicating 是否显示详细的团组状态
		/// </summary>
		/// <value><c>true</c> if [show state]; otherwise, <c>false</c>.</value>
		[XmlElement("ShowState")]
        public bool ShowState
		{
			get { return showState; }
			set { showState = value; }
		}

		private bool showApplyUnit;

		/// <summary>
		/// Gets or sets a value 是否显示申报单位
		/// </summary>
		/// <value><c>true</c> if [show apply unit]; otherwise, <c>false</c>.</value>
		[XmlElement("ShowApplyUnit")]
        public bool ShowApplyUnit
		{
			get { return showApplyUnit; }
			set { showApplyUnit = value; }
		}

	
		private bool showCheckResult;

		/// <summary>
		/// Gets or sets a value 是否显示校验结果信息:用不同的图标显示不同的校验结果
		/// </summary>
		/// <value><c>true</c> if [show check result]; otherwise, <c>false</c>.</value>
		[XmlElement("ShowCheckResult")]
        public bool ShowCheckResult
		{
			get { return showCheckResult; }
			set { showCheckResult = value; }
		}


		private string cssClass;

		/// <summary>
		/// Gets or sets 生成的div的css类名
		/// </summary>
		/// <value>The CSS class.</value>
		[XmlElement("CssClassName")]
        public string CssClassName
		{
			get { return cssClass; }
			set { cssClass = value; }
		}

		private bool isCollapsable;

		/// <summary>
		/// Gets or sets a value indicating 是否将列表显示为可折叠的样子.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is collapsable; otherwise, <c>false</c>.
		/// </value>
		[XmlElement("IsCollapsable")]
        public bool IsCollapsable
		{
			get { return isCollapsable; }
			set { isCollapsable = value; }
		}


		private bool showIndexNo;

		/// <summary>
		/// Gets or sets a value 是否在任务列表中显示每条任务的序号
		/// </summary>
		/// <value><c>true</c> if [show index no]; otherwise, <c>false</c>.</value>
		[XmlElement("ShowIndexNo")]
		public bool ShowIndexNo
		{
			get { return showIndexNo; }
			set { showIndexNo = value; }
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="TaskListRendingOption"/> class.
		/// </summary>
		public TaskListRendingOption()
		{
			this.renderEmptyTaskList = true;
			this.mergeAllTaskList = true;
			this.showTitle = true;
			this.showState = true;
			this.cssClass = "rounded";
			this.showApplyUnit = true;
			this.isCollapsable = true;
			this.showCheckResult = true;
			this.showStatusIcon = true;
			this.showTaskCountAtCategory = true;
			this.showIndexNo = true;
		}
	}
}
using System;
using System.Collections.Generic;
using System.Text;
using OilDigital.Workflows;
using System.Xml.Serialization;


namespace OilDigital.Workflows
{
	/// <summary>
	/// �����б����ΪHtmlʱ,һЩ������Ŀ
	/// </summary>
    [Serializable]
    public class TaskListRendingOption
	{
		private bool renderEmptyTaskList;

		private bool mergeAllTaskList;

		/// <summary>
		/// Gets or sets �Ƿ�ϲ���ʾ���е������б�
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
		/// �ϲ���ʾ�����б�ʱ�Ƿ���ʾ����
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
		/// Gets or sets a value �Ƿ��������������ʾ���������
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


		private string categoryWhenMergingAll = "a.������ı�����";

		/// <summary>
		/// Gets or sets �������������б�ϲ���ʾʱ,��ʾ���������,ȱʡΪ:������ı�����
		/// </summary>
		/// <value>The category when merging all.</value>
        [XmlElement("CategoryNameWhenMergingAll")]
        public string CategoryNameWhenMergingAll
		{
			get { return categoryWhenMergingAll; }
			set { categoryWhenMergingAll = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating ������ʾʱ,�Ƿ���ʾû��������������б�.
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
		/// Gets or sets �Ƿ��ڱ���ǰ����ʾ��ͬ��ͼ��,�Ա�ʾ��ͬ��״̬,����"�˻�"��״̬���˻�ͼ���ʶ.
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
		/// Gets or sets a value indicating �Ƿ���ʾ��ϸ������״̬
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
		/// Gets or sets a value �Ƿ���ʾ�걨��λ
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
		/// Gets or sets a value �Ƿ���ʾУ������Ϣ:�ò�ͬ��ͼ����ʾ��ͬ��У����
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
		/// Gets or sets ���ɵ�div��css����
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
		/// Gets or sets a value indicating �Ƿ��б���ʾΪ���۵�������.
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
		/// Gets or sets a value �Ƿ��������б�����ʾÿ����������
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
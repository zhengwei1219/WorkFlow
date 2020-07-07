using System;
using System.Collections.Generic;
using System.Text;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 显示办理紧急程度的指示器:根据给定的处理时间,已经已经过去的时间,来显示某个件需要办理的紧急程度.
	/// </summary>
	public class ProgressRender
	{
		private int min;

		/// <summary>
		/// Gets or sets the min.
		/// </summary>
		/// <value>The min.</value>
		public int Min
		{
			get { return min; }
			set { min = value; }
		}

		private int max;

		/// <summary>
		/// Gets or sets the max.
		/// </summary>
		/// <value>The max.</value>
		public int Max
		{
			get { return max; }
			set { max = value; }
		}

		private int current;

		/// <summary>
		/// Gets or sets the current.
		/// </summary>
		/// <value>The current.</value>
		public int Current
		{
			get { return current; }
			set { current = value; }
		}

		private int warningPoint;

		/// <summary>
		/// Gets or sets 警告点的数值,小于这个数值时显示安全色
		/// </summary>
		/// <value>The warning point.</value>
		public int WarningPoint
		{
			get { return warningPoint; }
			set { warningPoint = value; }
		}

		private int urgentPoint;

		/// <summary>
		/// Gets or sets the 紧急点数值,小于这个点的数值显示为警告色,大于这个点的显示为coral颜色(粉红).
		/// </summary>
		/// <value>The urgent point.</value>
		public int UrgentPoint
		{
			get { return urgentPoint; }
			set { urgentPoint = value; }
		}

		#region  外观定义
		private int width;

		/// <summary>
		/// Gets or sets the width.
		/// </summary>
		/// <value>The width.</value>
		public int Width
		{
			get { return width; }
			set { width = value; }
		}

		private int height;

		/// <summary>
		/// Gets or sets the height.
		/// </summary>
		/// <value>The height.</value>
		public int Height
		{
			get { return height; }
			set { height = value; }
		}
		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="ProgressRender"/> class.
		/// </summary>
		/// <param name="min">The min.</param>
		/// <param name="max">The max.</param>
		/// <param name="current">The current.</param>
		/// <param name="warningPoint">The warning point.</param>
		/// <param name="urgentPoint">The urgent point.</param>
		/// <param name="width">The width.</param>
		/// <param name="height">The height.</param>
		public ProgressRender(int min, int max, int current, int warningPoint, int urgentPoint, int width, int height)
		{
			if (max <= min)
				throw new ArgumentException("max must greater than min");
			if (current < min)
				throw new ArgumentException("current must greater than min");
			if (warningPoint <= min || warningPoint > max)
				throw new ArgumentException("warningPoint must between max and min");
			if (urgentPoint < warningPoint || urgentPoint > max)
				throw new ArgumentException("urgentPoint must between max and warningPoint");
			this.max = max;
			this.min = min;
			this.current = current;
			this.warningPoint = warningPoint;
			this.urgentPoint = urgentPoint;
			this.width = width;
			this.height = height;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ProgressRender"/> class.
		/// </summary>
		/// <param name="max">The max.</param>
		/// <param name="current">The current.</param>
		/// <param name="warningPoint">The warning point.</param>
		public ProgressRender(int max, int current, int warningPoint)
			: this(0, max, current, warningPoint, max, 100, 5)
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ProgressRender"/> class.
		/// </summary>
		/// <param name="max">The max.</param>
		/// <param name="current">The current.</param>
		public ProgressRender(int max, int current)
			: this(0, max, current, (int)(max * 2 / 3), (int)(max * 5 / 6), 100, 5)
		{

		}
		/// <summary>
		/// Renders this instance.
		/// </summary>
		/// <returns></returns>
		public string Render()
		{
			StringBuilder html = new StringBuilder();
			string color = current <= warningPoint ? "green" : current <= urgentPoint ? "yellow" : current <= max ? "coral" : "red";
			html.AppendFormat("<div style='position:relative;height:{0};width:{1};border:1px solid {2};font-size:1px' class='progress' title='{3}/{4}'>", height, width, color, current - min, max - min);
			int innerHeight = height - 2;
			if (current <= warningPoint)
			{
				html.AppendFormat("<div style='position:absolute;left:0;top:0;height:{0};width:{1};background-color:{2}'></div>", innerHeight, (int)((current - min) * width / (max - min)), color);
			}
			else if (current <= urgentPoint)
			{
				int warningWidth = (int)((warningPoint - min) * width / (max - min));
				html.AppendFormat("<div style='position:absolute;left:0;top:0;height:{0};width:{1};background-color:{2}'></div>", innerHeight, warningWidth, color);
				html.AppendFormat("<div style='position:absolute;left:{0};top:0;height:{1};width:{2};background-color:{3}'></div>", warningWidth, innerHeight, (int)((current - warningPoint) * width / (max - min)), color);
			}
			else if (current <= max)
			{
				int warningWidth = (int)((warningPoint - min) * width / (max - min));
				int urgentWidth = (int)((urgentPoint - warningPoint) * width / (max - min));
				html.AppendFormat("<div style='position:absolute;left:0;top:0;height:{0};width:{1};background-color:{2}'></div>", innerHeight, warningWidth, color);
				html.AppendFormat("<div style='position:absolute;left:{0};top:0;height:{1};width:{2};background-color:{3}'></div>", warningWidth, innerHeight, urgentWidth, color);
				html.AppendFormat("<div style='position:absolute;left:{0};top:0;height:{1};width:{2};background-color:{3}'></div>", warningWidth + urgentWidth, innerHeight, (int)((current - urgentPoint) * width / (max - min)), color);
			}
			else
			{
				//超出整个期限
				int oldWidth = (int)((max - min) * width / (current - min));
				html.AppendFormat("<div style='position:absolute;left:0;top:0;height:{0};width:{1};background-color:yellow'></div>", innerHeight, oldWidth);
				html.AppendFormat("<div style='position:absolute;left:{0};top:0;height:{1};width:{2};background-color:{3}'></div>", oldWidth, innerHeight, (int)((current - max) * width / (current - min)), color);
			}
			html.Append("</div>");
			return html.ToString();
		}
	}
}

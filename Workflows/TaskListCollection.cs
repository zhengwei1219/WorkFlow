using System;
using System.Collections.Generic;
using System.Text;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 任务列表集合类,继承与List<TaskList>类
	/// </summary>
	public class TaskListCollection : List<TaskList>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TaskListCollection"/> class.
		/// </summary>
		public TaskListCollection()
			: base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TaskListCollection"/> class.
		/// </summary>
		/// <param name="list">The list.</param>
		public TaskListCollection(IEnumerable<TaskList> list)
			: base(list)
		{
		}

		/// <summary>
		///合并一个任务列表到当前集合中.如果加入的TaskList的任务类别已经存在,那么自动合并到以及该存在的任务类别中,否则新增加一个任务类别.
		/// </summary>
		/// <param name="one">The one.</param>
		public void Merge(TaskList one)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (this[i].Cagegory.Equals(one.Cagegory))
				{
					List<ITaskItem> items = new List<ITaskItem>(this[i].Items);
					items.AddRange(one.Items);
					this[i].Items = items.ToArray();
					return;
				}
			}
			this.Add(one);
		}

		/// <summary>
		/// 合并 一个任务列表结合到当前集合中
		/// </summary>
		/// <param name="list">The list.</param>
		public void MergeRange(IEnumerable<TaskList> list)
		{
			IEnumerator<TaskList> enumerator = list.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Merge(enumerator.Current);
			}
		}
	}
}

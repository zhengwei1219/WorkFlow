using System;
using System.Collections.Generic;
using System.Text;

namespace OilDigital.Workflows
{
	/// <summary>
	/// �����б�����,�̳���List<TaskList>��
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
		///�ϲ�һ�������б���ǰ������.��������TaskList����������Ѿ�����,��ô�Զ��ϲ����Լ��ô��ڵ����������,����������һ���������.
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
		/// �ϲ� һ�������б��ϵ���ǰ������
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

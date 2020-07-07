using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 代表审批动作执行权限的审批角色类
	/// </summary>
	[Serializable]
	public class ApprovalRole
	{
		/// <summary>
		/// 空构造函数
		/// </summary>
		public ApprovalRole()
		{
		}

		private string name;
		/// <summary>
		/// 角色名称
		/// </summary>
		[XmlAttribute()]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		private int grade;
		/// <summary>
		/// 角色的级别编号
		/// </summary>
		[XmlAttribute()]
		public int Grade
		{
			get { return grade; }
			set { grade = value; }
		}

		private int group;
		/// <summary>
		/// 用户角色分组编号
		/// </summary>
		[XmlAttribute()]
		public int Group
		{
			get { return group; }
			set { group = value; }
		}
	
	
		private string description;
		/// <summary>
		/// 角色附加信息
		/// </summary>
		[XmlAttribute()]
		public string Description
		{
			get { return description; }
			set { description = value; }
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
		/// <returns>
		/// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		/// <exception cref="T:System.NullReferenceException">
		/// The <paramref name="obj"/> parameter is null.
		/// </exception>
		public override bool Equals(object obj)
		{
			if (obj is ApprovalRole)
			{
				ApprovalRole role = obj as ApprovalRole;
				return role.Name == this.name && role.Group == this.group && role.Grade == this.grade;
			}
			return false;
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode()
		{
			return (this.name + this.group.ToString() + this.grade.ToString()).GetHashCode();
		}
	}
}

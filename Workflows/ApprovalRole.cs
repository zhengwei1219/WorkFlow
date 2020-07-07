using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace OilDigital.Workflows
{
	/// <summary>
	/// ������������ִ��Ȩ�޵�������ɫ��
	/// </summary>
	[Serializable]
	public class ApprovalRole
	{
		/// <summary>
		/// �չ��캯��
		/// </summary>
		public ApprovalRole()
		{
		}

		private string name;
		/// <summary>
		/// ��ɫ����
		/// </summary>
		[XmlAttribute()]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		private int grade;
		/// <summary>
		/// ��ɫ�ļ�����
		/// </summary>
		[XmlAttribute()]
		public int Grade
		{
			get { return grade; }
			set { grade = value; }
		}

		private int group;
		/// <summary>
		/// �û���ɫ������
		/// </summary>
		[XmlAttribute()]
		public int Group
		{
			get { return group; }
			set { group = value; }
		}
	
	
		private string description;
		/// <summary>
		/// ��ɫ������Ϣ
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

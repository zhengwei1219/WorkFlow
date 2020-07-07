using System;
using System.Data;
using System.Configuration;
using System.Web.Security;
using System.Collections.Generic;

namespace OilDigital.Workflows
{
	/// <summary>
	/// �ж��û��Ƿ�������ĳһ��ɫ
	/// </summary>
	public interface IUserInRole
	{
		/// <summary>
		/// �ж��û��Ƿ�����ĳ��ɫ
		/// </summary>
		/// <param name="userId">�û�Id</param>
		/// <param name="roleName">��ɫ����</param>
		/// <returns>
		/// 	<c>true</c> ���ڷ���true �����ڷ��� <c>false</c>.
		/// </returns>
		bool IsUserInRole(string userId, string roleName);
		/// <summary>
		/// ˢ��ĳ�û���Ӧ��ɫ����
		/// </summary>
		/// <param name="userId">�û�Id</param>
		void Flush(string userId);
		/// <summary>
		/// �������ʱ��
		/// </summary>
		/// <value>����ʱ��</value>
		int CacheDuration { set;}
		/// <summary>
		/// ��ȡĳ�û���������ɫ
		/// </summary>
		/// <param name="userId">�û�Id</param>
		/// <returns>�û����ڽ�ɫ�����飬�޽�ɫ���ؿ�����</returns>
		string[] GetUserRoles(string userId);
		/// <summary>
		/// ��ȡ����ĳ��ɫ�������û���Ϣ.
		/// </summary>
		/// <param name="roleName">��ɫ����</param>
		/// <returns>�����û�Id���飬���û����ؿ�����</returns>
		string[] GetUsersInRole(string roleName);
	}

	/// <summary>
	/// �������UserInRole
	/// </summary>
	public class HttpUserInRole : IUserInRole
	{
		private HttpUserInRole() { }
		private Dictionary<string, UserRole> rolesDic = new Dictionary<string, UserRole>();
		private static HttpUserInRole instance = new HttpUserInRole();
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static HttpUserInRole Instance
		{
			get { return instance; }
		}
		#region IUserInRole Members

		/// <summary>
		/// Determines whether [is user in role] [the specified user id].
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <param name="roleName">Name of the role.</param>
		/// <returns>
		/// 	<c>true</c> if [is user in role] [the specified user id]; otherwise, <c>false</c>.
		/// </returns>
		public bool IsUserInRole(string userId, string roleName)
		{
			string[] roles = GetUserRoles(userId);
			foreach (string role in roles)
			{
				if (role == roleName)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Flushes the specified user id.
		/// </summary>
		/// <param name="userId">The user id.</param>
		public void Flush(string userId)
		{
			LoadRoles(userId);
		}

		/// <summary>
		/// Gets the user roles.
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <returns></returns>
		public string[] GetUserRoles(string userId)
		{
			if (!rolesDic.ContainsKey(userId) || rolesDic[userId].TimeStamp.AddMinutes(DurationMinutes) < DateTime.Now)
				LoadRoles(userId);
			return rolesDic[userId].Roles;
		}

		/// <summary>
		/// Loads the roles.
		/// </summary>
		/// <param name="userId">The user id.</param>
		private void LoadRoles(string userId)
		{
			UserRole userRole = new UserRole();
			userRole.Roles = Roles.GetRolesForUser(userId);
			userRole.TimeStamp = DateTime.Now;
			rolesDic[userId] = userRole;
		}

		/// <summary>
		/// Gets the users in role.
		/// </summary>
		/// <param name="roleName">Name of the role.</param>
		/// <returns></returns>
		public string[] GetUsersInRole(string roleName)
		{
			return Roles.GetUsersInRole(roleName);
		}
		//Ĭ�ϻ���30min
		private int DurationMinutes = 30;
		/// <summary>
		/// Sets the duration of the cache.
		/// </summary>
		/// <value>The duration of the cache.</value>
		public int CacheDuration
		{
			set { this.DurationMinutes = value; }
		}
		#endregion

		/// <summary>
		/// 
		/// </summary>
		private class UserRole
		{
			/// <summary>
			/// 
			/// </summary>
			public DateTime TimeStamp;
			/// <summary>
			/// 
			/// </summary>
			public string[] Roles;
		}
	}
}

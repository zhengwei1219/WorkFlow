using System;
using System.Data;
using System.Configuration;
using System.Web.Security;
using System.Collections.Generic;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 判断用户是否在属于某一角色
	/// </summary>
	public interface IUserInRole
	{
		/// <summary>
		/// 判断用户是否属于某角色
		/// </summary>
		/// <param name="userId">用户Id</param>
		/// <param name="roleName">角色名称</param>
		/// <returns>
		/// 	<c>true</c> 属于返回true 不属于返回 <c>false</c>.
		/// </returns>
		bool IsUserInRole(string userId, string roleName);
		/// <summary>
		/// 刷新某用户对应角色缓冲
		/// </summary>
		/// <param name="userId">用户Id</param>
		void Flush(string userId);
		/// <summary>
		/// 缓存持续时间
		/// </summary>
		/// <value>缓存时间</value>
		int CacheDuration { set;}
		/// <summary>
		/// 获取某用户的所属角色
		/// </summary>
		/// <param name="userId">用户Id</param>
		/// <returns>用户所在角色的数组，无角色返回空数组</returns>
		string[] GetUserRoles(string userId);
		/// <summary>
		/// 获取属于某角色的所有用户信息.
		/// </summary>
		/// <param name="roleName">角色名称</param>
		/// <returns>返回用户Id数组，无用户返回空数组</returns>
		string[] GetUsersInRole(string roleName);
	}

	/// <summary>
	/// 带缓存的UserInRole
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
		//默认缓存30min
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

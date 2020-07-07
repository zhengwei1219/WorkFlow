using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Profile;
using System.Web;

namespace OilDigital.Workflows
{
	/// <summary>
	/// Http�û���Identity��Ϣ
	/// </summary>
	public class HttpUserIdentity : IUserIdentity
	{
		private string userId;
		private string userName;
		private string unitCode;
		private ProfileBase profileBase;

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpUserIdentity"/> class.
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <param name="userName">Name of the user.</param>
		/// <param name="userUnitCode">The user unit code.</param>
		/// <param name="profileBase">The profile base.</param>
		public HttpUserIdentity(string userId, string userName,string userUnitCode, ProfileBase profileBase)
		{
			this.userId = userId;
			this.userName = userName;
			this.profileBase = profileBase;
			this.unitCode = userUnitCode;
		}
		#region IUserIdentity Members

		/// <summary>
		/// �û�ID
		/// </summary>
		/// <returns></returns>
		public string GetUserId()
		{
			return userId;
		}

		/// <summary>
		/// �û�����
		/// </summary>
		/// <returns></returns>
		public string GetUserName()
		{
			return userName;
		}

		/// <summary>
		/// Gets the user unit code.
		/// </summary>
		/// <returns></returns>
		public string GetUserUnitCode()
		{
			return this.unitCode;
		}
		/// <summary>
		/// </summary>
		/// <param name="profileName"></param>
		/// <returns></returns>
		public object GetProfileValue(string profileName)
		{
			return profileBase.GetPropertyValue(profileName);
		}

		#endregion
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 用户身份信息
	/// </summary>
	public interface IUserIdentity
	{
		/// <summary>
		/// 用户ID
		/// </summary>
		string GetUserId();
		/// <summary>
		/// 用户姓名
		/// </summary>
		string GetUserName();
		/// <summary>
		/// Gets the user unit code.
		/// </summary>
		/// <returns></returns>
		string GetUserUnitCode();
		/// <summary>
		/// 
		/// </summary>
		/// <param name="profileName"></param>
		/// <returns></returns>
		object GetProfileValue(string profileName);
		
	}
}

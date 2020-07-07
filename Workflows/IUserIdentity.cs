using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace OilDigital.Workflows
{
	/// <summary>
	/// �û������Ϣ
	/// </summary>
	public interface IUserIdentity
	{
		/// <summary>
		/// �û�ID
		/// </summary>
		string GetUserId();
		/// <summary>
		/// �û�����
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

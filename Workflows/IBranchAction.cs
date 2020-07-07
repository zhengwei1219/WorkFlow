using System;
using System.Collections.Generic;
using System.Text;

namespace OilDigital.Workflows
{
	/// <summary>
	/// 分支动作接口
	/// </summary>
	public interface IBranchAction
	{

		/// <summary>
		/// 获取在动作执行完成以后,最终选定的状态
		/// </summary>
		/// <returns></returns>
		string GetSelectedState();
	}
}

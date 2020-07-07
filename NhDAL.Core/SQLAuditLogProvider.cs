using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Specialized;
using System.Configuration.Provider;
using OilDigital.Common.Log;

namespace OilDigital.Common.NHDAL.Core
{
	/// <summary>
	/// 基于直接sql的操作的数据审核日志提供者,在插入日志的时候使用Nhibernate中的事务和连接对象
	/// </summary>
	public class SQLAuditLogProvider : AuditLogProvider
	{
		private string connectionString;

		/// <summary>
		/// 是否enabled
		/// </summary>
		bool enabled;

		/// <summary>
		/// save log object,使用配置的数据库链接字符串来执行.
		/// </summary>
		/// <param name="log"><see cref="IWebAuditLog"/> instance </param>
		public override void Log(IWebAuditLog log)
		{
			IDbCommand command = new SqlCommand();

			command.Connection = NHibernateSessionManager.Instance.GetSession().Connection;

			NHibernateSessionManager.Instance.GetSession().Transaction.Enlist(command);
			command.CommandText = "usp_auditlog_insert";
			command.CommandType = CommandType.StoredProcedure;


			SqlParameter[] parms = new SqlParameter[10];
			parms[0] = new SqlParameter("@userid", log.UserID);
			parms[1] = new SqlParameter("@ip", log.IP);
			parms[2] = new SqlParameter("@actiontype", log.Action);
			parms[3] = new SqlParameter("@classname", log.ClassName);
			parms[4] = new SqlParameter("@propertyname", log.PropertyName);
			parms[5] = new SqlParameter("@originalvalue", log.OriginalValue);
			parms[6] = new SqlParameter("@currentvalue", log.CurrentVaue);
			parms[7] = new SqlParameter("@entityid", log.EntityId);
			parms[8] = new SqlParameter("@updatetime", log.UpdateTime);
			parms[9] = new SqlParameter("@threadid", log.ThreadId);
			//SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "usp_auditlog_insert", parms);
			for (int i = 0; i < parms.Length;i++ )
			{
				command.Parameters.Add(parms[i]);
			}
			command.ExecuteNonQuery();
		}

		/// <summary>
		/// 从配置文件初始化
		/// </summary>
		/// <param name="name"></param>
		/// <param name="config"></param>
		public override void Initialize(string name, NameValueCollection config)
		{
			// Verify that config isn't null
			if (config == null)
				throw new ArgumentNullException("config");

			// Assign the provider a default name if it doesn't have one
			if (String.IsNullOrEmpty(name))
				name = "sqlAuditLogProvider";

			// Add a default "description" attribute to config if the
			// attribute doesn't exist or is empty
			if (string.IsNullOrEmpty(config["description"]))
			{
				config.Remove("description");
				config.Add("description",
					"基于数据库的审核日志操作提供者");
			}
			base.Initialize(name, config);

			enabled = bool.Parse(ExtractConfigValue(config, "enabled", "true"));
			string connectionStringName = ExtractConfigValue(config, "connectionStringName", "LocalSqlServer");

			ConnectionStringSettings connectionStrings = ConfigurationManager.ConnectionStrings[connectionStringName];
			if (null == connectionStrings)
				throw new ProviderException("配置文件错误,没有找到名字为" + connectionStringName + "的数据库连接串配置");
			this.connectionString = connectionStrings.ConnectionString;
			// Throw an exception if unrecognized attributes remain
			if (config.Count > 0)
			{
				string attr = config.GetKey(0);
				if (!String.IsNullOrEmpty(attr))
					throw new System.Configuration.Provider.ProviderException("Unrecognized attribute: " +
					attr);
			}
		}

		/// <summary>
		/// A helper function to retrieve config values from the configuration file and remove the entry.
		/// </summary>
		/// <returns></returns>
		private string ExtractConfigValue(System.Collections.Specialized.NameValueCollection config, string key, string defaultValue)
		{
			string val = config[key];
			if (val == null)
				return defaultValue;
			else
			{
				config.Remove(key);

				return val;
			}
		}

		/// <summary>
		/// 获取某个类名下面的实体的所有相关修改记录
		/// </summary>
		/// <param name="className">Name of the class.</param>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		public override List<WebAuditLog> GetLogsByEntityId(string className, string id)
		{
			return GetLogs(className, id, "usp_auditlog_select");
		}

		/// <summary>
		/// Gets the logs by parent id.
		/// </summary>
		/// <param name="className">Name of the class.</param>
		/// <param name="parentId">The parent id.</param>
		/// <returns></returns>
		public override List<WebAuditLog> GetLogsByParentId(string className, string parentId)
		{
			return GetLogs(className, parentId, "usp_auditlog_select_member");
		}

		private List<WebAuditLog> GetLogs(string className, string id, string sqlProcedure)
		{

			SqlParameter[] parms = new SqlParameter[2];
			parms[0] = new SqlParameter("@classname", className);
			parms[1] = new SqlParameter("@entityid", id);
			SqlDataReader reader = SqlHelper.ExecuteReader(this.connectionString, CommandType.StoredProcedure, sqlProcedure, parms);
			List<WebAuditLog> logs = new List<WebAuditLog>();
			while (reader.Read())
			{
				WebAuditLog log = new WebAuditLog();
				log.Action = reader["actiontype"].ToString();
				log.ClassName = reader["classname"].ToString();
				log.IP = reader["ip"].ToString();
				log.PropertyName = reader["propertyName"].ToString();
				log.OriginalValue = reader["originalvalue"].ToString();
				log.CurrentVaue = reader["currentvalue"].ToString();
				log.EntityId = reader["entityId"].ToString();
				log.ThreadId = long.Parse(reader["ThreadId"].ToString());
				log.UserID = reader["userid"].ToString();
				log.UpdateTime = DateTime.Parse(reader["updatetime"].ToString());
				logs.Add(log);
			}

			return logs;
		}
	}
}


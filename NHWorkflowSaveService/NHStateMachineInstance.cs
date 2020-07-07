using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace OilDigital.Workflows.DAO
{
	public class NHStateMachineInstance
	{
		public NHStateMachineInstance()
		{
 
		}
		private Guid id;
		/// <summary>
		/// 实例Id
		/// </summary>
		public virtual Guid Id
		{
			get { return id; }
			set { id = value; }
		}

		private int eaId;

		/// <summary>
		/// Gets or sets the ea id.
		/// </summary>
		/// <value>The ea id.</value>
		public virtual int EaId
		{
			get { return eaId; }
			set { eaId = value; }
		}

		private string parentId;

		/// <summary>
		/// Gets or sets the parent id.
		/// </summary>
		/// <value>The parent id.</value>
		public virtual string ParentId
		{
			get { return parentId; }
			set { parentId = value; }
		}

		private string children;

		/// <summary>
		/// Gets or sets the children.
		/// </summary>
		/// <value>The children.</value>
		public virtual string Children
		{
			get { return children; }
			set { children = value; }
		}

		private string workflowName;

		/// <summary>
		/// Gets or sets the name of the workflow.
		/// </summary>
		/// <value>The name of the workflow.</value>
		public virtual string WorkflowName
		{
			get { return workflowName; }
			set { workflowName = value; }
		}

		private string typeName;
		/// <summary>
		/// 实例类型
		/// </summary>
		public virtual string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}

		private string stateName;
		/// <summary>
		/// 实例状态
		/// </summary>
		public virtual string StateName
		{
			get { return stateName; }
			set { stateName = value; }
		}

		private string createrUnit;

		/// <summary>
		/// Gets or sets the creater unit.
		/// </summary>
		/// <value>The creater unit.</value>
		public virtual string CreaterUnit
		{
			get { return createrUnit; }
			set { createrUnit = value; }
		}

		private string createrUserId;

		/// <summary>
		/// Gets or sets the creater user id.
		/// </summary>
		/// <value>The creater user id.</value>
		public virtual string CreaterUserId
		{
			get { return createrUserId; }
			set { createrUserId = value; }
		}


		private DateTime persistTime;

		/// <summary>
		/// Gets or sets the persist time.
		/// </summary>
		/// <value>The persist time.</value>
		public virtual DateTime PersistTime
		{
			get { return persistTime; }
			set { persistTime = value; }
		}


		private string executedActivitiesIds;

		/// <summary>
		/// Gets or sets the execute activities ids.
		/// </summary>
		/// <value>The execute activities ids.</value>
		public virtual string ExecuteActivitiesIds
		{
			get { return executedActivitiesIds; }
			set { executedActivitiesIds = value; }
		}

		private string stateRecord;

		/// <summary>
		/// Gets or sets the state record.
		/// </summary>
		/// <value>The state record.</value>
		public virtual string StateRecord
		{
			get { return stateRecord; }
			set { stateRecord = value; }
		}
	}
}

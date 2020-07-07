using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace OilDigital.Workflows
{
	/// <summary>
	/// ����ʵ���б������Լ���ȡ���ɫ��ʵ���б�
	/// </summary>
	public class InstanceWithRole
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="InstanceWithRole"/> class.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="role">The role.</param>
		/// <param name="isOwner">��ǰ��ʵ���Ƿ����û��Լ���ʵ��,���Ǳ���ָ��ʵ��</param>
		public InstanceWithRole(StateMachineWorkflowInstance instance, ApprovalRole role, bool isOwner)
		{
			this.role = role;
			this.instance = instance;
			this.isOwner = isOwner;
		}

		private bool isOwner;

		/// <summary>
		/// ��ǰ��ʵ���Ƿ����û��Լ���ʵ��,���Ǳ���ָ��ʵ��
		/// </summary>
		public bool IsOwner
		{
			get { return isOwner; }
			set { isOwner = value; }
		}


		private ApprovalRole role;

		/// <summary>
		/// Gets ��Ӧ��������ɫ
		/// </summary>
		/// <value>The states.</value>
		public ApprovalRole Role
		{
			get { return role; }
			set { role = value; }
		}

		private StateMachineWorkflowInstance instance;

		/// <summary>
		/// Gets ������ʵ������
		/// </summary>
		/// <value>The instance.</value>
		public StateMachineWorkflowInstance Instance
		{
			get { return instance; }
			set { instance = value; }
		}

		private TimeSpan exceedTime = TimeSpan.Zero;
		/// <summary>
		/// Gets �������ʱ��
		/// </summary>
		/// <value>The exceed time.</value>
		public TimeSpan ExceedTime
		{
			get
			{
				if (exceedTime == TimeSpan.Zero)
				{
					foreach (ApprovalEvent one in this.Instance.CurrentState.Events)
					{
						if (exceedTime != TimeSpan.Zero)
							break;
						if ((isOwner && one.Authorization == Authorization.DenyOwner.ToString())
						|| (!isOwner && one.Authorization == Authorization.OwnerOnly.ToString()))
							continue;
						foreach (EventRole eventRole in one.Roles)
						{
							if (eventRole.Name.Equals(role.Name, StringComparison.OrdinalIgnoreCase))
							{
								if (eventRole.ExceedTime != null && eventRole.ExceedTime != TimeSpan.Zero)
									exceedTime = eventRole.ExceedTime;
							}
						}
					}
					if (exceedTime == TimeSpan.Zero)
						exceedTime = TimeSpan.MaxValue;
				}
				return exceedTime;
			}
		}

		private string taskName = null;
		/// <summary>
		/// ��ʵ����Ӧ����������
		/// </summary>
		/// <value>The name of the task.</value>
		public virtual string TaskName
		{
			get
			{
				if (taskName == null)
				{
					foreach (ApprovalEvent one in this.Instance.CurrentState.Events)
					{
						if (taskName != null)
							break;
						if ((isOwner && one.Authorization == Authorization.DenyOwner.ToString())
						|| (!isOwner && one.Authorization == Authorization.OwnerOnly.ToString()))
							continue;
						foreach (EventRole eventRole in one.Roles)
						{
							if (eventRole.Name.Equals(role.Name, StringComparison.OrdinalIgnoreCase))
							{
								taskName = eventRole.TaskName;
								break;
							}
						}
					}
					if (taskName == null)
						taskName = string.Empty;
				}
				return taskName;
			}
			set { this.taskName = value; }
		}
	}



	/// <summary>
	/// ʵ���б���
	/// </summary>
	public class InstanceCollection : List<InstanceWithRole>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="InstanceCollection"/> class.
		/// </summary>
		public InstanceCollection() : base() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="InstanceCollection"/> class.
		/// </summary>
		/// <param name="list">The list.</param>
		public InstanceCollection(IEnumerable<InstanceWithRole> list) : base(list) { }
		/// <summary>
		/// Gets the name of the task.
		/// </summary>
		/// <value>The name of the task.</value>
		public string TaskName
		{
			get
			{
				if (this.Count > 0)
					return this[0].TaskName;
				return string.Empty;
			}
		}
		/// <summary>
		/// ����ɫ���ƺϲ����ϣ�����EaId���������ƽ��з���
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, List<InstanceCollection>> Split()
		{
			//����ɫ����
			Dictionary<string, InstanceCollection> dic = new Dictionary<string, InstanceCollection>();
			foreach (InstanceWithRole one in this)
			{
				if (dic.ContainsKey(one.Role.Name))
					dic[one.Role.Name].Add(one);
				else
				{
					InstanceCollection c = new InstanceCollection();
					c.Add(one);
					dic.Add(one.Role.Name, c);
				}
			}
			//��ÿ����ɫ�µķ������EaId���������Ʒ���
			Dictionary<string, List<InstanceCollection>> dic2 = new Dictionary<string, List<InstanceCollection>>();
			foreach (string key in dic.Keys)
			{
				dic2.Add(key, dic[key].SplitByEaId());
			}
			return dic2;
		}

		/// <summary>
		/// ��ȡ��ʵ�����ϰ��������н�ɫ����
		/// </summary>
		/// <returns></returns>
		public List<ApprovalRole> GetAllRoles()
		{
			//����ɫ����
			List<ApprovalRole> rtn = new List<ApprovalRole>();
			foreach (InstanceWithRole one in this)
			{
				if (!rtn.Contains(one.Role))
					rtn.Add(one.Role);
			}
			return rtn;
		}

		/// <summary>
		///����ɫ����ƥ���ʵ�����.
		/// </summary>
		/// <param name="role">The role.</param>
		/// <returns></returns>
		public InstanceCollection GetByRole(ApprovalRole role)
		{
			InstanceCollection rtn = new InstanceCollection();
			foreach (InstanceWithRole one in this)
			{
				if (one.Role.Equals(role))
					rtn.Add(one);
			}
			return rtn;
		}

		/// <summary>
		/// ����Ea id���������ƽ��з���ϲ�
		/// </summary>
		/// <returns></returns>
		public List<InstanceCollection> SplitByEaId()
		{
			//�Ȱ��������ƣ�EaId������������
			Dictionary<string, Dictionary<int, InstanceCollection>> dic2 = new Dictionary<string, Dictionary<int, InstanceCollection>>();
			foreach (InstanceWithRole one in this)
			{
				string taskName = one.TaskName;
				int eaid = one.Instance.EaId;
				//����������
				if (dic2.ContainsKey(taskName))
				{
					//��EaId�ļ������ʵ����������Ӽ�ֵ
					if (dic2[taskName].ContainsKey(eaid))
						dic2[taskName][eaid].Add(one);
					else
					{
						InstanceCollection o = new InstanceCollection();
						o.Add(one);
						dic2[taskName].Add(eaid, o);
					}
				}
				else
				{
					//��������ֵ������Ӽ�ֵ
					InstanceCollection o = new InstanceCollection();
					o.Add(one);
					Dictionary<int, InstanceCollection> oDic = new Dictionary<int, InstanceCollection>();
					oDic.Add(eaid, o);
					dic2.Add(taskName, oDic);
				}
			}
			//���ذ�EaId���������Ʒ����ʵ�������б�
			List<InstanceCollection> splited = new List<InstanceCollection>();
			foreach (string key1 in dic2.Keys)
			{
				foreach (int key2 in dic2[key1].Keys)
				{
					splited.Add(dic2[key1][key2]);
				}
			}
			return splited;
		}

		/// <summary>
		/// ��ȡ����ʵ�������ID
		/// </summary>
		/// <value>The instance I ds.</value>
		public Guid[] InstanceIDs
		{
			get
			{
				List<Guid> rtn = new List<Guid>();
				foreach (InstanceWithRole one in this)
				{
					if (!rtn.Contains(one.Instance.Id))
						rtn.Add(one.Instance.Id);
				}
				return rtn.ToArray();
			}
		}
	}

	/// <summary>
	/// ��ĳ����ɫ��Ӧ��ʵ��
	/// </summary>
	public class InstanceCollectionForRole
	{
		private ApprovalRole role;

		/// <summary>
		/// ����ʵ����Ӧ�Ľ�ɫ.
		/// </summary>
		/// <value>The role.</value>
		public ApprovalRole Role
		{
			get { return this.role; }
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="InstanceCollectionForRole"/> class.
		/// </summary>
		/// <param name="role">The role.</param>
		public InstanceCollectionForRole(ApprovalRole role)
		{
			this.role = role;
			this.instances = new InstanceCollection();
		}

		private InstanceCollection instances;

		/// <summary>
		/// Gets or sets ʵ����϶���
		/// </summary>
		/// <value>The instances.</value>
		public InstanceCollection Instances
		{
			get { return instances; }
			set { instances = value; }
		}

	}
}

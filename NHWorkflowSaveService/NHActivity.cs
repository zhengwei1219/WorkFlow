using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace OilDigital.Workflows.DAO
{
	/// <summary>
	/// ���ݿ��б����Activityʵ����.
	/// </summary>
	public class NHActivity
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NHActivity"/> class.
		/// </summary>
		public NHActivity()
		{
 
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NHActivity"/> class.
		/// </summary>
		/// <param name="activity">The activity.</param>
		public NHActivity(Activity activity)
		{
			ConvertFrom(activity);
		}
		private long id;
		/// <summary>
		/// ʵ��Id
		/// </summary>
		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}
		
		private string typeName;
		/// <summary>
		/// �����
		/// </summary>
		public virtual string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}

		private string persistInfo;
		/// <summary>
		/// ���л���Ϣ
		/// </summary>
		public virtual string PersistInfo
		{
			get { return persistInfo; }
			set { persistInfo = value; }
		}
		/// <summary>
		/// ��NHActivity�������л�ת��ΪApprovalActivity����
		/// </summary>
		/// <returns>ת����ApprovalActivity����</returns>
		public virtual Activity CovertTo()
		{
			Type type = Type.GetType(TypeName);
			if (type == null)
				throw new ApplicationException("can not load type " + typeName);
			XmlSerializer serializer = new XmlSerializer(type);
			string base64String = this.PersistInfo;
			byte[] arrBytes = Convert.FromBase64String(base64String);
			using (MemoryStream stream = new MemoryStream(arrBytes, 0, arrBytes.Length))
			{
				Activity activity = (Activity)serializer.Deserialize(stream);
				activity.Id = this.id;
				return activity;
			}
		}
		/// <summary>
		///ApprovalActivity�������л�ת��ΪNHActivity����
		/// </summary>
		/// <param name="activity">�����</param>
		public virtual void ConvertFrom(Activity activity)
		{
			this.Id = activity.Id;
			this.TypeName = activity.GetType().FullName + "," + activity.GetType().AssemblyQualifiedName.Split(',')[1];
			XmlSerializer serializer = new XmlSerializer(activity.GetType());
			using (MemoryStream memoryStream = new MemoryStream())
			{
				serializer.Serialize(memoryStream, activity);
				byte[] aryByte = memoryStream.ToArray();
				string base64String = Convert.ToBase64String(aryByte, 0, aryByte.Length);
				this.persistInfo = base64String;
			}
		}
	}
}

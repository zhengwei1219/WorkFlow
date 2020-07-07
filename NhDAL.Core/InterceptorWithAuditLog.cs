using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using System.Reflection;
using OilDigital.Common.Log;
using System.Collections;

namespace OilDigital.Common.NHDAL.Core
{
	/// <summary>
	/// ��¼�����޸���־
	/// </summary>
	public class InterceptorWithAuditLog:IInterceptor
	{
		private bool logAllPropertyWhenDeleting;

		#region ���캯��
		/// <summary>
		/// Initializes a new instance of the <see cref="InterceptorWithAuditLog"/> class.
		/// </summary>
		/// <param name="logAllPropertyWhenDeleting">if set to <c>true</c> ��ɾ�������ʱ���Ƿ���Ҫ��¼���е�����.</param>
		public InterceptorWithAuditLog(bool logAllPropertyWhenDeleting)
		{
			this.logAllPropertyWhenDeleting = logAllPropertyWhenDeleting;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InterceptorWithAuditLog"/> class
		/// </summary>
		public InterceptorWithAuditLog()
			: this(true)
		{
		}
		#endregion

		private void SaveLog(WebAuditLog log)
		{
			try
			{
				AuditLogService.Log(log);
			}
			catch (Exception ep)
			{
				throw ep;
			}
		}

		#region IInterceptor Members

		/// <summary>
		/// Called from <c>Flush()</c>. The return value determines whether the entity is updated
		/// </summary>
		/// <param name="entity">A persistent entity</param>
		/// <param name="id"></param>
		/// <param name="currentState"></param>
		/// <param name="previousState"></param>
		/// <param name="propertyNames"></param>
		/// <param name="types"></param>
		/// <returns>
		/// An array of dirty property indicies or <c>null</c> to choose default behavior
		/// </returns>
		/// <remarks>
		/// 	<list>
		/// 		<item>an array of property indicies - the entity is dirty</item>
		/// 		<item>an empty array - the entity is not dirty</item>
		/// 		<item><c>null</c> - use Hibernate's default dirty-checking algorithm</item>
		/// 	</list>
		/// </remarks>
		public int[] FindDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, NHibernate.Type.IType[] types)
		{
			return null;
		}

		public object Instantiate(Type type, object id)
		{
			return null;
		}

		public object IsUnsaved(object entity)
		{
			return null;
		}


		/// <summary>
		/// Called before an object is deleted
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="id"></param>
		/// <param name="state"></param>
		/// <param name="propertyNames"></param>
		/// <param name="types"></param>
		/// <remarks>
		/// It is not recommended that the interceptor modify the <c>state</c>.
		/// </remarks>
		public void OnDelete(object entity, object id, object[] state, string[] propertyNames, NHibernate.Type.IType[] types)
		{
			Type type = entity.GetType();
			AuditLogClassAttribute classAttribute = (AuditLogClassAttribute)Attribute.GetCustomAttribute(type, typeof(AuditLogClassAttribute));
			if (classAttribute == null) return ;

			//����Ҫ��¼ɾ������
			if (!classAttribute.LogDeleting) return;

			PropertyInfo idProperties = type.GetProperty(classAttribute.IdProperty);

			if (idProperties == null) throw new AuditLogException("��¼�޸���־����,û���ҵ����ڵ�Id,��ȷ�����Ͷ�������ȷ������AuditLogAttributeʵ��");

			//string idValue = idProperties.GetValue(entity, null).ToString();
			//long threadId = DateTime.Now.Ticks;
			//WebAuditLog log = new WebAuditLog();
			//log.ThreadId = threadId;
			//log.IP = System.Web.HttpContext.Current.Request.UserHostAddress;
			//log.Action = "ɾ��";
			//log.EntityId = idValue;
			//log.PropertyName = idProperties.Name;
			//log.UserID = System.Web.HttpContext.Current.User.Identity.Name;
			//log.ClassName = classAttribute.Name;
			//log.UpdateTime = DateTime.Now;
			//log.CurrentVaue = idValue;
			//log.OriginalValue = idValue;
			//AuditLogService.Log(log);	
			object idValue = idProperties.GetValue(entity, null);

			//��ȡ��Ҫ������־���ٵ�����,�����Ӧ��AuditLogAttributeֵ
			AuditLogAttribute[] attributes = null;
			PropertyInfo[] properties = GetAuditProperties(type.GetProperties(), out attributes);

			long threadId = DateTime.Now.Ticks;
			for (int i = 0; i < properties.Length; i++)
			{
				//
				PropertyInfo property = properties[i];
				AuditLogAttribute propertyAttribute = attributes[i];

				//��ȡ��������PropertyNames�����ж�Ӧ������
				int index = GetIndexOf(property, propertyNames);
				if (index < 0) continue;

				//���������ֵ��Ϊ��,�������Ϊʼ�ռ�¼,��ô��¼��������
				if (propertyAttribute.AlwaysLogging || (state[index] != null && propertyAttribute.LogDeleting))
				{
					WebAuditLog log = new WebAuditLog();
					log.ThreadId = threadId;
					log.IP = System.Web.HttpContext.Current.Request.UserHostAddress;
					log.Action = "ɾ��";
					log.EntityId = string.Format("{0}", id == null ? idValue : id);
					log.PropertyName = propertyAttribute.Name;
					log.UserID = System.Web.HttpContext.Current.User.Identity.Name;
					log.ClassName = classAttribute.Name;
					log.UpdateTime = DateTime.Now;
					string value = state[index] == null ? "null" : state[index].ToString();
					log.CurrentVaue = value;
					log.OriginalValue = value;
					SaveLog(log);
				}
			}
		}

		/// <summary>
		/// Called when an object is detected to be dirty, during a flush.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="id"></param>
		/// <param name="currentState"></param>
		/// <param name="previousState"></param>
		/// <param name="propertyNames"></param>
		/// <param name="types"></param>
		/// <returns>
		/// 	<c>true</c> if the user modified the <c>currentState</c> in any way
		/// </returns>
		/// <remarks>
		/// The interceptor may modify the detected <c>currentState</c>, which will be propagated to
		/// both the database and the persistent object. Note that all flushes end in an actual
		/// synchronization with the database, in which as the new <c>currentState</c> will be propagated
		/// to the object, but not necessarily (immediately) to the database. It is strongly recommended
		/// that the interceptor <b>not</b> modify the <c>previousState</c>.
		/// </remarks>
		public bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, NHibernate.Type.IType[] types)
		{
			Type type=entity.GetType();
			AuditLogClassAttribute classAttribute = (AuditLogClassAttribute)Attribute.GetCustomAttribute(type, typeof(AuditLogClassAttribute));
			if (classAttribute == null) return true;

			//�������Ҫ��¼�޸�,��ô����
			if (!classAttribute.LogChanging) return true;

			PropertyInfo idProperties=type.GetProperty(classAttribute.IdProperty);
			if (idProperties == null) throw new AuditLogException("��¼�޸���־����,û���ҵ����ڵ�Id,��ȷ�����Ͷ�������ȷ������AuditLogAttributeʵ��");

			object idValue = idProperties.GetValue(entity, null);

			AuditLogAttribute[] attributes = null;
			PropertyInfo[] properties = GetAuditProperties(type.GetProperties(), out attributes);

			long threadId = DateTime.Now.Ticks;

			//��һ��ѭ��,�����������,��������Ҫ�����֮�����������,��������ٵ�����ֵ�����仯,��ô��¼��,����¼�ܹ�����ĸ���
			int changed = 0;
			for (int i = 0; i < properties.Length; i++)
			{
				//
				PropertyInfo property = properties[i];
				AuditLogAttribute propertyAttribute = attributes[i];

				//��ȡ��������PropertyNames�����ж�Ӧ������
				int index = GetIndexOf(property, propertyNames);
				if (index < 0) continue;

				//���������ֵ�����仯,��¼�仯����,�����������ж��Ƿ��¼�����ݿ�
				if ((currentState[index] != null && previousState[index] == null) 
					|| (currentState[index] == null && previousState[index] != null) 
					|| ( currentState[index] != null && previousState[index] != null &&!currentState[index].Equals(previousState[index])))
				{
					changed++;

					//�����ʼ�ռ�¼������,��ô��ʱ����,��Ϊ��һ��ѭ����ֱ�ӱ���
					if (propertyAttribute.AlwaysLogging || propertyAttribute.LogChanging) continue;

					//��¼ֵ�����仯������
					WebAuditLog log = new WebAuditLog();
					log.ThreadId = threadId;
					log.IP = System.Web.HttpContext.Current.Request.UserHostAddress;
					log.Action = "�޸�";
					log.EntityId = idValue.ToString();
					log.PropertyName = propertyAttribute.Name;
					log.UserID = System.Web.HttpContext.Current.User.Identity.Name;
					log.ClassName = classAttribute.Name;
					log.UpdateTime = DateTime.Now;
					log.CurrentVaue = currentState[index] == null ? "null" : currentState[index].ToString();
					log.OriginalValue = previousState[index] == null ? "null" : previousState[index].ToString();
					SaveLog(log);
				}
			}

			//��������Ե����ݷ����仯,��ô������Щ���뱣�������
			if (changed > 0)
			{
				for (int i = 0; i < properties.Length; i++)
				{
					//
					PropertyInfo property = properties[i];
					AuditLogAttribute propertyAttribute = attributes[i];

					//��ȡ��������PropertyNames�����ж�Ӧ������
					int index = GetIndexOf(property, propertyNames);
					if (index < 0) continue;

					//���������ֵû�з����仯,
					if (propertyAttribute.AlwaysLogging || propertyAttribute.LogChanging)
					{
						WebAuditLog log = new WebAuditLog();
						log.ThreadId = threadId;
						log.IP = System.Web.HttpContext.Current.Request.UserHostAddress;
						log.Action = "�޸�";
						log.EntityId = idValue.ToString();
						log.PropertyName = propertyAttribute.Name;
						log.UserID = System.Web.HttpContext.Current.User.Identity.Name;
						log.ClassName = classAttribute.Name;
						log.UpdateTime = DateTime.Now;
						log.CurrentVaue = currentState[index] == null ? "null" : currentState[index].ToString();
						log.OriginalValue = previousState[index] == null ? "null" : previousState[index].ToString();
						SaveLog(log);
					}
				}
			}
			return false;
		}

		private int GetIndexOf(PropertyInfo property, string[] propertyNames)
		{
			for (int i = 0; i < propertyNames.Length; i++)
			{
				if (propertyNames[i] == property.Name) return i;
			}
			return -1;
		}

		/// <summary>
		/// Gets the audit properties.
		/// </summary>
		/// <param name="properties">The properties.</param>
		/// <param name="attributes">The attributes.</param>
		/// <returns></returns>
		private PropertyInfo[] GetAuditProperties(PropertyInfo[] properties,out AuditLogAttribute[] attributes)
		{
			List<PropertyInfo> result = new List<PropertyInfo>();
			attributes = null;

			List<AuditLogAttribute> attributeList = new List<AuditLogAttribute>();
			foreach (PropertyInfo property in properties)
			{
				AuditLogAttribute attribute = (AuditLogAttribute)Attribute.GetCustomAttribute(property, typeof(AuditLogAttribute));
				if (attribute != null)
				{
					result.Add(property);
					attributeList.Add(attribute);
				}

			}
			attributes = attributeList.ToArray();
			return result.ToArray();
		}


		/// <summary>
		/// Called just before an object is initialized
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="id"></param>
		/// <param name="state"></param>
		/// <param name="propertyNames"></param>
		/// <param name="types"></param>
		/// <returns>
		/// 	<c>true</c> if the user modified the <c>state</c> in any way
		/// </returns>
		/// <remarks>
		/// The interceptor may change the <c>state</c>, which will be propagated to the persistent
		/// object. Note that when this method is called, <c>entity</c> will be an empty
		/// uninitialized instance of the class.</remarks>
		public bool OnLoad(object entity, object id, object[] state, string[] propertyNames, NHibernate.Type.IType[] types)
		{
			return false;
		}

		/// <summary>
		/// Called before a new object is saved
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="id"></param>
		/// <param name="state"></param>
		/// <param name="propertyNames"></param>
		/// <param name="types"></param>
		/// <returns>
		/// 	<c>true</c> if the user modified the <c>state</c> in any way
		/// </returns>
		/// <remarks>
		/// The interceptor may modify the <c>state</c>, which will be used for the SQL <c>INSERT</c>
		/// and propagated to the persistent object
		/// </remarks>
		public bool OnSave(object entity, object id, object[] state, string[] propertyNames, NHibernate.Type.IType[] types)
		{
			//��ȡ���е�AuditLogAttrivute
			Type type = entity.GetType();
			AuditLogClassAttribute classAttribute = (AuditLogClassAttribute)Attribute.GetCustomAttribute(type, typeof(AuditLogClassAttribute));
			if (classAttribute == null) return true;

			//����Ҫ��¼���붯��
			if (!classAttribute.LogInserting) return false;

			PropertyInfo idProperties = type.GetProperty(classAttribute.IdProperty);
			if (idProperties == null) throw new AuditLogException("��¼�޸���־����,û���ҵ����ڵ�Id,��ȷ�����Ͷ�������ȷ������AuditLogAttributeʵ��");

			object idValue = idProperties.GetValue(entity, null);

			//��ȡ��Ҫ������־���ٵ�����,�����Ӧ��AuditLogAttributeֵ
			AuditLogAttribute[] attributes = null;
			PropertyInfo[] properties = GetAuditProperties(type.GetProperties(),out attributes);

			long threadId = DateTime.Now.Ticks;
			for(int i=0;i<properties.Length;i++)
			{
				//
				PropertyInfo property = properties[i];
				AuditLogAttribute propertyAttribute = attributes[i];

				//��ȡ��������PropertyNames�����ж�Ӧ������
				int index = GetIndexOf(property, propertyNames);
				if (index < 0) continue;

				//���������ֵ��Ϊ��,�������Ϊʼ�ռ�¼,��ô��¼��������
				if (propertyAttribute.AlwaysLogging	|| (state[index] != null && propertyAttribute.LogInserting ))
				{
					WebAuditLog log = new WebAuditLog();
					log.ThreadId = threadId;
					log.IP = System.Web.HttpContext.Current.Request.UserHostAddress;
					log.Action = "����";
					log.EntityId = string.Format("{0}",id==null?idValue:id);
					log.PropertyName = propertyAttribute.Name;
					log.UserID = System.Web.HttpContext.Current.User.Identity.Name;
					log.ClassName = classAttribute.Name;
					log.UpdateTime = DateTime.Now;
					string value=state[index] == null ? "null" : state[index].ToString();
					log.CurrentVaue = value;
					log.OriginalValue = value;
					SaveLog(log);
				}
			}
			return false;
		}

		public void PostFlush(System.Collections.ICollection entities)
		{
			return;
		}

		public void PreFlush(System.Collections.ICollection entities)
		{
			return;
		}

		#endregion
	}
}

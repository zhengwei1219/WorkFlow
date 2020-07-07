using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Expression;

namespace OilDigital.Common.NHDAL.Core
{
    public abstract class GenericNHibernateDao<T, ID> : IGenericDao<T, ID>
    {
        /// <summary>
        /// Exposes the ISession used within the DAO.
        /// </summary>
        protected ISession session {
            get {
                return NHibernateSessionManager.Instance.GetSession();
            }
        }

        /// <summary>
        /// Loads an instance of type T from the DB based on its ID.
        /// </summary>
        public T GetById(ID id, bool shouldLock) {
            T entity;

            if (shouldLock) {
                entity = (T)session.Load(persitentType, id, LockMode.Upgrade);
            }
            else {
                entity = (T)session.Load(persitentType, id);
            }

            return entity;
        }

        /// <summary>
        /// Loads every instance of the requested type with no filtering.
        /// </summary>
        public virtual List<T> GetAll() {
            return GetByCriteria();
        }

        /// <summary>
        /// Loads every instance of the requested type using the supplied <see cref="ICriterion" />.
        /// If no <see cref="ICriterion" /> is supplied, this behaves like <see cref="GetAll" />.
        /// </summary>
        public List<T> GetByCriteria(params ICriterion[] criterion) {
            ICriteria criteria = session.CreateCriteria(persitentType);

            foreach (ICriterion criterium in criterion) {
                criteria.Add(criterium);
            }

            return ConvertToGenericList(criteria.List());
        }

		public List<T> GetByQuery(string queryString)
		{
			IQuery query = session.CreateQuery(queryString);
			return ConvertToGenericList(query.List());
		}

        public List<T> GetByExample(T exampleInstance, params string[] propertiesToExclude) {
            ICriteria criteria = session.CreateCriteria(persitentType);
            Example example = Example.Create(exampleInstance);

            foreach (string propertyToExclude in propertiesToExclude) {
                example.ExcludeProperty(propertyToExclude);
            }

            criteria.Add(example);

            return ConvertToGenericList(criteria.List());
        }

        /// <summary>
        /// Looks for a single instance using the example provided.
        /// </summary>
        /// <exception cref="NonUniqueResultException" />
        public T GetUniqueByExample(T exampleInstance, params string[] propertiesToExclude) {
            List<T> foundList = GetByExample(exampleInstance, propertiesToExclude);

            if (foundList.Count > 1) {
                throw new NonUniqueResultException(foundList.Count);
            }

            if (foundList.Count > 0) {
                return foundList[0];
            }
            else {
                return default(T);
            }
        }

        /// <summary>
        /// For entities that have assigned ID's, you must explicitly call Save to add a new one.
        /// See http://www.hibernate.org/hib_docs/reference/en/html/mapping.html#mapping-declaration-id-assigned.
        /// </summary>
        public T Save(T entity) {
            session.Save(entity);
            return entity;
        }

        /// <summary>
        /// For entities with automatatically generated IDs, such as identity, SaveOrUpdate may 
        /// be called when saving a new entity.  SaveOrUpdate can also be called to update any 
        /// entity, even if its ID is assigned.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public T SaveOrUpdate(T entity) {
            session.SaveOrUpdate(entity);
            return entity;
        }

        public void Delete(T entity) {
            session.Delete(entity);
        }

        public void DeletebyId(ID id)
        {
			session.Delete("from " + persitentType.Name + " o where o.id = " + id.ToString()); 
        }
        public void CommitChanges() {
            NHibernateSessionManager.Instance.CommitTransaction();
        }

        /// <summary>
        /// Accepts a list of objects and converts them to a strongly typed list.
        /// </summary>
        /// <remarks>This should be put into a utility class as it's not directly
        /// related to data access.</remarks>
        public List<T> ConvertToGenericList(System.Collections.IList listObjects) {
            List<T> convertedList = new List<T>();

            foreach (object listObject in listObjects) {
                convertedList.Add((T)listObject);
            }

            return convertedList;
        }

		virtual protected Type persitentType
		{
			get { return typeof(T); }
		}
    }
}

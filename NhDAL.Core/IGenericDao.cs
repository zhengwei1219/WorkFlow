using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Expression;

namespace OilDigital.Common.NHDAL.Core
{
    public interface IGenericDao<T, ID>
    {
        T GetById(ID id, bool shouldLock);
        List<T> GetAll();
        List<T> GetByExample(T exampleInstance, params string[] propertiesToExclude);
        T GetUniqueByExample(T exampleInstance, params string[] propertiesToExclude);
        T Save(T entity);
        T SaveOrUpdate(T entity);
        void Delete(T entity);
		List<T> GetByQuery(string queryString);
        List<T> GetByCriteria(params ICriterion[] criterion);
        /// <summary>
        /// Commits all of the changes thus far to the storage system
        /// </summary>
        void CommitChanges();
        void DeletebyId(ID id);
    }
}

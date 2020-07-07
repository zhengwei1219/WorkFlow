using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Text;
using NHibernate;
using NHibernate.Cache;
using System.Web;
using System.Data;
using System.Configuration;

namespace OilDigital.Common.NHDAL.Core
{
    /// <summary>
    /// Handles creation and management of sessions and transactions.  It is a singleton because 
    /// building the initial session factory is very expensive. Inspiration for this class came 
    /// from Chapter 8 of Hibernate in Action by Bauer and King.  Although it is a sealed singleton
    /// you can use TypeMock (http://www.typemock.com) for more flexible testing.
    /// </summary>
    public sealed class NHibernateSessionManager
    {
        #region Thread-safe, lazy Singleton

        /// <summary>
        /// This is a thread-safe, lazy singleton.  See http://www.yoda.arachsys.com/csharp/singleton.html
        /// for more details about its implementation.
        /// </summary>
        public static NHibernateSessionManager Instance {
            get {
                return Nested.nHibernateSessionManager;
            }
        }

        /// <summary>
        /// Initializes the NHibernate session factory upon instantiation.
        /// </summary>
        private NHibernateSessionManager() {
            InitSessionFactory();
        }

        /// <summary>
        /// Assists with ensuring thread-safe, lazy singleton
        /// </summary>
        private class Nested
        {
            static Nested() { }
            internal static readonly NHibernateSessionManager nHibernateSessionManager = new NHibernateSessionManager();
        }

        #endregion

        private void InitSessionFactory() {
            NHibernate.Cfg.Configuration cfg = new NHibernate.Cfg.Configuration();

            // The following makes sure the the web.config contains a declaration for the HBM_ASSEMBLY appSetting
			string filesString = System.Configuration.ConfigurationManager.AppSettings["HBM_ASSEMBLY"];
            if (filesString == null || filesString == "") {
                throw new ConfigurationErrorsException("NHibernateManager.InitSessionFactory: \"HBM_ASSEMBLY\" must be " +
                    "provided as an appSetting within your config file. \"HBM_ASSEMBLY\" informs NHibernate which assembly " +
                    "contains the HBM files. It is assumed that the HBM files are embedded resources. An example config " +
                    "declaration is <add key=\"HBM_ASSEMBLY\" value=\"MyProject.Core\" />");
            }


			string[] files = filesString.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			foreach (string one in files)
			{
				cfg.AddAssembly(one);
			}
            sessionFactory = cfg.BuildSessionFactory();
        }

        /// <summary>
        /// Allows you to register an interceptor on a new session.  This may not be called if there is already
        /// an open session attached to the HttpContext.  If you have an interceptor to be used, modify
        /// the HttpModule to call this before calling BeginTransaction().
        /// </summary>
        public void RegisterInterceptor(IInterceptor interceptor) {
            ISession session = threadSession;

            if (session != null && session.IsOpen) {
                throw new CacheException("You cannot register an interceptor once a session has already been opened");
            }

            GetSession(interceptor);
        }

        public ISession GetSession() {
            return GetSession(null);
        }

        /// <summary>
        /// Gets a session with or without an interceptor.  This method is not called directly; instead,
        /// it gets invoked from other public methods.
        /// </summary>
        private ISession GetSession(IInterceptor interceptor) {
            ISession session = threadSession;

            if (session == null) {
                if (interceptor != null) {
                    session = sessionFactory.OpenSession(interceptor);
					session.FlushMode = FlushMode.Commit;
                }
                else {
                    session = sessionFactory.OpenSession();
					session.FlushMode = FlushMode.Commit;
                }
                threadSession = session;
            }

            return session;
        }

        public void CloseSession() {
            ISession session = threadSession;
            threadSession = null;

            if (session != null && session.IsOpen) {
                session.Close();
            }
        }

        public void BeginTransaction() {
            ITransaction transaction = threadTransaction;

            if (transaction == null) {
                transaction = GetSession().BeginTransaction();
                threadTransaction = transaction;
            }
        }

        public void CommitTransaction() {
            ITransaction transaction = threadTransaction;

            try {
                if (transaction != null && !transaction.WasCommitted && !transaction.WasRolledBack) {
                    transaction.Commit();
                    threadTransaction = null;
                }
            }
            catch (HibernateException ex) {
                RollbackTransaction();
                throw ex;
            }
        }

        public void RollbackTransaction() {
            ITransaction transaction = threadTransaction;

            try {
                threadTransaction = null;

                if (transaction != null && !transaction.WasCommitted && !transaction.WasRolledBack) {
                    transaction.Rollback();
                }
            }
            catch (HibernateException ex) {
                throw ex;
            }
            finally {
                CloseSession();
            }
        }

        private ITransaction threadTransaction {
            get {
                return (ITransaction)CallContext.GetData("THREAD_TRANSACTION");
            }
            set {
                CallContext.SetData("THREAD_TRANSACTION", value);
            }
        }

        private ISession threadSession {
            get {
                return (ISession)CallContext.GetData("THREAD_SESSION");
            }
            set {
                CallContext.SetData("THREAD_SESSION", value);
            }
        }

        private ISessionFactory sessionFactory;
    }
}

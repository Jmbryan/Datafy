using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Datafy.Core;

namespace Datafy.App
{
    public class SQLiteDbProvider<TContext> : IDbProvider
        where TContext : SQLiteDbContext
    {
        public TContext Context => ContextFactory?.Create(m_options);
        private IDbContextFactory<TContext> ContextFactory { get; }

        private SqliteConnection m_connection = null;
        private SQLiteDbContextOptions<TContext> m_options = null;

        public SQLiteDbProvider(IDbContextFactory<TContext> factory)
        {
            ContextFactory = factory;
        }

        public void Connect()
        {
            m_connection = new SqliteConnection("DataSource=:memory:");
            m_connection.Open();

            m_options = new SQLiteDbContextOptions<TContext>(m_connection);

            using var db = Context;
            db.Database.EnsureCreated();
        }

        public void Disconnect()
        {
            m_options = null;
            m_connection?.Close();
        }

        public void OnTypesAdded(IEnumerable<IType> types)
        {
            using var db = Context;
            foreach (var type in types)
            {
                db.Types.Add(new DbType(type));
            }
            db.SaveChanges();
        }

        public void OnTypesUpdated(IEnumerable<IType> types)
        {
            using var db = Context;
            foreach (var type in types)
            {
                var dbType = db.Types.Find((long)type.TypeId.Value);
                if (dbType != null)
                {
                    dbType.Set(type);
                    db.Types.Update(dbType);
                }
            }
            db.SaveChanges();
        }

        public void OnTypesRemoved(IEnumerable<IType> types)
        {
            using var db = Context;
            foreach (var type in types)
            {
                var dbType = db.Types.Find((long)type.TypeId.Value);
                if (dbType != null)
                {
                    db.Types.Remove(dbType);
                }
            }
            db.SaveChanges();
        }

        public void OnObjectsAdded(IEnumerable<IObject> objs)
        {
            // TODO
        }

        public void OnObjectsUpdated(IEnumerable<IObject> objs)
        {
            // TODO
        }

        public void OnObjectsRemoved(IEnumerable<IObject> objs)
        {
            // TODO
        }

        #region IDisposable
        private bool m_disposing = false;
        public void Dispose(bool disposing)
        {
            if (!m_disposing)
            {
                m_disposing = true;
                m_connection?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Datafy.Core;

namespace Datafy.App
{
    public class SQLiteDbContextOptions<TContext> : IDbContextOptions
        where TContext : SQLiteDbContext
    {
        public DbContextOptions<TContext> Options { get; }

        public SQLiteDbContextOptions(SqliteConnection connection)
        {
            Options = new DbContextOptionsBuilder<TContext>()
                .UseSqlite(connection) // Set the connection explicitly, so it won't be closed automatically by EF
                .Options;
        }
    }
}

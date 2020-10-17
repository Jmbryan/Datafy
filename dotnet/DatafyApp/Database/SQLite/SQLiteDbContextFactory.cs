using System;
using Microsoft.EntityFrameworkCore;
using Datafy.Core;

namespace Datafy.App
{
    public class SQLiteDbContextFactory : IDbContextFactory<SQLiteDbContext>
    {
        public SQLiteDbContext Create(IDbContextOptions options)
        {
            if (options is SQLiteDbContextOptions<SQLiteDbContext> sqliteOptions)
            {
                return new SQLiteDbContext(sqliteOptions);
            }
            throw new ArgumentException("Expected SQLiteDbContextOptions");
        }
    }
}

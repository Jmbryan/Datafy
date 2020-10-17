using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Datafy.Core;

namespace Datafy.App
{
    public class DbField
    {
        [Key]
        string Name { get; }
        [Required]
        ValueType ValueType { get; }
    }

    public class DbType
    {
        [Key]
        public long TypeId { get; set; }
        [Required]
        public string Name { get; set; }

        public DbType() { }
        public DbType(IType type)
        {
            Set(type);
        }

        public void Set(IType type)
        {
            TypeId = (long)type.TypeId.Value;
            Name = type.Name;
        }
    }

    public class SQLiteDbContext : DbContext, IDbContext
    {
        public DbSet<DbType> Types { get; set; }

        public SQLiteDbContext(SQLiteDbContextOptions<SQLiteDbContext> options) : base(options?.Options) { }
    }
}

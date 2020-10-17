
namespace Datafy.Core
{
    public interface IDbContextFactory<TContext>
        where TContext : IDbContext
    {
        TContext Create(IDbContextOptions options);
    }
}

namespace Autopart.Domain.SharedKernel
{
    public interface IRepository<TEntity> where TEntity : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }
    }
}

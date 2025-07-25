using HealthcarePortal.Core.Entities;

namespace HealthcarePortal.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<Member> Members { get; }
    IRepository<Provider> Providers { get; }
    IRepository<Claim> Claims { get; }
    IRepository<Authorization> Authorizations { get; }
    IRepository<CareJourneyPlan> CareJourneyPlans { get; }
    IRepository<CareGoal> CareGoals { get; }
    IRepository<CareTask> CareTasks { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
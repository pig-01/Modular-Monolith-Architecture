using System.Data;
using Base.Infrastructure.Interface.TimeZone;
using Base.Infrastructure.Toolkits.Converter.EntityFramework;
using DataHub.Domain.SeedWork;
using DataHub.Infrastructure.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace DataHub.Infrastructure.Contexts;

public partial class DemoContext(
    ILogger<DemoContext> logger,
    DbContextOptions<DemoContext> options,
    ITimeZoneService timeZoneService,
    IMediator mediator) : DbContext(options)
{
    private readonly ITimeZoneService timeZoneService = timeZoneService;
    private readonly IMediator mediator = mediator;

    public IDbContextTransaction GetCurrentTransaction() => Database.CurrentTransaction ?? throw new InvalidOperationException("Transaction is not started");

    public bool HasActiveTransaction => Database.CurrentTransaction is not null;

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        if (HasActiveTransaction) throw new InvalidOperationException("Transaction is already started");

        return await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
    }


    public async Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(transaction);
        if (transaction != Database.CurrentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

        try
        {
            logger.LogInformation("Committing transaction {TransactionId}", transaction.TransactionId);
            _ = await SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error committing transaction {TransactionId}", transaction.TransactionId);
            RollbackTransaction();
            throw;
        }
        finally
        {
            Database.CurrentTransaction?.Dispose();
        }

        // Dispatch Domain Events collection. 
        await mediator.DispatchDomainEventsAsync(this);
    }

    public void RollbackTransaction()
    {
        try
        {
            Database.CurrentTransaction?.Rollback();
        }
        finally
        {
            Database.CurrentTransaction?.Dispose();
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (EntityEntry<Entity> entry in ChangeTracker.Entries<Entity>())
        {
            if (entry.State is EntityState.Added)
                entry.Entity.CreatedDate = timeZoneService.ConvertToUserTimeZone(DateTime.UtcNow);

            if (entry.State is EntityState.Added or EntityState.Modified)
                entry.Entity.ModifiedDate = timeZoneService.ConvertToUserTimeZone(DateTime.UtcNow);
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Chinese_Taiwan_Stroke_BIN");

        // 全局應用時區轉換器到所有DateTime屬性
        foreach (Microsoft.EntityFrameworkCore.Metadata.IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (Microsoft.EntityFrameworkCore.Metadata.IMutableProperty property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(new DateTimeValueConverter(timeZoneService));
                }
            }
        }

        OnModelCreatingPartialByCustomer(modelBuilder);
        OnModelCreatingPartialByOrder(modelBuilder);
        OnModelCreatingPartialByUser(modelBuilder);
        OnModelCreatingPartialByTenant(modelBuilder);
        OnModelCreatingPartialByMail(modelBuilder);
        OnModelCreatingPartialByFunction(modelBuilder);
    }

    static partial void OnModelCreatingPartialByCustomer(ModelBuilder modelBuilder);
    static partial void OnModelCreatingPartialByOrder(ModelBuilder modelBuilder);
    static partial void OnModelCreatingPartialByUser(ModelBuilder modelBuilder);
    static partial void OnModelCreatingPartialByTenant(ModelBuilder modelBuilder);
    static partial void OnModelCreatingPartialByMail(ModelBuilder modelBuilder);
    static partial void OnModelCreatingPartialByFunction(ModelBuilder modelBuilder);
}

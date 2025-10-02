using System.Data;
using Base.Infrastructure.Toolkits.Converter.EntityFramework;
using Scheduler.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;


namespace Scheduler.Infrastructure.Context;

public partial class DemoContext(
    ILogger<DemoContext> logger,
    DbContextOptions<DemoContext> options,
    ITimeZoneService timeZoneService,
    IMediator mediator) : DbContext(options)
{
    private readonly ITimeZoneService timeZoneService = timeZoneService;
    private readonly IMediator mediator = mediator;

    private IDbContextTransaction? currentTransaction;

    public IDbContextTransaction GetCurrentTransaction() => currentTransaction ?? throw new InvalidOperationException("Transaction is not started");

    public bool HasActiveTransaction => currentTransaction != null;

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        if (currentTransaction is not null) throw new InvalidOperationException("Transaction is already started");

        currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        return currentTransaction;
    }


    public async Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(transaction);
        if (transaction != currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

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
            if (currentTransaction != null)
            {
                currentTransaction.Dispose();
                currentTransaction = null;
            }
        }

        // Dispatch Domain Events collection.
        await mediator.DispatchDomainEventsAsync(this);
    }

    public void RollbackTransaction()
    {
        try
        {
            currentTransaction?.Rollback();
        }
        finally
        {
            if (currentTransaction != null)
            {
                currentTransaction.Dispose();
                currentTransaction = null;
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.UseCollation("Chinese_Taiwan_Stroke_BIN");

        OnMailModelCreatingPartial(modelBuilder);
        OnPlanModelCreatingPartial(modelBuilder);
        OnQuartzModelCreatingPartial(modelBuilder);

        // 全局應用時區轉換器到所有DateTime屬性
        foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (IMutableProperty property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(new DateTimeValueConverter(timeZoneService));
                }
            }
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (EntityEntry<Entity> entry in ChangeTracker.Entries<Entity>())
        {
            if (entry.State is EntityState.Added)
            {
                entry.Entity.CreatedDate = timeZoneService.ConvertToUserTimeZone(DateTime.UtcNow);
            }

            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                entry.Entity.ModifiedDate = timeZoneService.ConvertToUserTimeZone(DateTime.UtcNow);
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}

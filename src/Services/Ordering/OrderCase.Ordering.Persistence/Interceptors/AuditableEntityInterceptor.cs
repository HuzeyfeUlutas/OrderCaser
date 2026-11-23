using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OrderCase.Ordering.Domain.Common;
using OrderCase.Ordering.Persistence.Abstractions;

namespace OrderCase.Ordering.Persistence.Interceptors;

public sealed class AuditableEntityInterceptor(IAuditUserProvider auditUserProvider) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ApplyAuditing(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplyAuditing(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplyAuditing(DbContext? context)
    {
        if (context is null)
            return;

        var now = DateTime.UtcNow;
        var userId = auditUserProvider.UserId;

        foreach (var entry in context.ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    SetValue(entry, nameof(AuditableEntity.CreatedAt), now);
                    SetValue(entry, nameof(AuditableEntity.CreatedBy), userId);
                    break;

                case EntityState.Modified:
                    SetValue(entry, nameof(AuditableEntity.UpdatedAt), now);
                    SetValue(entry, nameof(AuditableEntity.UpdatedBy), userId);
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    SetValue(entry, nameof(AuditableEntity.IsDeleted), true);
                    SetValue(entry, nameof(AuditableEntity.DeletedAt), now);
                    SetValue(entry, nameof(AuditableEntity.DeletedBy), userId);
                    break;
            }
        }
    }

    private static void SetValue(EntityEntry entry, string propertyName, object? value) =>
        entry.Property(propertyName).CurrentValue = value;
}
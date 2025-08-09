using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using XYZ.WShop.Domain.Interfaces;
using XYZ.WShop.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using XYZ.WShop.Domain.Enums;

namespace XZY.WShop.Infrastructure.Data.Interceptors
{
    /// <summary>
    /// Interceptor for auditing changes made to the database during save operations.
    /// </summary>
    public class AuditingSaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditingSaveChangesInterceptor"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        public AuditingSaveChangesInterceptor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Overrides the SavedChangesAsync method to perform auditing before saving changes to the database.
        /// </summary>
        /// <param name="eventData">The save changes event data.</param>
        /// <param name="result">The result of the save changes operation.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous save operation.</returns>
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            var userId = _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var dbContext = eventData.Context;
            var auditEntries = new List<AuditEntry>();
            foreach (var entry in dbContext.ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted))
            {
                if (entry.Entity is IAuditableEntity auditable)
                {
                    if (entry.State == EntityState.Added)
                    {
                        auditable.CreatedDate = DateTime.UtcNow;
                        if (userId != null)
                        {
                            auditable.CreatedBy = Guid.Parse(userId);

                        }
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        auditable.ModifiedDate = DateTime.UtcNow;
                        auditable.ModifiedBy = userId != null ? Guid.Parse(userId) : Guid.Empty;
                    }
                }

                var auditEntry = new AuditEntry(entry)
                {
                    TableName = entry.Entity.GetType().Name,
                    UserId = userId
                };

                foreach (var property in entry.Properties)
                {
                    string propertyName = property.Metadata.Name;

                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.AuditType = AuditType.Create;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            break;
                        case EntityState.Deleted:
                            auditEntry.AuditType = AuditType.Delete;
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            break;
                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditEntry.ChangedColumns.Add(propertyName);
                                auditEntry.AuditType = AuditType.Update;
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                            }
                            break;
                    }
                }

                auditEntries.Add(auditEntry);
            }

            dbContext.Set<Audit>().AddRange(auditEntries.Select(auditEntry => auditEntry.ToAudit()));

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}

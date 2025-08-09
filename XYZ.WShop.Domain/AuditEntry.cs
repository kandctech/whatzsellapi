using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;
using XYZ.WShop.Domain.Enums;

namespace XYZ.WShop.Domain
{
    public class AuditEntry
    {
        public AuditEntry(EntityEntry entry)
        {
            Entry = entry;
        }

        public EntityEntry Entry { get; }
        public string UserId { get; set; }
        public string TableName { get; set; }
        public Dictionary<string, object> KeyValues { get; } = [];
        public Dictionary<string, object> OldValues { get; } = [];
        public Dictionary<string, object> NewValues { get; } = [];
        public AuditType AuditType { get; set; }
        public List<string> ChangedColumns { get; } = [];
        public Audit ToAudit()
        {
            var audit = new Audit();
            audit.UserId = UserId != null ? Guid.Parse(UserId) : Guid.Empty;
            audit.Type = AuditType.ToString();
            audit.TableName = TableName;
            audit.CreatedDate = DateTime.UtcNow;
            audit.PrimaryKey = JsonSerializer.Serialize(KeyValues);
            audit.OldValues = OldValues.Count == 0 ? null : JsonSerializer.Serialize(OldValues);
            audit.NewValues = NewValues.Count == 0 ? null : JsonSerializer.Serialize(NewValues);
            audit.AffectedColumns = ChangedColumns.Count == 0 ? null : JsonSerializer.Serialize(ChangedColumns);

            return audit;
        }
    }
}

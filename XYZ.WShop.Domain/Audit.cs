using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYZ.WShop.Domain;
/// <summary>
/// Represents an audit entity for tracking changes.
/// </summary>
public class Audit
{
    /// <summary>
    /// Gets or sets the unique identifier of the audit.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the user ID associated with the audit.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the type of audit.
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets the name of the table being audited.
    /// </summary>
    public string? TableName { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the audit was created.
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets the old values before the change.
    /// </summary>
    public string? OldValues { get; set; }

    /// <summary>
    /// Gets or sets the new values after the change.
    /// </summary>
    public string? NewValues { get; set; }

    /// <summary>
    /// Gets or sets the columns affected by the change.
    /// </summary>
    public string? AffectedColumns { get; set; }

    /// <summary>
    /// Gets or sets the primary key of the audited entity.
    /// </summary>
    public string PrimaryKey { get; set; }
}

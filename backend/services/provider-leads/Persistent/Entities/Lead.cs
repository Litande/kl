using Plat4Me.DialLeadProvider.Application.Enums;

namespace Plat4Me.DialLeadProvider.Persistent.Entities;

public class Lead
{
    public long Id { get; set; }
    public long ClientId { get; set; }
    public long DataSourceId { get; set; }
    public long? DuplicateOfId { get; set; }
    public string Phone { get; set; } = null!;
    public DateTimeOffset? LastUpdateTime { get; set; }
    public string? ExternalId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? LanguageCode { get; set; }
    public string? CountryCode { get; set; }
    public LeadStatusTypes Status { get; set; }
    public LeadSystemStatusTypes? SystemStatus { get; set; }
    public DateTimeOffset? LastTimeOnline { get; set; }
    public DateTimeOffset RegistrationTime { get; set; }
    public long? AssignedUserId { get; set; }
    public DateTimeOffset? FirstDepositTime { get; set; }
    public DateTimeOffset? RemindOn { get; set; }
    public string? Metadata { get; set; }
    public string? Timezone { get; set; }
    public string? City { get; set; }
    public string? Email { get; set; }
    public DateTimeOffset? ImportedOn { get; set; }

    public virtual DataSource DataSource { get; set; } = null!;
    public virtual Lead? DuplicateOf { get; set; }
    public virtual Country? Country { get; set; }
    public virtual Language? Language { get; set; }
    public virtual User? AssignedUser { get; set; }

    public virtual ICollection<Lead> Duplicates { get; set; } = new HashSet<Lead>();
}
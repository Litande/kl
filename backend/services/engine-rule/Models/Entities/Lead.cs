using KL.Engine.Rule.Enums;

namespace KL.Engine.Rule.Models.Entities;

public class Lead
{
    public long Id { get; set; }
    public long ClientId { get; set; }
    public long DataSourceId { get; set; }
    public long? DuplicateOfId { get; set; }
    public string Phone { get; set; } = null!;
    public DateTimeOffset? LastUpdateTime { get; set; }
    public string? ExternalId { get; set; }
    public LeadStatusTypes Status { get; set; } = LeadStatusTypes.NewLead;
    public LeadSystemStatusTypes? SystemStatus { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? LanguageCode { get; set; }
    public string? CountryCode { get; set; }
    public DateTimeOffset? LastTimeOnline { get; set; }
    public DateTimeOffset RegistrationTime { get; set; }
    public long? LastCallAgentId { get; set; }
    public long? AssignedAgentId { get; set; }
    public DateTimeOffset? FirstDepositTime { get; set; }
    public DateTimeOffset? RemindOn { get; set; }
    public DateTimeOffset? FreezeTo { get; set; }
    public DateTimeOffset? DeletedOn { get; set; }
    public string? Timezone { get; set; }
    public bool IsTest { get; set; }
    public DateTimeOffset? FirstTimeQueuedOn { get; set; }
    public string? MetaData { get; set; }

    //public virtual Client Client { get; set; }
    public virtual DataSource DataSource { get; set; } = null!;
    public virtual Lead? DuplicateOf { get; set; }
    public virtual User? LastCallAgent { get; set; }
    public virtual User? AssignedAgent { get; set; }
    public virtual ICollection<LeadHistory> LeadHistory { get; set; } = new HashSet<LeadHistory>();
    public virtual ICollection<LeadBlacklist> LeadBlacklist { get; set; } = new HashSet<LeadBlacklist>();
}

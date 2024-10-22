using KL.Manager.API.Application.Enums;

namespace KL.Manager.API.Persistent.Entities.Projections;

public class LeadInfoProjection
{
    public long LeadId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Phone { get; set; }
    public LeadStatusTypes LeadStatus { get; set; }
    public string? AffiliateId { get; set; }
    public DateTimeOffset RegistrationTime { get; set; }
    public string? CountryCode { get; set; }
    public string? BrandName { get; set; }
}
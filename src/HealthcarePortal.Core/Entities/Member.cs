namespace HealthcarePortal.Core.Entities;

public class Member : BaseEntity
{
    public string MemberId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string SSN { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    
    // Address Information
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = "US";
    
    // Insurance Information
    public string PlanId { get; set; } = string.Empty;
    public string GroupNumber { get; set; } = string.Empty;
    public DateTime EffectiveDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public string MembershipStatus { get; set; } = "Active";
    
    // Relationships
    public virtual ICollection<Claim> Claims { get; set; } = new List<Claim>();
    public virtual ICollection<Authorization> Authorizations { get; set; } = new List<Authorization>();
    public virtual ICollection<CareJourneyPlan> CareJourneyPlans { get; set; } = new List<CareJourneyPlan>();
    
    public string FullName => $"{FirstName} {LastName}";
}
namespace HealthcarePortal.Core.Entities;

public class Provider : BaseEntity
{
    public string ProviderId { get; set; } = string.Empty;
    public string NPI { get; set; } = string.Empty; // National Provider Identifier
    public string TaxId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public string ProviderType { get; set; } = string.Empty; // Individual, Organization
    public string Specialty { get; set; } = string.Empty;
    public string SubSpecialty { get; set; } = string.Empty;
    
    // Contact Information
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string FaxNumber { get; set; } = string.Empty;
    
    // Address Information
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = "US";
    
    // Practice Information
    public bool AcceptingNewPatients { get; set; } = true;
    public bool TelehealthEnabled { get; set; } = false;
    public string NetworkStatus { get; set; } = "In-Network"; // In-Network, Out-of-Network
    public string ContractStatus { get; set; } = "Active";
    public DateTime? ContractEffectiveDate { get; set; }
    public DateTime? ContractTerminationDate { get; set; }
    
    // Quality Metrics
    public decimal? QualityRating { get; set; }
    public int? PatientSatisfactionScore { get; set; }
    
    // Relationships
    public virtual ICollection<Claim> Claims { get; set; } = new List<Claim>();
    public virtual ICollection<Authorization> Authorizations { get; set; } = new List<Authorization>();
    public virtual ICollection<ProviderSpecialty> Specialties { get; set; } = new List<ProviderSpecialty>();
    
    public string FullName => string.IsNullOrEmpty(OrganizationName) 
        ? $"{FirstName} {LastName}" 
        : OrganizationName;
}

public class ProviderSpecialty : BaseEntity
{
    public string ProviderId { get; set; } = string.Empty;
    public string SpecialtyCode { get; set; } = string.Empty;
    public string SpecialtyName { get; set; } = string.Empty;
    public bool IsPrimary { get; set; } = false;
    
    public virtual Provider Provider { get; set; } = null!;
}
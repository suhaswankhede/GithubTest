namespace HealthcarePortal.Core.Entities;

public class Authorization : BaseEntity
{
    public string AuthorizationNumber { get; set; } = string.Empty;
    public string MemberId { get; set; } = string.Empty;
    public string ProviderId { get; set; } = string.Empty;
    public string RequestingProviderId { get; set; } = string.Empty;
    
    // Request Information
    public DateTime RequestDate { get; set; }
    public DateTime ServiceDate { get; set; }
    public DateTime? ServiceEndDate { get; set; }
    public string ServiceType { get; set; } = string.Empty; // Inpatient, Outpatient, DME, etc.
    public string ProcedureCode { get; set; } = string.Empty;
    public string ProcedureDescription { get; set; } = string.Empty;
    public string DiagnosisCode { get; set; } = string.Empty;
    public string DiagnosisDescription { get; set; } = string.Empty;
    
    // Authorization Details
    public string AuthorizationStatus { get; set; } = string.Empty; // Pending, Approved, Denied, Expired
    public DateTime? ApprovalDate { get; set; }
    public DateTime? DenialDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public int? AuthorizedUnits { get; set; }
    public int? UsedUnits { get; set; }
    public int? RemainingUnits { get; set; }
    
    // Clinical Information
    public string ClinicalNotes { get; set; } = string.Empty;
    public string MedicalNecessity { get; set; } = string.Empty;
    public string? DenialReason { get; set; }
    public string? DenialCode { get; set; }
    
    // Review Information
    public string? ReviewedBy { get; set; }
    public DateTime? ReviewDate { get; set; }
    public string? ReviewerNotes { get; set; }
    public string Priority { get; set; } = "Standard"; // Urgent, Standard, Routine
    
    // Appeal Information
    public bool IsAppealable { get; set; } = true;
    public DateTime? AppealDeadline { get; set; }
    public bool HasAppeal { get; set; } = false;
    
    // Relationships
    public virtual Member Member { get; set; } = null!;
    public virtual Provider Provider { get; set; } = null!;
    public virtual Provider RequestingProvider { get; set; } = null!;
    public virtual ICollection<AuthorizationDocument> Documents { get; set; } = new List<AuthorizationDocument>();
    public virtual ICollection<AuthorizationAppeal> Appeals { get; set; } = new List<AuthorizationAppeal>();
}

public class AuthorizationDocument : BaseEntity
{
    public string AuthorizationId { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty; // Medical Records, Lab Results, Imaging, etc.
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string UploadedBy { get; set; } = string.Empty;
    
    public virtual Authorization Authorization { get; set; } = null!;
}

public class AuthorizationAppeal : BaseEntity
{
    public string AuthorizationId { get; set; } = string.Empty;
    public string AppealNumber { get; set; } = string.Empty;
    public DateTime AppealDate { get; set; }
    public string AppealReason { get; set; } = string.Empty;
    public string AppealStatus { get; set; } = string.Empty; // Submitted, Under Review, Approved, Denied
    public string? AppealNotes { get; set; }
    public DateTime? ResolutionDate { get; set; }
    public string? Resolution { get; set; }
    
    public virtual Authorization Authorization { get; set; } = null!;
}
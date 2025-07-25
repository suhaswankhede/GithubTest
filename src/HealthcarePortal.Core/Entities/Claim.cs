namespace HealthcarePortal.Core.Entities;

public class Claim : BaseEntity
{
    public string ClaimNumber { get; set; } = string.Empty;
    public string MemberId { get; set; } = string.Empty;
    public string ProviderId { get; set; } = string.Empty;
    public string ProviderName { get; set; } = string.Empty;
    public DateTime ServiceDate { get; set; }
    public DateTime? ServiceEndDate { get; set; }
    public DateTime SubmittedDate { get; set; }
    public DateTime? ProcessedDate { get; set; }
    
    // Financial Information
    public decimal BilledAmount { get; set; }
    public decimal AllowedAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DeductibleAmount { get; set; }
    public decimal CoPayAmount { get; set; }
    public decimal CoInsuranceAmount { get; set; }
    public decimal MemberResponsibility { get; set; }
    
    // Claim Details
    public string ClaimStatus { get; set; } = string.Empty; // Submitted, Processing, Paid, Denied, Adjusted
    public string PrimaryDiagnosisCode { get; set; } = string.Empty;
    public string PrimaryDiagnosisDescription { get; set; } = string.Empty;
    public string ProcedureCode { get; set; } = string.Empty;
    public string ProcedureDescription { get; set; } = string.Empty;
    public string PlaceOfService { get; set; } = string.Empty;
    
    // Denial/Adjustment Information
    public string? DenialReason { get; set; }
    public string? DenialCode { get; set; }
    public string? AdjustmentReason { get; set; }
    
    // EOB Information
    public string? EOBExplanation { get; set; }
    public string? PaymentExplanation { get; set; }
    
    // Relationships
    public virtual Member Member { get; set; } = null!;
    public virtual Provider Provider { get; set; } = null!;
    public virtual ICollection<ClaimLineItem> LineItems { get; set; } = new List<ClaimLineItem>();
}

public class ClaimLineItem : BaseEntity
{
    public string ClaimId { get; set; } = string.Empty;
    public int LineNumber { get; set; }
    public string ProcedureCode { get; set; } = string.Empty;
    public string ProcedureDescription { get; set; } = string.Empty;
    public int Units { get; set; }
    public decimal BilledAmount { get; set; }
    public decimal AllowedAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal MemberResponsibility { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? DenialReason { get; set; }
    
    public virtual Claim Claim { get; set; } = null!;
}
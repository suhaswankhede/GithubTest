namespace HealthcarePortal.Member.Dashboard.Models;

public class DashboardViewModel
{
    public MemberSummary MemberInfo { get; set; } = new();
    public ClaimsSummary ClaimsSummary { get; set; } = new();
    public CoverageSummary CoverageSummary { get; set; } = new();
    public TasksSummary TasksSummary { get; set; } = new();
    public UpcomingItems UpcomingItems { get; set; } = new();
    public RecentActivity RecentActivity { get; set; } = new();
    public QuickActions QuickActions { get; set; } = new();
}

public class MemberSummary
{
    public string MemberId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public string GroupNumber { get; set; } = string.Empty;
    public DateTime EffectiveDate { get; set; }
    public string MembershipStatus { get; set; } = string.Empty;
}

public class ClaimsSummary
{
    public int TotalClaims { get; set; }
    public int PendingClaims { get; set; }
    public int ProcessedClaims { get; set; }
    public int DeniedClaims { get; set; }
    public decimal TotalBilledAmount { get; set; }
    public decimal TotalPaidAmount { get; set; }
    public decimal YearToDateDeductible { get; set; }
    public decimal YearToDateOutOfPocket { get; set; }
    public List<RecentClaimItem> RecentClaims { get; set; } = new();
}

public class RecentClaimItem
{
    public string ClaimNumber { get; set; } = string.Empty;
    public string ProviderName { get; set; } = string.Empty;
    public DateTime ServiceDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal BilledAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal MemberResponsibility { get; set; }
}

public class CoverageSummary
{
    public decimal AnnualDeductible { get; set; }
    public decimal DeductibleRemaining { get; set; }
    public decimal DeductibleMet { get; set; }
    public decimal OutOfPocketMaximum { get; set; }
    public decimal OutOfPocketRemaining { get; set; }
    public decimal OutOfPocketMet { get; set; }
    public List<BenefitSummaryItem> Benefits { get; set; } = new();
}

public class BenefitSummaryItem
{
    public string BenefitCategory { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CoverageLevel { get; set; } = string.Empty;
    public decimal? Copay { get; set; }
    public decimal? Coinsurance { get; set; }
    public bool RequiresAuth { get; set; }
    public string NetworkStatus { get; set; } = string.Empty;
}

public class TasksSummary
{
    public int TotalPendingTasks { get; set; }
    public int OverdueTasks { get; set; }
    public int CompletedThisWeek { get; set; }
    public List<PendingTaskItem> PendingTasks { get; set; } = new();
}

public class PendingTaskItem
{
    public string TaskId { get; set; } = string.Empty;
    public string TaskName { get; set; } = string.Empty;
    public string TaskType { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public string Priority { get; set; } = string.Empty;
    public bool IsOverdue { get; set; }
}

public class UpcomingItems
{
    public List<UpcomingAppointment> Appointments { get; set; } = new();
    public List<UpcomingRenewal> Renewals { get; set; } = new();
    public List<UpcomingReminder> Reminders { get; set; } = new();
}

public class UpcomingAppointment
{
    public string AppointmentId { get; set; } = string.Empty;
    public string ProviderName { get; set; } = string.Empty;
    public string AppointmentType { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool RequiresAuth { get; set; }
}

public class UpcomingRenewal
{
    public string ItemType { get; set; } = string.Empty; // Authorization, Prescription, etc.
    public string Description { get; set; } = string.Empty;
    public DateTime ExpirationDate { get; set; }
    public int DaysUntilExpiration { get; set; }
    public string ActionRequired { get; set; } = string.Empty;
}

public class UpcomingReminder
{
    public string ReminderId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime ReminderDate { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
}

public class RecentActivity
{
    public List<ActivityItem> Activities { get; set; } = new();
}

public class ActivityItem
{
    public string ActivityType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime ActivityDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ActionUrl { get; set; }
}

public class QuickActions
{
    public List<QuickActionItem> Actions { get; set; } = new();
}

public class QuickActionItem
{
    public string ActionId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string ActionUrl { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
}
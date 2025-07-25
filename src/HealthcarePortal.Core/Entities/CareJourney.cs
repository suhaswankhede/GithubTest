namespace HealthcarePortal.Core.Entities;

public class CareJourneyPlan : BaseEntity
{
    public string MemberId { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public string PlanType { get; set; } = string.Empty; // Chronic Care, Preventive, Wellness, Disease Management
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = "Active"; // Active, Completed, Discontinued, On Hold
    public string Priority { get; set; } = "Medium"; // High, Medium, Low
    
    // Health Condition Information
    public string? ConditionCode { get; set; }
    public string? ConditionName { get; set; }
    public string? ConditionDescription { get; set; }
    public string Severity { get; set; } = string.Empty; // Mild, Moderate, Severe
    
    // Care Team Information
    public string? PrimaryCareProviderId { get; set; }
    public string? CaseManagerId { get; set; }
    public string? SpecialistId { get; set; }
    
    // Progress Tracking
    public decimal OverallProgress { get; set; } = 0; // 0-100 percentage
    public DateTime? LastAssessmentDate { get; set; }
    public DateTime? NextAssessmentDate { get; set; }
    
    // Relationships
    public virtual Member Member { get; set; } = null!;
    public virtual ICollection<CareGoal> CareGoals { get; set; } = new List<CareGoal>();
    public virtual ICollection<CareTask> CareTasks { get; set; } = new List<CareTask>();
    public virtual ICollection<CareAssessment> Assessments { get; set; } = new List<CareAssessment>();
}

public class CareGoal : BaseEntity
{
    public string CareJourneyPlanId { get; set; } = string.Empty;
    public string GoalName { get; set; } = string.Empty;
    public string GoalDescription { get; set; } = string.Empty;
    public string GoalType { get; set; } = string.Empty; // Clinical, Behavioral, Educational
    public string Category { get; set; } = string.Empty; // Weight Management, Blood Pressure Control, etc.
    
    // Goal Metrics
    public string MetricType { get; set; } = string.Empty; // Numeric, Boolean, Text
    public decimal? TargetValue { get; set; }
    public decimal? CurrentValue { get; set; }
    public string? Unit { get; set; }
    public DateTime TargetDate { get; set; }
    public string Status { get; set; } = "In Progress"; // Not Started, In Progress, Achieved, Discontinued
    
    // Progress Information
    public decimal Progress { get; set; } = 0; // 0-100 percentage
    public DateTime? LastUpdated { get; set; }
    public string? Notes { get; set; }
    
    public virtual CareJourneyPlan CareJourneyPlan { get; set; } = null!;
    public virtual ICollection<CareTask> RelatedTasks { get; set; } = new List<CareTask>();
}

public class CareTask : BaseEntity
{
    public string CareJourneyPlanId { get; set; } = string.Empty;
    public string? CareGoalId { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public string TaskDescription { get; set; } = string.Empty;
    public string TaskType { get; set; } = string.Empty; // Appointment, Medication, Exercise, Education, Test
    public string Category { get; set; } = string.Empty;
    
    // Task Details
    public DateTime DueDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, In Progress, Completed, Overdue, Cancelled
    public string Priority { get; set; } = "Medium"; // High, Medium, Low
    public bool IsRecurring { get; set; } = false;
    public string? RecurrencePattern { get; set; } // Daily, Weekly, Monthly, etc.
    
    // Completion Information
    public string? CompletionNotes { get; set; }
    public string? CompletedBy { get; set; }
    public decimal? CompletionValue { get; set; }
    public string? CompletionUnit { get; set; }
    
    // Reminders
    public bool ReminderEnabled { get; set; } = true;
    public DateTime? LastReminderSent { get; set; }
    public DateTime? NextReminderDate { get; set; }
    
    public virtual CareJourneyPlan CareJourneyPlan { get; set; } = null!;
    public virtual CareGoal? CareGoal { get; set; }
}

public class CareAssessment : BaseEntity
{
    public string CareJourneyPlanId { get; set; } = string.Empty;
    public string AssessmentName { get; set; } = string.Empty;
    public string AssessmentType { get; set; } = string.Empty; // Baseline, Progress, Outcome
    public DateTime AssessmentDate { get; set; }
    public string AssessedBy { get; set; } = string.Empty; // Provider ID or system
    
    // Assessment Results
    public string OverallScore { get; set; } = string.Empty;
    public string RiskLevel { get; set; } = string.Empty; // Low, Medium, High
    public string Summary { get; set; } = string.Empty;
    public string Recommendations { get; set; } = string.Empty;
    
    public virtual CareJourneyPlan CareJourneyPlan { get; set; } = null!;
    public virtual ICollection<AssessmentQuestion> Questions { get; set; } = new List<AssessmentQuestion>();
}

public class AssessmentQuestion : BaseEntity
{
    public string CareAssessmentId { get; set; } = string.Empty;
    public string QuestionText { get; set; } = string.Empty;
    public string QuestionType { get; set; } = string.Empty; // Scale, YesNo, MultipleChoice, Text
    public string Answer { get; set; } = string.Empty;
    public decimal? NumericAnswer { get; set; }
    public int SortOrder { get; set; }
    
    public virtual CareAssessment CareAssessment { get; set; } = null!;
}
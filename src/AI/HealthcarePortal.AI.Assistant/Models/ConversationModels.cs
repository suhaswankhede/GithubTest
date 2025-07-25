namespace HealthcarePortal.AI.Assistant.Models;

public class ConversationRequest
{
    public string UserId { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public ConversationContext Context { get; set; } = new();
    public ConversationMode Mode { get; set; } = ConversationMode.Chat;
    public string? AudioData { get; set; } // Base64 encoded audio for voice input
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class ConversationResponse
{
    public string ResponseId { get; set; } = Guid.NewGuid().ToString();
    public string SessionId { get; set; } = string.Empty;
    public string TextResponse { get; set; } = string.Empty;
    public string? AudioResponse { get; set; } // Base64 encoded audio for voice output
    public ConversationIntent Intent { get; set; } = new();
    public List<ActionSuggestion> SuggestedActions { get; set; } = new();
    public ConversationContext UpdatedContext { get; set; } = new();
    public bool RequiresAuthentication { get; set; } = false;
    public bool RequiresAdditionalInfo { get; set; } = false;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public ConversationStatus Status { get; set; } = ConversationStatus.Success;
    public string? ErrorMessage { get; set; }
}

public class ConversationContext
{
    public string MemberId { get; set; } = string.Empty;
    public string UserRole { get; set; } = string.Empty; // Member, Provider, Admin
    public Dictionary<string, object> SessionData { get; set; } = new();
    public List<string> ConversationHistory { get; set; } = new();
    public string CurrentTopic { get; set; } = string.Empty;
    public DateTime LastActivity { get; set; } = DateTime.UtcNow;
    public bool IsAuthenticated { get; set; } = false;
    public Dictionary<string, string> UserPreferences { get; set; } = new();
}

public class ConversationIntent
{
    public string IntentName { get; set; } = string.Empty;
    public decimal Confidence { get; set; }
    public Dictionary<string, string> Entities { get; set; } = new();
    public string Category { get; set; } = string.Empty; // Claims, Benefits, Provider Search, etc.
    public bool RequiresEscalation { get; set; } = false;
}

public class ActionSuggestion
{
    public string ActionId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ActionType { get; set; } = string.Empty; // Navigation, Form, Phone, etc.
    public string ActionUrl { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
    public string Icon { get; set; } = string.Empty;
    public int Priority { get; set; } = 0;
}

public class VoiceInteractionRequest
{
    public string SessionId { get; set; } = string.Empty;
    public string AudioData { get; set; } = string.Empty; // Base64 encoded
    public string AudioFormat { get; set; } = "wav";
    public int SampleRate { get; set; } = 16000;
    public string Language { get; set; } = "en-US";
    public bool EnableSpeechToText { get; set; } = true;
    public bool EnableTextToSpeech { get; set; } = true;
    public VoiceSettings VoiceSettings { get; set; } = new();
}

public class VoiceInteractionResponse
{
    public string TranscribedText { get; set; } = string.Empty;
    public string ResponseText { get; set; } = string.Empty;
    public string? AudioResponse { get; set; } // Base64 encoded
    public decimal TranscriptionConfidence { get; set; }
    public bool IsSuccessful { get; set; }
    public string? ErrorMessage { get; set; }
    public TimeSpan ProcessingTime { get; set; }
}

public class VoiceSettings
{
    public string VoiceName { get; set; } = "en-US-JennyNeural";
    public decimal SpeechRate { get; set; } = 1.0m;
    public decimal Volume { get; set; } = 1.0m;
    public string Pitch { get; set; } = "medium";
    public string Style { get; set; } = "friendly";
}

public class ChatSession
{
    public string SessionId { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    public DateTime LastActivity { get; set; } = DateTime.UtcNow;
    public ConversationContext Context { get; set; } = new();
    public List<ChatMessage> Messages { get; set; } = new();
    public ChatSessionStatus Status { get; set; } = ChatSessionStatus.Active;
    public bool IsVoiceEnabled { get; set; } = false;
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class ChatMessage
{
    public string MessageId { get; set; } = Guid.NewGuid().ToString();
    public string SessionId { get; set; } = string.Empty;
    public ChatMessageType Type { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? AudioContent { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public ConversationIntent? Intent { get; set; }
    public List<ActionSuggestion> Actions { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public enum ConversationMode
{
    Chat,
    Voice,
    Mixed
}

public enum ConversationStatus
{
    Success,
    Error,
    Pending,
    RequiresAuth,
    RequiresEscalation
}

public enum ChatMessageType
{
    UserText,
    UserVoice,
    AssistantText,
    AssistantVoice,
    System,
    Action
}

public enum ChatSessionStatus
{
    Active,
    Inactive,
    Ended,
    Escalated
}
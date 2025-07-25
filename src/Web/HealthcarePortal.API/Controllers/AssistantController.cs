using HealthcarePortal.AI.Assistant.Models;
using HealthcarePortal.AI.Assistant.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthcarePortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AssistantController : ControllerBase
{
    private readonly IHealthcareAssistantService _assistantService;
    private readonly ILogger<AssistantController> _logger;

    public AssistantController(
        IHealthcareAssistantService assistantService,
        ILogger<AssistantController> logger)
    {
        _assistantService = assistantService;
        _logger = logger;
    }

    /// <summary>
    /// Process a chat conversation with the AI assistant
    /// </summary>
    [HttpPost("chat")]
    public async Task<ActionResult<ConversationResponse>> ProcessChatAsync(
        [FromBody] ConversationRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Get user information from JWT token
            var userId = User.FindFirst("sub")?.Value ?? string.Empty;
            var memberId = User.FindFirst("member_id")?.Value ?? string.Empty;

            // Set user context
            request.UserId = userId;
            request.Context.MemberId = memberId;
            request.Context.IsAuthenticated = true;
            request.Context.UserRole = "Member";
            request.Mode = ConversationMode.Chat;

            _logger.LogInformation("Processing chat request for user {UserId}", userId);

            var response = await _assistantService.ProcessConversationAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing chat request");
            return StatusCode(500, new ConversationResponse
            {
                SessionId = request.SessionId,
                TextResponse = "I'm sorry, I'm experiencing technical difficulties. Please try again later.",
                Status = ConversationStatus.Error,
                ErrorMessage = "Internal server error"
            });
        }
    }

    /// <summary>
    /// Process a voice conversation with the AI assistant
    /// </summary>
    [HttpPost("voice")]
    public async Task<ActionResult<ConversationResponse>> ProcessVoiceAsync(
        [FromBody] VoiceConversationRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = User.FindFirst("sub")?.Value ?? string.Empty;
            var memberId = User.FindFirst("member_id")?.Value ?? string.Empty;

            var conversationRequest = new ConversationRequest
            {
                UserId = userId,
                SessionId = request.SessionId,
                Message = string.Empty, // Will be populated from audio transcription
                AudioData = request.AudioData,
                Mode = ConversationMode.Voice,
                Context = new ConversationContext
                {
                    MemberId = memberId,
                    IsAuthenticated = true,
                    UserRole = "Member"
                }
            };

            _logger.LogInformation("Processing voice request for user {UserId}", userId);

            var response = await _assistantService.ProcessConversationAsync(conversationRequest, cancellationToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing voice request");
            return StatusCode(500, new ConversationResponse
            {
                SessionId = request.SessionId,
                TextResponse = "I'm sorry, I'm experiencing technical difficulties with voice processing. Please try again later.",
                Status = ConversationStatus.Error,
                ErrorMessage = "Internal server error"
            });
        }
    }

    /// <summary>
    /// Get conversation history for a session
    /// </summary>
    [HttpGet("sessions/{sessionId}/history")]
    public async Task<ActionResult<ChatSession>> GetConversationHistoryAsync(
        string sessionId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = User.FindFirst("sub")?.Value ?? string.Empty;
            
            // In a real implementation, this would fetch from a data store
            var session = new ChatSession
            {
                SessionId = sessionId,
                UserId = userId,
                StartTime = DateTime.UtcNow.AddHours(-1),
                LastActivity = DateTime.UtcNow,
                Status = ChatSessionStatus.Active,
                Messages = new List<ChatMessage>()
            };

            return Ok(session);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving conversation history for session {SessionId}", sessionId);
            return StatusCode(500, "Unable to retrieve conversation history");
        }
    }

    /// <summary>
    /// Start a new conversation session
    /// </summary>
    [HttpPost("sessions")]
    public async Task<ActionResult<ChatSession>> StartSessionAsync(
        [FromBody] StartSessionRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = User.FindFirst("sub")?.Value ?? string.Empty;
            var memberId = User.FindFirst("member_id")?.Value ?? string.Empty;

            var session = new ChatSession
            {
                SessionId = Guid.NewGuid().ToString(),
                UserId = userId,
                StartTime = DateTime.UtcNow,
                LastActivity = DateTime.UtcNow,
                Status = ChatSessionStatus.Active,
                IsVoiceEnabled = request.EnableVoice,
                Context = new ConversationContext
                {
                    MemberId = memberId,
                    IsAuthenticated = true,
                    UserRole = "Member",
                    UserPreferences = request.UserPreferences ?? new Dictionary<string, string>()
                }
            };

            _logger.LogInformation("Started new session {SessionId} for user {UserId}", session.SessionId, userId);

            return Ok(session);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting new session");
            return StatusCode(500, "Unable to start new session");
        }
    }

    /// <summary>
    /// End a conversation session
    /// </summary>
    [HttpPost("sessions/{sessionId}/end")]
    public async Task<ActionResult> EndSessionAsync(
        string sessionId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = User.FindFirst("sub")?.Value ?? string.Empty;
            
            // In a real implementation, this would update the session in the data store
            _logger.LogInformation("Ended session {SessionId} for user {UserId}", sessionId, userId);

            return Ok(new { Message = "Session ended successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ending session {SessionId}", sessionId);
            return StatusCode(500, "Unable to end session");
        }
    }

    /// <summary>
    /// Get available assistant capabilities and features
    /// </summary>
    [HttpGet("capabilities")]
    [AllowAnonymous]
    public ActionResult<AssistantCapabilities> GetCapabilities()
    {
        var capabilities = new AssistantCapabilities
        {
            SupportedLanguages = new[] { "en-US", "es-US" },
            SupportedModes = new[] { "chat", "voice", "mixed" },
            Features = new[]
            {
                "Claims inquiry and explanation",
                "Benefits information",
                "Provider search",
                "Prior authorization tracking",
                "Appointment scheduling assistance",
                "Care plan management",
                "Medication information",
                "Billing and payment help"
            },
            VoiceCapabilities = new VoiceCapabilities
            {
                SpeechToTextEnabled = true,
                TextToSpeechEnabled = true,
                SupportedVoices = new[] { "en-US-JennyNeural", "en-US-GuyNeural", "es-US-AlonsoNeural" },
                SupportedAudioFormats = new[] { "wav", "mp3", "ogg" }
            }
        };

        return Ok(capabilities);
    }
}

public class VoiceConversationRequest
{
    public string SessionId { get; set; } = string.Empty;
    public string AudioData { get; set; } = string.Empty;
    public VoiceSettings? VoiceSettings { get; set; }
}

public class StartSessionRequest
{
    public bool EnableVoice { get; set; } = false;
    public Dictionary<string, string>? UserPreferences { get; set; }
}

public class AssistantCapabilities
{
    public string[] SupportedLanguages { get; set; } = Array.Empty<string>();
    public string[] SupportedModes { get; set; } = Array.Empty<string>();
    public string[] Features { get; set; } = Array.Empty<string>();
    public VoiceCapabilities VoiceCapabilities { get; set; } = new();
}

public class VoiceCapabilities
{
    public bool SpeechToTextEnabled { get; set; }
    public bool TextToSpeechEnabled { get; set; }
    public string[] SupportedVoices { get; set; } = Array.Empty<string>();
    public string[] SupportedAudioFormats { get; set; } = Array.Empty<string>();
}
using Azure.AI.OpenAI;
using HealthcarePortal.AI.Assistant.Models;
using HealthcarePortal.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace HealthcarePortal.AI.Assistant.Services;

public class HealthcareAssistantService : IHealthcareAssistantService
{
    private readonly OpenAIClient _openAIClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<HealthcareAssistantService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISpeechService _speechService;

    public HealthcareAssistantService(
        OpenAIClient openAIClient,
        IConfiguration configuration,
        ILogger<HealthcareAssistantService> logger,
        IUnitOfWork unitOfWork,
        ISpeechService speechService)
    {
        _openAIClient = openAIClient;
        _configuration = configuration;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _speechService = speechService;
    }

    public async Task<ConversationResponse> ProcessConversationAsync(ConversationRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Processing conversation for user {UserId} in session {SessionId}", 
                request.UserId, request.SessionId);

            // Handle voice input if provided
            string messageText = request.Message;
            if (!string.IsNullOrEmpty(request.AudioData) && request.Mode != ConversationMode.Chat)
            {
                var transcription = await _speechService.TranscribeAudioAsync(request.AudioData, cancellationToken);
                messageText = transcription.TranscribedText;
            }

            // Determine intent and entities
            var intent = await AnalyzeIntentAsync(messageText, request.Context, cancellationToken);

            // Generate context-aware response
            var responseText = await GenerateResponseAsync(messageText, intent, request.Context, cancellationToken);

            // Create response object
            var response = new ConversationResponse
            {
                SessionId = request.SessionId,
                TextResponse = responseText,
                Intent = intent,
                UpdatedContext = await UpdateContextAsync(request.Context, intent, cancellationToken)
            };

            // Add suggested actions based on intent
            response.SuggestedActions = await GenerateSuggestedActionsAsync(intent, request.Context, cancellationToken);

            // Generate voice response if requested
            if (request.Mode != ConversationMode.Chat)
            {
                response.AudioResponse = await _speechService.SynthesizeSpeechAsync(responseText, cancellationToken);
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing conversation for user {UserId}", request.UserId);
            return new ConversationResponse
            {
                SessionId = request.SessionId,
                TextResponse = "I apologize, but I'm experiencing technical difficulties. Please try again or contact support.",
                Status = ConversationStatus.Error,
                ErrorMessage = ex.Message
            };
        }
    }

    private async Task<ConversationIntent> AnalyzeIntentAsync(string message, ConversationContext context, CancellationToken cancellationToken)
    {
        var systemPrompt = @"
You are a healthcare intent analyzer. Analyze the user's message and determine their intent.
Return a JSON object with the following structure:
{
    ""IntentName"": ""string"",
    ""Confidence"": 0.0-1.0,
    ""Entities"": {""key"": ""value""},
    ""Category"": ""string"",
    ""RequiresEscalation"": boolean
}

Categories: Claims, Benefits, Provider Search, Appointments, Medications, Care Plans, Authorization, Billing, General
Common intents: ViewClaims, SearchProvider, CheckBenefits, ScheduleAppointment, TrackAuth, etc.";

        var messages = new[]
        {
            new { role = "system", content = systemPrompt },
            new { role = "user", content = $"Message: {message}\nContext: Member authenticated: {context.IsAuthenticated}" }
        };

        try
        {
            // This would be the actual OpenAI API call
            // For now, returning a mock response
            var mockIntent = new ConversationIntent
            {
                IntentName = DetermineIntentFromMessage(message),
                Confidence = 0.85m,
                Category = DetermineCategoryFromMessage(message),
                Entities = ExtractEntitiesFromMessage(message)
            };

            return mockIntent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing intent for message: {Message}", message);
            return new ConversationIntent
            {
                IntentName = "Unknown",
                Confidence = 0.0m,
                Category = "General"
            };
        }
    }

    private async Task<string> GenerateResponseAsync(string message, ConversationIntent intent, ConversationContext context, CancellationToken cancellationToken)
    {
        var systemPrompt = @"
You are a helpful healthcare virtual assistant for a health insurance portal. 
You help members with claims, benefits, provider searches, appointments, and general healthcare questions.

Guidelines:
- Be empathetic and professional
- Provide accurate, helpful information
- If you don't know something, admit it and suggest next steps
- Always prioritize patient privacy and security
- Suggest specific actions when appropriate
- Use simple, clear language
- If the request requires authentication and the user isn't authenticated, guide them to log in
- For complex medical questions, recommend consulting a healthcare provider

Current user context:
- Authenticated: {IsAuthenticated}
- Member ID: {MemberId}
- Current Topic: {CurrentTopic}";

        try
        {
            // Build conversation history
            var conversationHistory = context.ConversationHistory.TakeLast(5).ToList();
            
            // Generate contextual response based on intent
            return intent.IntentName switch
            {
                "ViewClaims" => await GenerateClaimsResponseAsync(context, cancellationToken),
                "SearchProvider" => await GenerateProviderSearchResponseAsync(message, intent, cancellationToken),
                "CheckBenefits" => await GenerateBenefitsResponseAsync(context, cancellationToken),
                "ScheduleAppointment" => "I can help you find providers who are accepting new patients. Would you like me to search for providers in your area?",
                "TrackAuth" => await GenerateAuthorizationResponseAsync(context, cancellationToken),
                _ => await GenerateGeneralResponseAsync(message, context, cancellationToken)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating response for intent {Intent}", intent.IntentName);
            return "I apologize, but I'm having trouble processing your request right now. Please try again or contact customer service.";
        }
    }

    private async Task<string> GenerateClaimsResponseAsync(ConversationContext context, CancellationToken cancellationToken)
    {
        if (!context.IsAuthenticated)
        {
            return "To view your claims information, please log in to your account first. I'll be happy to help you review your claims once you're authenticated.";
        }

        try
        {
            var recentClaims = await _unitOfWork.Claims
                .FindAsync(c => c.MemberId == context.MemberId, cancellationToken);
            
            var claimCount = recentClaims.Count();
            var pendingCount = recentClaims.Count(c => c.ClaimStatus == "Processing");

            return $"I found {claimCount} claims in your account. You have {pendingCount} claims currently being processed. Would you like me to show you the details of your recent claims or help you with a specific claim?";
        }
        catch
        {
            return "I'm having trouble accessing your claims information right now. Please try again or contact customer service for assistance.";
        }
    }

    private async Task<string> GenerateProviderSearchResponseAsync(string message, ConversationIntent intent, CancellationToken cancellationToken)
    {
        var specialty = intent.Entities.GetValueOrDefault("specialty", "");
        var location = intent.Entities.GetValueOrDefault("location", "");

        if (string.IsNullOrEmpty(specialty) && string.IsNullOrEmpty(location))
        {
            return "I can help you find healthcare providers. What type of provider are you looking for? For example, primary care physician, specialist (like cardiologist or dermatologist), or a specific location?";
        }

        return $"I'll help you find {(string.IsNullOrEmpty(specialty) ? "providers" : specialty + " providers")}" +
               $"{(string.IsNullOrEmpty(location) ? "" : " in " + location)}. " +
               "Let me search our network for available providers who are accepting new patients.";
    }

    private async Task<string> GenerateBenefitsResponseAsync(ConversationContext context, CancellationToken cancellationToken)
    {
        if (!context.IsAuthenticated)
        {
            return "To check your benefits information, please log in to your account. Once authenticated, I can show you your coverage details, deductible status, and copay information.";
        }

        return "I can help you understand your benefits. Here's what I can show you:\n" +
               "• Your current deductible status\n" +
               "• Copay amounts for different services\n" +
               "• Coverage levels for medications\n" +
               "• Network provider information\n\n" +
               "What specific benefit information would you like to know about?";
    }

    private async Task<string> GenerateAuthorizationResponseAsync(ConversationContext context, CancellationToken cancellationToken)
    {
        if (!context.IsAuthenticated)
        {
            return "To check your prior authorization status, please log in to your account first.";
        }

        try
        {
            var pendingAuths = await _unitOfWork.Authorizations
                .FindAsync(a => a.MemberId == context.MemberId && a.AuthorizationStatus == "Pending", cancellationToken);

            var authCount = pendingAuths.Count();
            return authCount == 0 
                ? "You don't have any pending prior authorization requests. Is there a specific authorization you'd like me to help you track?"
                : $"You have {authCount} pending prior authorization request{(authCount > 1 ? "s" : "")}. Would you like me to show you the details?";
        }
        catch
        {
            return "I'm having trouble accessing your authorization information. Please try again or contact customer service.";
        }
    }

    private async Task<string> GenerateGeneralResponseAsync(string message, ConversationContext context, CancellationToken cancellationToken)
    {
        return "I'm here to help you with your healthcare needs. I can assist you with:\n" +
               "• Viewing and understanding your claims\n" +
               "• Checking your benefits and coverage\n" +
               "• Finding healthcare providers\n" +
               "• Tracking prior authorizations\n" +
               "• Managing your care plans\n\n" +
               "What would you like help with today?";
    }

    private async Task<List<ActionSuggestion>> GenerateSuggestedActionsAsync(ConversationIntent intent, ConversationContext context, CancellationToken cancellationToken)
    {
        return intent.IntentName switch
        {
            "ViewClaims" => new List<ActionSuggestion>
            {
                new() { ActionId = "view-claims", Title = "View All Claims", ActionUrl = "/member/claims", ActionType = "Navigation" },
                new() { ActionId = "search-claims", Title = "Search Claims", ActionUrl = "/member/claims/search", ActionType = "Navigation" }
            },
            "SearchProvider" => new List<ActionSuggestion>
            {
                new() { ActionId = "find-provider", Title = "Provider Directory", ActionUrl = "/member/find-care", ActionType = "Navigation" },
                new() { ActionId = "telehealth", Title = "Telehealth Options", ActionUrl = "/member/telehealth", ActionType = "Navigation" }
            },
            "CheckBenefits" => new List<ActionSuggestion>
            {
                new() { ActionId = "view-benefits", Title = "View Benefits", ActionUrl = "/member/benefits", ActionType = "Navigation" },
                new() { ActionId = "download-card", Title = "Download ID Card", ActionUrl = "/member/id-card", ActionType = "Download" }
            },
            _ => new List<ActionSuggestion>()
        };
    }

    private async Task<ConversationContext> UpdateContextAsync(ConversationContext context, ConversationIntent intent, CancellationToken cancellationToken)
    {
        context.CurrentTopic = intent.Category;
        context.LastActivity = DateTime.UtcNow;
        context.ConversationHistory.Add($"{intent.IntentName}:{DateTime.UtcNow:HH:mm}");
        
        // Keep only last 10 items in history
        if (context.ConversationHistory.Count > 10)
        {
            context.ConversationHistory = context.ConversationHistory.TakeLast(10).ToList();
        }

        return context;
    }

    // Helper methods for intent analysis
    private string DetermineIntentFromMessage(string message)
    {
        var lowerMessage = message.ToLower();
        
        if (lowerMessage.Contains("claim") || lowerMessage.Contains("eob"))
            return "ViewClaims";
        if (lowerMessage.Contains("provider") || lowerMessage.Contains("doctor") || lowerMessage.Contains("find"))
            return "SearchProvider";
        if (lowerMessage.Contains("benefit") || lowerMessage.Contains("coverage") || lowerMessage.Contains("deductible"))
            return "CheckBenefits";
        if (lowerMessage.Contains("appointment") || lowerMessage.Contains("schedule"))
            return "ScheduleAppointment";
        if (lowerMessage.Contains("authorization") || lowerMessage.Contains("prior auth"))
            return "TrackAuth";
        
        return "General";
    }

    private string DetermineCategoryFromMessage(string message)
    {
        var lowerMessage = message.ToLower();
        
        if (lowerMessage.Contains("claim")) return "Claims";
        if (lowerMessage.Contains("provider") || lowerMessage.Contains("doctor")) return "Provider Search";
        if (lowerMessage.Contains("benefit") || lowerMessage.Contains("coverage")) return "Benefits";
        if (lowerMessage.Contains("appointment")) return "Appointments";
        if (lowerMessage.Contains("authorization")) return "Authorization";
        
        return "General";
    }

    private Dictionary<string, string> ExtractEntitiesFromMessage(string message)
    {
        var entities = new Dictionary<string, string>();
        var lowerMessage = message.ToLower();

        // Simple entity extraction (in production, use more sophisticated NLP)
        if (lowerMessage.Contains("cardiologist")) entities["specialty"] = "cardiologist";
        if (lowerMessage.Contains("dermatologist")) entities["specialty"] = "dermatologist";
        if (lowerMessage.Contains("primary care")) entities["specialty"] = "primary care";
        
        // Extract locations (simplified)
        var commonCities = new[] { "chicago", "new york", "los angeles", "houston", "phoenix" };
        foreach (var city in commonCities)
        {
            if (lowerMessage.Contains(city))
            {
                entities["location"] = city;
                break;
            }
        }

        return entities;
    }
}

public interface IHealthcareAssistantService
{
    Task<ConversationResponse> ProcessConversationAsync(ConversationRequest request, CancellationToken cancellationToken = default);
}

public interface ISpeechService
{
    Task<VoiceInteractionResponse> TranscribeAudioAsync(string audioData, CancellationToken cancellationToken = default);
    Task<string> SynthesizeSpeechAsync(string text, CancellationToken cancellationToken = default);
}
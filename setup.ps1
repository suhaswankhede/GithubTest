# Healthcare Portal Setup Script
# This script initializes the modular healthcare portal project

Write-Host "🏥 Healthcare Portal - Modular Architecture Setup" -ForegroundColor Green
Write-Host "=================================================" -ForegroundColor Green

# Check if .NET 8 SDK is installed
Write-Host "Checking .NET 8 SDK..." -ForegroundColor Yellow
$dotnetVersion = dotnet --version 2>$null
if ($dotnetVersion -and $dotnetVersion.StartsWith("8.")) {
    Write-Host "✅ .NET 8 SDK found: $dotnetVersion" -ForegroundColor Green
} else {
    Write-Host "❌ .NET 8 SDK not found. Please install .NET 8 SDK" -ForegroundColor Red
    exit 1
}

# Create project structure if not exists
Write-Host "Creating project structure..." -ForegroundColor Yellow

$directories = @(
    "src/HealthcarePortal.Core",
    "src/HealthcarePortal.Infrastructure", 
    "src/HealthcarePortal.Shared",
    "src/Modules/Member/HealthcarePortal.Member.Dashboard",
    "src/Modules/Member/HealthcarePortal.Member.Benefits",
    "src/Modules/Member/HealthcarePortal.Member.VirtualAssistant",
    "src/Modules/Member/HealthcarePortal.Member.Claims",
    "src/Modules/Member/HealthcarePortal.Member.FindCare",
    "src/Modules/Member/HealthcarePortal.Member.TrackRequests",
    "src/Modules/Member/HealthcarePortal.Member.CareJourney",
    "src/Modules/Member/HealthcarePortal.Member.Wellness",
    "src/Modules/Provider/HealthcarePortal.Provider.MemberLookup",
    "src/Modules/Provider/HealthcarePortal.Provider.Eligibility",
    "src/Modules/Provider/HealthcarePortal.Provider.Claims",
    "src/Modules/Provider/HealthcarePortal.Provider.Authorization",
    "src/Modules/Provider/HealthcarePortal.Provider.CareGaps",
    "src/Modules/Provider/HealthcarePortal.Provider.Messaging",
    "src/AI/HealthcarePortal.AI.Assistant",
    "src/AI/HealthcarePortal.AI.ClaimsExplanation",
    "src/AI/HealthcarePortal.AI.CoverageQA",
    "src/AI/HealthcarePortal.AI.RulesEngine",
    "src/Web/HealthcarePortal.API",
    "src/Web/HealthcarePortal.Web.Member",
    "src/Web/HealthcarePortal.Web.Provider",
    "tests/Unit",
    "tests/Integration",
    "docs",
    "scripts"
)

foreach ($dir in $directories) {
    if (!(Test-Path $dir)) {
        New-Item -ItemType Directory -Path $dir -Force | Out-Null
        Write-Host "📁 Created directory: $dir" -ForegroundColor Cyan
    }
}

# Create missing project files for modules not yet implemented
Write-Host "Creating remaining project files..." -ForegroundColor Yellow

$moduleProjects = @(
    @{
        Path = "src/HealthcarePortal.Infrastructure/HealthcarePortal.Infrastructure.csproj"
        Content = @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\HealthcarePortal.Core\HealthcarePortal.Core.csproj" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Abstractions" Version="8.0.0" />
  </ItemGroup>
</Project>
"@
    }
    @{
        Path = "src/Modules/Member/HealthcarePortal.Member.Benefits/HealthcarePortal.Member.Benefits.csproj"
        Content = @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\..\..\HealthcarePortal.Core\HealthcarePortal.Core.csproj" />
    <ProjectReference Include="..\..\..\HealthcarePortal.Shared\HealthcarePortal.Shared.csproj" />
  </ItemGroup>
</Project>
"@
    }
    @{
        Path = "src/AI/HealthcarePortal.AI.ClaimsExplanation/HealthcarePortal.AI.ClaimsExplanation.csproj"
        Content = @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\..\HealthcarePortal.Core\HealthcarePortal.Core.csproj" />
    <ProjectReference Include="..\..\HealthcarePortal.Shared\HealthcarePortal.Shared.csproj" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="Azure.AI.OpenAI" Version="1.0.0-beta.12" />
    <PackageReference Include="Microsoft.ML" Version="3.0.1" />
  </ItemGroup>
</Project>
"@
    }
)

foreach ($project in $moduleProjects) {
    if (!(Test-Path $project.Path)) {
        $project.Content | Out-File -FilePath $project.Path -Encoding UTF8
        Write-Host "📄 Created project file: $($project.Path)" -ForegroundColor Cyan
    }
}

# Restore NuGet packages
Write-Host "Restoring NuGet packages..." -ForegroundColor Yellow
try {
    dotnet restore HealthcarePortal.sln --verbosity quiet
    Write-Host "✅ NuGet packages restored successfully" -ForegroundColor Green
} catch {
    Write-Host "❌ Failed to restore NuGet packages" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
}

# Build the solution
Write-Host "Building solution..." -ForegroundColor Yellow
try {
    dotnet build HealthcarePortal.sln --configuration Debug --verbosity quiet --no-restore
    Write-Host "✅ Solution built successfully" -ForegroundColor Green
} catch {
    Write-Host "⚠️  Build completed with warnings or errors. Check output above." -ForegroundColor Yellow
}

# Create environment configuration files
Write-Host "Creating configuration files..." -ForegroundColor Yellow

$appsettingsContent = @"
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=HealthcarePortalDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "AzureOpenAI": {
    "Endpoint": "https://your-openai-resource.openai.azure.com/",
    "ApiKey": "your-api-key-here",
    "DeploymentName": "gpt-4o"
  },
  "AzureSpeech": {
    "SubscriptionKey": "your-speech-key-here",
    "Region": "eastus"
  },
  "JWT": {
    "SecretKey": "your-super-secret-key-here-must-be-at-least-32-characters",
    "Issuer": "HealthcarePortal",
    "Audience": "HealthcarePortal.API",
    "ExpirationMinutes": 60
  },
  "AllowedHosts": "*"
}
"@

$appsettingsPath = "src/Web/HealthcarePortal.API/appsettings.json"
if (!(Test-Path $appsettingsPath)) {
    $appsettingsContent | Out-File -FilePath $appsettingsPath -Encoding UTF8
    Write-Host "📄 Created appsettings.json" -ForegroundColor Cyan
}

# Create API Program.cs
$programContent = @"
using HealthcarePortal.AI.Assistant.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JWT");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["SecretKey"] ?? ""))
        };
    });

// Register AI services (mock implementations for now)
builder.Services.AddScoped<IHealthcareAssistantService, HealthcareAssistantService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
"@

$programPath = "src/Web/HealthcarePortal.API/Program.cs"
if (!(Test-Path $programPath)) {
    $programContent | Out-File -FilePath $programPath -Encoding UTF8
    Write-Host "📄 Created Program.cs" -ForegroundColor Cyan
}

# Create README
$readmeContent = @"
# Healthcare Portal - Modular Architecture

A comprehensive healthcare portal built with .NET 8, featuring modular architecture for both member and provider portals with integrated AI capabilities.

## 🚀 Quick Start

1. **Prerequisites**:
   - .NET 8 SDK
   - SQL Server (LocalDB for development)
   - Azure OpenAI API key (optional for AI features)

2. **Setup**:
   ```bash
   git clone <repository-url>
   cd healthcare-portal
   ./setup.ps1  # Run setup script
   ```

3. **Configuration**:
   - Update `appsettings.json` with your database connection string
   - Add Azure OpenAI API keys for AI features
   - Configure authentication settings

4. **Run**:
   ```bash
   dotnet run --project src/Web/HealthcarePortal.API
   ```

## 📚 Documentation

- [Architecture Overview](ARCHITECTURE.md)
- [API Documentation](http://localhost:5000/swagger) (when running)

## 🧩 Modules

### Member Portal
- 📊 Dashboard - Personalized member overview
- 💊 Benefits - Plan benefits and coverage
- 🤖 Virtual Assistant - AI-powered chat and voice
- 📋 Claims & EOB - Claims management
- 🔍 Find Care - Provider search and telehealth
- 📝 Track Requests - Appeals and authorizations
- 🎯 Care Journey - Health goals and care plans
- 🏃 Wellness - Activity tracking and rewards

### Provider Portal
- 👤 Member Lookup - Secure member search
- ✅ Eligibility - Real-time benefit verification
- 📄 Claims - Electronic claims submission
- 🔐 Authorization - Prior auth requests
- 📈 Care Gaps - HEDIS/Stars insights
- 💬 Messaging - Secure communication

### AI & Automation
- 🧠 AI Assistant - GPT-4o powered assistant
- 📝 Claims Explanation - NLU claims processing
- ❓ Coverage Q&A - Semantic search over benefits
- ⚙️ Rules Engine - Plan-specific workflows

## 🔧 Technology Stack

- **Backend**: .NET 8, ASP.NET Core Web API
- **AI**: Azure OpenAI, Azure Speech Services
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: JWT with OAuth 2.0
- **Documentation**: Swagger/OpenAPI

## 📝 License

This project is licensed under the MIT License.
"@

if (!(Test-Path "README.md")) {
    $readmeContent | Out-File -FilePath "README.md" -Encoding UTF8
    Write-Host "📄 Created README.md" -ForegroundColor Cyan
}

Write-Host ""
Write-Host "🎉 Healthcare Portal setup completed!" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Update appsettings.json with your configuration" -ForegroundColor White
Write-Host "2. Run: dotnet run --project src/Web/HealthcarePortal.API" -ForegroundColor White
Write-Host "3. Visit: https://localhost:5001/swagger for API documentation" -ForegroundColor White
Write-Host ""
Write-Host "For detailed architecture information, see ARCHITECTURE.md" -ForegroundColor Cyan
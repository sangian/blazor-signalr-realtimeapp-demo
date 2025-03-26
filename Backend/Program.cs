using Backend.Authentication;
using Backend.LiveTelemetry;
using Backend.TelemetryServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddSingleton<TelemetryServerUserManager>();
builder.Services.AddSingleton<StreamChannelManager>();
builder.Services.AddSingleton<JwtTokenGenerator>();

builder.Services.AddTransient<LiveTelemetryService>();
builder.Services.AddTransient<TelemetryServerService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var validIssuer = configuration["AuthenticationOptions:Issuer"];
    var validAudience = configuration["AuthenticationOptions:Audience"];
    var signingKey = configuration["AuthenticationOptions:SigningKey"];

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = validIssuer!,
        ValidAudience = validAudience!,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey!))
    };

    var signalRMainPath = configuration["SignalROptions:MainPath"];

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Look for the access token in the query string for SignalR
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            // Check if the request is for SignalR
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments(signalRMainPath))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});

// Add CORS policy to allow browser clients to connect - not recommended in production
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        In = ParameterLocation.Header,
        Description = "Enter your access token below. The 'Bearer' prefix will be added automatically."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    options.OperationFilter<SwaggerBearerTokenFilter>();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireCors("AllowAll");

var signalRMainPath = app.Configuration["SignalROptions:MainPath"];
var liveTelemetryHubPath = signalRMainPath + app.Configuration["LiveTelemetryHubOptions:HubPath"];
var telemetryServerHubPath = signalRMainPath + app.Configuration["TelemetryServerHubOptions:HubPath"];

app.MapHub<LiveTelemetryHub>(liveTelemetryHubPath).RequireCors("AllowAll");
app.MapHub<TelemetryServerHub>(telemetryServerHubPath).RequireCors("AllowAll");

app.Run();

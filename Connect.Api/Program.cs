using Connect.Api.HttpClients;
using Connect.Api.HttpClients.Handler;
using Connect.Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
// Add services to the container.
builder.Services.AddMemoryCache();
builder.Services.AddControllers();
var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(10));

builder.Services.AddScoped<IAzureAdTokenHandler, AzureAdTokenHandler>();
builder.Services.AddScoped<AuthHandler>();
builder.Services.Configure<AzureAdTokenConfig>(adToken =>
{
    adToken.ClientId = config["AzureAd:ClientId"];
    adToken.ClientSecret = config["AzureAd:ClientSecret"];
    adToken.TenantId = config["AzureAd:TenantId"];
    adToken.Scope = config["AzureAd:ClientId"];
});

builder.Services.AddHttpClient<IWeatherService, WeatherService>(c =>
{
    c.BaseAddress = new Uri("https://localhost:7095/api/WeatherForecast/");
})
    .AddHttpMessageHandler<AuthHandler>()
    .AddPolicyHandler(retryPolicy);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,

    };

    // for debugging auth events
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = (c) => {
            return Task.CompletedTask;
        },

        OnChallenge = (c) => {
            return Task.CompletedTask;
        },

        OnForbidden = (context) => {

            return Task.CompletedTask;
        },
        OnAuthenticationFailed = (context) =>
        {

            return Task.CompletedTask;
        }
    };
    options.Audience = "94a4ae78-8230-4031-ba66-0423e659907c";
    options.Authority = "https://login.microsoftonline.com/06e2775e-9d3d-49de-ad36-da82e295fa67/v2.0";
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

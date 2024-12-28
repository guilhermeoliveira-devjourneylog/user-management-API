using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers(); // Add controller support
builder.Services.AddEndpointsApiExplorer(); // Enable API exploration for Swagger
builder.Services.AddSwaggerGen(); // Add Swagger support for API documentation
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});
builder.Services.AddResponseCompression(); // Enable response compression
builder.Services.AddResponseCaching(); // Enable response caching

// Configure secure cookies globally
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always; // Prevent JavaScript access to cookies
    options.Secure = CookieSecurePolicy.Always; // Cookies transmitted only over HTTPS
});

// Optional: Add health checks for monitoring
builder.Services.AddHealthChecks();

// Add authentication and authorization services
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY"); // Retrieve the secret key from environment variables
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("JWT_SECRET_KEY is not configured in environment variables.");
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "your-issuer", // Use the environment variable or a default value
            ValidateAudience = true,
            ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "your-audience", // Use the environment variable or a default value
            ValidateLifetime = true, // Validate token expiration
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)) // Use the secret key
        };
    });

builder.Services.AddAuthorization(); // Add policy-based authorization

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // Redirect HTTP requests to HTTPS
app.UseCors("AllowAll"); // Apply CORS policy
app.UseResponseCompression(); // Compress responses for better performance
app.UseResponseCaching(); // Cache responses where applicable
app.UseCookiePolicy(); // Secure cookies globally
app.UseAuthentication(); // Ensure authentication happens early
app.UseAuthorization(); // Ensure authorization happens after authentication

// Custom middleware
app.UseMiddleware<UserManagementAPI.Middleware.ErrorHandlingMiddleware>();
app.UseMiddleware<UserManagementAPI.Middleware.AuthenticationMiddleware>();

// Optional: Logging middleware (only in development for performance)
if (app.Environment.IsDevelopment())
{
    app.UseMiddleware<UserManagementAPI.Middleware.LoggingMiddleware>();
}

// Map controllers
app.MapControllers();

// Map health check endpoint
app.MapHealthChecks("/health");

app.Run();

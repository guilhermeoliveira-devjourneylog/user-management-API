using Microsoft.OpenApi.Models;
using UserManagementAPI.Interfaces;
using UserManagementAPI.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure JWT settings
var jwtSettings = builder.Configuration.GetSection("JWT");
var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? jwtSettings["SecretKey"];
var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? jwtSettings["Issuer"];
var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? jwtSettings["Audience"];

if (string.IsNullOrWhiteSpace(secretKey))
{
    throw new InvalidOperationException("JWT_SECRET_KEY is not configured or is empty.");
}

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "User Management API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer <token>'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});
builder.Services.AddResponseCompression();
builder.Services.AddResponseCaching();
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
    options.Secure = CookieSecurePolicy.Always;
});

// Add Authentication and Authorization services
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });
builder.Services.AddAuthorization();

// Dependency Injection
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "User Management API v1");
        options.RoutePrefix = string.Empty; // Configura o Swagger na raiz "/"
    });
}

// Middleware for error handling
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500; // Internal Server Error
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{\"error\": \"An internal server error occurred.\"}");
    });
});

// Middleware for logging requests and responses
app.Use(async (context, next) =>
{
    Console.WriteLine($"HTTP {context.Request.Method} - {context.Request.Path}");
    await next();
    Console.WriteLine($"Response Status Code: {context.Response.StatusCode}");
});

// Common middleware
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseResponseCompression();
app.UseResponseCaching();
app.UseCookiePolicy();

// Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map controllers and health checks
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

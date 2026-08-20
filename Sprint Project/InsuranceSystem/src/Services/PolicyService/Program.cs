using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PolicyService.Data;
using PolicyService.Messaging;
using PolicyService.Services;
using SharedLibrary.Messaging;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Policy Service API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your JWT token here."
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddDbContext<PolicyDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, ValidateAudience = true, ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddCors(opts =>
    opts.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddScoped<IPolicyService, PolicyManagementService>();
builder.Services.AddSingleton<IRabbitMQPublisher>(sp =>
    new RabbitMQPublisher(builder.Configuration["RabbitMQ:Host"] ?? "localhost"));
builder.Services.AddHostedService<PaymentCompletedConsumer>();
builder.Services.AddHostedService<ClaimApprovedConsumer>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Policy Service v1"));
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PolicyDbContext>();
    db.Database.EnsureCreated();

    // Safely add the new column if it doesn't exist yet
    try 
    {
        db.Database.ExecuteSqlRaw(@"
            IF COL_LENGTH('CustomerPolicies', 'RemainingCoverageAmount') IS NULL
            BEGIN
                ALTER TABLE CustomerPolicies ADD RemainingCoverageAmount decimal(18,2) NOT NULL DEFAULT 0;
                
                UPDATE cp
                SET cp.RemainingCoverageAmount = p.CoverageAmount
                FROM CustomerPolicies cp
                INNER JOIN Policies p ON cp.PolicyId = p.PolicyId
                WHERE cp.RemainingCoverageAmount = 0;
            END
        ");
    } 
    catch { }
}

app.Run();

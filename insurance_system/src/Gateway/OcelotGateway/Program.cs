using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();

// Aggregated Swagger UI on Gateway
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Insurance System Microservices API Gateway", Version = "v1" });
});

var app = builder.Build();

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("http://localhost:5001/swagger/v1/swagger.json", "Identity Microservice API (:5001)");
        c.SwaggerEndpoint("http://localhost:5002/swagger/v1/swagger.json", "Policy Microservice API (:5002)");
        c.SwaggerEndpoint("http://localhost:5003/swagger/v1/swagger.json", "Ticket Microservice API (:5003)");
        c.RoutePrefix = "swagger";
    });
}

app.UseRouting();

await app.UseOcelot();

app.Run("http://localhost:5000");

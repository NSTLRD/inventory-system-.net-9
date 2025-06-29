using System.Text;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Inventory.Api.Common.Interfaces;
using Inventory.Api.Application.Events;
using Inventory.Api.Application.Commands.Handlers;

using Inventory.Api.Infrastructure.Configuration;
using Inventory.Api.Infrastructure.Data;
using Inventory.Api.Infrastructure.Messaging;
using Inventory.Api.Infrastructure.Repositories;
using Inventory.Api.Middleware;
using Inventory.Api.Application.Handlers;
using Inventory.Api.Application.Common.Interfaces;



var builder = WebApplication.CreateBuilder(args);

// 1) Serilog
builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

// 2) EF Core + UoW + repos genéricas
builder.Services.AddDbContext<InventoryDbContext>(opts =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddMediatR(typeof(AdjustInventoryHandler).Assembly);


builder.Services.AddScoped<IUnitOfWork, InventoryDbContext>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(EfRepository<>));


// 3) Identity + JWT
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<InventoryDbContext>()
    .AddDefaultTokenProviders();

builder.Services
  .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(opts => {
    opts.RequireHttpsMetadata = false;
    opts.SaveToken = true;
    opts.TokenValidationParameters = new TokenValidationParameters {
      ValidIssuer           = builder.Configuration["Jwt:Issuer"],
      ValidAudience         = builder.Configuration["Jwt:Audience"],
      IssuerSigningKey      = new SymmetricSecurityKey(
        System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
      ValidateIssuer        = true,
      ValidateAudience      = true,
      ValidateIssuerSigningKey = true,
      ValidateLifetime      = true
    };
  });
builder.Services.AddAuthorization();

// 4) MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(ProductCreatedIntegrationHandler).Assembly);
});
builder.Services.AddMemoryCache();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// 5) KafkaSettings
builder.Services.Configure<KafkaSettings>(
    builder.Configuration.GetSection("KafkaSettings"));

// 6) Bus interno + handler + consumer
// Producer, bus interno y consumer
// builder.Services.AddSingleton<KafkaEventBus>();                  
builder.Services.AddSingleton<IEventBus, InMemoryEventBus>();      // bus in-mem
builder.Services.AddHostedService<KafkaConsumer>();               // único consumer

// Integration Handlers
builder.Services.AddScoped<IIntegrationEventHandler<ProductCreatedEvent>, ProductCreatedIntegrationHandler>();
builder.Services.AddScoped<IIntegrationEventHandler<ProductUpdatedEvent>, ProductUpdatedIntegrationHandler>();


// 7) MVC & OpenAPI
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Inventory API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer {token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
    {
      new OpenApiSecurityScheme {
        Reference = new OpenApiReference {
          Type = ReferenceType.SecurityScheme,
          Id   = "Bearer"
        }
      },
      new List<string>()
    }
  });
});



var app = builder.Build();

// Auto-migrate
using(var scope = app.Services.CreateScope()) {
  var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
  db.Database.Migrate();
}

// Suscribir el handler al bus interno
var bus = app.Services.GetRequiredService<IEventBus>();
bus.Subscribe<ProductCreatedEvent, ProductCreatedIntegrationHandler>();
bus.Subscribe<ProductUpdatedEvent, ProductUpdatedIntegrationHandler>();

// Pipeline
app.UseSerilogRequestLogging();
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json","Inventory API v1"));

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

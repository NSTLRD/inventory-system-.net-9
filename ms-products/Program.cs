using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Products.Api.Common.Interfaces;
using Products.Api.Infrastruture.Data;
using Products.Api.Infrastruture.Repositories;
using Products.Api.Middleware;
using Serilog;
using Products.Api.Infrastructure.Messaging;

var builder = WebApplication.CreateBuilder(args);

//Serilog
builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

// EF + DbContext
builder.Services.AddDbContext<ProductDbContext>(opts =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

            
//Identity (UserManager, RoleManager, SignInManager, stores)
builder.Services.AddIdentity<IdentityUser, IdentityRole>(opts =>
    {
        opts.Password.RequireDigit           = true;
        opts.Password.RequireNonAlphanumeric = false;
        opts.Password.RequiredLength         = 6;
    })
    .AddEntityFrameworkStores<ProductDbContext>()
    .AddDefaultTokenProviders();

// Authentication & Authorization
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken            = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey         = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
            ),
            ValidateIssuer   = true,
            ValidIssuer      = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience    = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true
        };
    })
    .Services   
    .AddAuthorization();

//Application services (UoW, repos, MediatR, validators, Kafka, cache)
builder.Services.AddScoped<IUnitOfWork, ProductDbContext>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(EfRepository<>));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddSingleton<KafkaProducer>();
builder.Services.AddMemoryCache();

//HttpClient
builder.Services.AddHttpClient();
builder.Services.AddScoped<Products.Api.Common.Interfaces.ICurrencyConversionService, Products.Api.Infrastructure.Services.CurrencyConversionService>();

//Swagger + JWT support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Products API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = SecuritySchemeType.ApiKey,
        Scheme       = "Bearer",
        BearerFormat = "JWT",
        In           = ParameterLocation.Header,
        Description  = "Enter ‘Bearer {token}’"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

//MVC + validation
builder.Services.AddControllers().AddFluentValidation();

// ──────────────────────────────────────────────────────────────────────────

var app = builder.Build();

//Middleware pipeline
app.UseSerilogRequestLogging();
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Products API v1");
    c.RoutePrefix = "swagger";
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//Apply EF migrations at startup
using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
db.Database.Migrate();

//Ensure Kafka topic is created at startup
var kafkaProducer = scope.ServiceProvider.GetRequiredService<KafkaProducer>();

app.Run();

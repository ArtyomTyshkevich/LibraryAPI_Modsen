using Library.Data.Context;
using Library.Data.Repositories.UnitOfWork;
using Library.Domain.Entities;
using Library.Domain.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using FluentValidation.AspNetCore;
using System.Text;
using Library.WebAPI.Middlewares;
using Library.Application.Mappers;
using MassTransit;
using Library.Data.Consumers;
using Library.Data.Services;
using Library.Application.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IImageCacheService, ImageCacheService>();
builder.Services.AddScoped<ImageCacheService, ImageCacheService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddStackExchangeRedisCache(option =>
    option.Configuration = builder.Configuration.GetConnectionString("Cache"));
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<BookRentConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMqSettings:Host"], c =>
        {
            c.Username(builder.Configuration["RabbitMqSettings:Username"]!);
            c.Password(builder.Configuration["RabbitMqSettings:Password"]!);
        });

        cfg.ReceiveEndpoint(builder.Configuration["RabbitMqSettings:QueueName"]!, e =>
        {
            e.ConfigureConsumer<BookRentConsumer>(context);
        });

        cfg.ClearSerialization();
        cfg.UseRawJsonSerializer();
        cfg.ConfigureEndpoints(context);
        cfg.UseFilter(new DelayMiddleware());
    });
});
builder.Services.AddDbContext<LibraryDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthentication(opt => {
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"]!,
            ValidAudience = builder.Configuration["Jwt:Audience"]!,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .Build();

    options.AddPolicy("AdminOnly", policy =>
    {
        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
        policy.RequireRole("Admin");
    });
    options.AddPolicy("ModerAndHigher", policy =>
    {
        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
        policy.RequireRole("Moder", "Admin");
    });
    options.AddPolicy("UserPolicy", policy =>
    {
        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
        policy.RequireRole("User");
    });
});

builder.Services.AddIdentity<User, IdentityRole<long>>()
    .AddEntityFrameworkStores<LibraryDbContext>()
    .AddUserManager<UserManager<User>>()
    .AddSignInManager<SignInManager<User>>();

builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Pathnostics", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[]{}
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCustomExceptionHandler();
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

Seed.InitializeRoles(app.Services).Wait();
app.MapControllers();

app.Run();

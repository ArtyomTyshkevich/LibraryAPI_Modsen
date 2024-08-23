using FluentValidation.AspNetCore;
using FluentValidation;
using Library.Application.Mappers;
using Library.Data.Context;
using Library.Data.Repositories.UnitOfWork;
using Library.Domain.DTOs;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Library.Domain.Services;
using Library.Domain.Validators;
using Library.WebAPI.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure services
ConfigureServices(builder.Services, builder.Configuration);

// Build the app
var app = builder.Build();

// Configure the app
ConfigureApp(app);

app.Run();

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.AddControllers();

    services.AddScoped<IBookService, BookService>();
    services.AddScoped<IUnitOfWork, UnitOfWork>();
    services.AddScoped<ITokenService, TokenService>();
    services.AddAutoMapper(typeof(MappingProfile));
    services.AddScoped<IValidator<Massage>, MassageValidator>();
    services.AddScoped<IValidator<User>, UserValidator>();
    services.AddScoped<IValidator<Book>, BookValidator>();
    services.AddScoped<IValidator<Author>, AuthorValidator>();
    services.AddScoped<IValidator<UserDTO>, UserDTOValidator>();
    services.AddScoped<IValidator<BookDTO>, BookDTOValidator>();
    services.AddFluentValidationAutoValidation();


    services.AddDbContext<LibraryDbContext>(opt =>
        opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

    services.AddAuthentication(opt => {
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
            ValidIssuer = configuration["Jwt:Issuer"]!,
            ValidAudience = configuration["Jwt:Audience"]!,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!))
        };
    });
    services.AddAuthorization(options =>
    {
        options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .Build();

        options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
        options.AddPolicy("ModerAndHigher", policy => policy.RequireRole("Moder", "Admin"));
        options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
    });
    services.AddIdentity<User, IdentityRole<long>>()
        .AddEntityFrameworkStores<LibraryDbContext>()
        .AddUserManager<UserManager<User>>()
        .AddSignInManager<SignInManager<User>>();
   
    services.AddSwaggerGen(option =>
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
}

void ConfigureApp(WebApplication app)
{
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
}
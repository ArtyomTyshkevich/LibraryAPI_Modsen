using FluentValidation.AspNetCore;
using Library.Application.Interfaces;
using Library.Application.Mappers;
using Library.Data.Repositories.UnitOfWork;
using Library.WebAPI.Middlewares;
using Library.WebAPI.Setups;
using MediatR;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddMediatR(Assembly.Load("Library.Data"));
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.ConfigureIdentity();
builder.Services.ConfigureSwagger();
builder.Services.ConfigureDatabase(builder.Configuration);
builder.Services.ConfigureCache(builder.Configuration);

builder.Services.ConfigureMassTransit(builder.Configuration);

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
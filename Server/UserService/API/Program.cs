using Microsoft.EntityFrameworkCore;
using UserService.Application.Interfaces.Repositories;
using UserService.Core.Interfaces.Services;
using UserService.Infrastructure.Repositories;
using UserService.Application.Interfaces.Services;
using UserService.Application.Services;
using UserService.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using UserService.Core.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IKafkaProducerService, KafkaProducerService>();
builder.Services.AddSingleton<KafkaConsumerService>();

builder.Services.AddControllers();

builder.Services.AddScoped<IUserService, UserService.Application.Services.UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher<UserEntity>, PasswordHasher<UserEntity>>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// База данных
string dbConnection = builder.Configuration.GetConnectionString(nameof(UserDbContext));
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(dbConnection));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();

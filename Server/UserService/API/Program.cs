using Microsoft.EntityFrameworkCore;
using UserService.Infrastructure.Context;

var builder = WebApplication.CreateBuilder(args);

// База данных
string dbConnection = builder.Configuration.GetConnectionString(nameof(UserDbContext));
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(dbConnection));

var app = builder.Build();

app.Run();

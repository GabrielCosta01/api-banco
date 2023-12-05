using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using api_banco.Middleware;
using api_banco.Persistence;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

string secretKey = configuration.GetSection("AppSettings:SecretKey").Value;

if (string.IsNullOrEmpty(secretKey))
{
    secretKey = Environment.GetEnvironmentVariable("SECRET_KEY");
}

if (string.IsNullOrEmpty(secretKey))
{
    throw new InvalidOperationException("A chave secreta não está configurada.");
}

// Add services to the container.
builder.Services.AddSingleton<UserDbContext>();
builder.Services.AddScoped<TokenService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

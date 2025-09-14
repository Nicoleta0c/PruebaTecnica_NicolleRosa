using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Application.Interfaces;
using PruebaTecnica.Application.Services;  
using PruebaTecnica.Domain.Entities;     
using PruebaTecnica.Infrastructure.Data;
using PruebaTecnica.Infrastructure.Repositories;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = "Data Source=PruebaTecnica.db"; 
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

// Dependency Injection
builder.Services.AddScoped<IUserRepository, UserRepository>();


builder.Services.AddScoped<UserService>();  
builder.Services.AddScoped<BetService>();  

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

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
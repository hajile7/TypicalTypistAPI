using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using TypicalTypistAPI;
using TypicalTypistAPI.Models;
using TypicalTypistAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the DI container.

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy =>
        {
            policy.WithOrigins("http://localhost:5263").AllowAnyMethod().AllowAnyHeader();
        });
});

// Add DbContext here so we can inject it elsewhere

builder.Services.AddDbContext<TypicalTypistDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString(TypicalTypistAPI.Secret.url)));

builder.Services.AddSingleton<WordCacheService>();
builder.Services.AddSingleton<PasswordService>();
builder.Services.AddScoped<TypicalTypistDbContext>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Load wordCache on startup

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TypicalTypistDbContext>();
    var wordCache = scope.ServiceProvider.GetRequiredService<WordCacheService>();
    await wordCache.LoadWordsAsync(dbContext);
}

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseAuthorization();

app.MapControllers();

app.Run();

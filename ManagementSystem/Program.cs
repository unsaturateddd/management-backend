using ManagementSystem;
using ManagementSystem.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Регистрация сервисов ---

// Настраиваем CORS один раз
builder.Services.AddCors(options =>
{
    options.AddPolicy("VercelPolicy", policy =>
    {
        // На время тестов можно оставить AllowAnyOrigin, 
        // но для защиты на Vercel лучше потом вписать конкретный URL
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ReleaseService>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=release.db"));

var app = builder.Build();

// --- 2. Конфигурация конвейера (Middleware) ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Включаем CORS
app.UseCors("VercelPolicy");

// Если у тебя нет контроллеров с [Authorize], это можно пока закомментировать
app.UseAuthorization();

app.MapControllers();

// --- 3. Инициализация Базы Данных ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();

    context.Database.EnsureCreated();

    if (!context.Projects.Any())
    {
        context.Projects.Add(new ManagementSystem.Models.Project
        {
            Id = Guid.NewGuid(),
            Name = "Nursultan Client",
            GitUrl = "https://github.com/nursultan",
            ProjectType = "Desktop Client"
        });
        context.SaveChanges();
    }
}

// --- 4. Запуск (ТОЛЬКО ОДИН РАЗ) ---
app.Run();
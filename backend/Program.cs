using backend.Services;
using Telegram.Bot;
using backend.Data;
using Microsoft.EntityFrameworkCore;
using backend.Models.Entities;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register services
builder.Services.AddSingleton<ITelegramService, TelegramService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowAll");
}


app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

// Initialize data
using (var scope = app.Services.CreateScope())
{
    var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
    await productService.LoadDataAsync();
}


// Применяем миграции
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();  // ← ЭТО РАБОТАЕТ БЕЗ dotnet ef
}


// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { 
    status = "Healthy", 
    timestamp = DateTime.UtcNow,
    service = "TubeCompanyApp Backend"
}));

// Test endpoint for bot
app.MapGet("/test-bot", async (IConfiguration configuration) =>
{
    var botToken = configuration["Telegram:BotToken"];
    if (string.IsNullOrEmpty(botToken)) 
        return Results.Json(new { error = "Bot token not configured" });
    
    try
    {
        var botClient = new Telegram.Bot.TelegramBotClient(botToken);
        var me = await botClient.GetMe();
        return Results.Json(new { 
            success = true, 
            botUsername = me.Username,
            botName = me.FirstName
        });
    }
    catch (Exception ex)
    {
        return Results.Json(new { error = ex.Message });
    }
});

// Manual webhook setup endpoint
app.MapGet("/setup-webhook", async (IConfiguration configuration) =>
{
    var botToken = configuration["Telegram:BotToken"];
    var baseUrl = configuration["App:BaseUrl"];
    
    if (string.IsNullOrEmpty(botToken) || string.IsNullOrEmpty(baseUrl))
        return Results.Json(new { error = "Bot token or base URL not configured" });
    
    var webhookUrl = $"{baseUrl}/api/telegram/webhook";
    
    try
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(
            $"https://api.telegram.org/bot{botToken}/setWebhook?url={webhookUrl}");
        
        var content = await response.Content.ReadAsStringAsync();
        
        if (response.IsSuccessStatusCode)
        {
            return Results.Json(new { 
                success = true, 
                message = "Webhook set successfully",
                webhookUrl = webhookUrl,
                response = content
            });
        }
        else
        {
            return Results.Json(new { 
                error = "Failed to set webhook",
                statusCode = response.StatusCode,
                response = content
            });
        }
    }
    catch (Exception ex)
    {
        return Results.Json(new { error = ex.Message });
    }
});

// Check webhook info
app.MapGet("/webhook-info", async (IConfiguration configuration) =>
{
    var botToken = configuration["Telegram:BotToken"];

    if (string.IsNullOrEmpty(botToken))
        return Results.Json(new { error = "Bot token not configured" });

    System.Console.Write(botToken);
    try
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(
            $"https://api.telegram.org/bot{botToken}/getWebhookInfo");

        var content = await response.Content.ReadAsStringAsync();

        return Results.Json(new
        {
            success = true,
            response = content
        });
    }
    catch (Exception ex)
    {
        return Results.Json(new { error = ex.Message });
    }
});

app.Run();
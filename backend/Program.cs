using backend.Services;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS for React frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000", 
                "https://localhost:3000",
                "https://tiera-pivotal-marketta.ngrok-free.dev"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
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
    app.UseCors("ReactFrontend");
}

app.UseRouting();
app.UseAuthorization();

app.MapControllers();

// Initialize data
using (var scope = app.Services.CreateScope())
{
    var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
    await productService.LoadDataAsync();
}

// ========== API ENDPOINTS ==========

// Health check
app.MapGet("/health", () => Results.Ok(new { 
    status = "Healthy", 
    timestamp = DateTime.UtcNow,
    service = "TubeCompanyApp Backend"
}));

// Test bot connection
app.MapGet("/test-bot", async (IConfiguration configuration) =>
{
    var botToken = configuration["Telegram:BotToken"];
    if (string.IsNullOrEmpty(botToken)) 
        return Results.Json(new { error = "Bot token not configured" });
    
    try
    {
        var botClient = new TelegramBotClient(botToken);
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

// Setup webhook manually
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
        
        return Results.Json(new { 
            success = response.IsSuccessStatusCode,
            statusCode = (int)response.StatusCode,
            webhookUrl = webhookUrl,
            response = content
        });
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
    
    try
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(
            $"https://api.telegram.org/bot{botToken}/getWebhookInfo");
        
        var content = await response.Content.ReadAsStringAsync();
        
        return Results.Json(new { 
            success = true,
            response = content
        });
    }
    catch (Exception ex)
    {
        return Results.Json(new { error = ex.Message });
    }
});

// Delete webhook
app.MapGet("/delete-webhook", async (IConfiguration configuration) =>
{
    var botToken = configuration["Telegram:BotToken"];
    
    if (string.IsNullOrEmpty(botToken))
        return Results.Json(new { error = "Bot token not configured" });
    
    try
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(
            $"https://api.telegram.org/bot{botToken}/deleteWebhook");
        
        var content = await response.Content.ReadAsStringAsync();
        
        return Results.Json(new { 
            success = response.IsSuccessStatusCode,
            response = content
        });
    }
    catch (Exception ex)
    {
        return Results.Json(new { error = ex.Message });
    }
});

// Full diagnostics
app.MapGet("/debug", async (IConfiguration configuration) =>
{
    var botToken = configuration["Telegram:BotToken"];
    var baseUrl = configuration["App:BaseUrl"];
    var results = new List<string>();
    
    // 1. Check token
    if (string.IsNullOrEmpty(botToken))
    {
        results.Add("❌ Токен бота не настроен");
        return Results.Json(new { errors = results });
    }
    results.Add("✅ Токен бота настроен");
    
    // 2. Check base URL
    if (string.IsNullOrEmpty(baseUrl))
    {
        results.Add("❌ Base URL не настроен");
    }
    else
    {
        results.Add($"✅ Base URL: {baseUrl}");
    }
    
    // 3. Check Telegram API connection
    try
    {
        var botClient = new TelegramBotClient(botToken);
        var me = await botClient.GetMe();
        results.Add($"✅ Бот подключен: @{me.Username} ({me.FirstName})");
    }
    catch (Exception ex)
    {
        results.Add($"❌ Ошибка подключения к Telegram: {ex.Message}");
    }
    
    // 4. Check webhook
    try
    {
        using var client = new HttpClient();
        var response = await client.GetStringAsync($"https://api.telegram.org/bot{botToken}/getWebhookInfo");
        results.Add($"✅ Webhook info: {response}");
    }
    catch (Exception ex)
    {
        results.Add($"❌ Ошибка проверки webhook: {ex.Message}");
    }
    
    return Results.Json(new { 
        diagnostics = results,
        timestamp = DateTime.UtcNow 
    });
});

app.Run();
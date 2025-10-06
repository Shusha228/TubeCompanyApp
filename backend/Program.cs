using backend.Services;
using Telegram.Bot;
using backend.Data;
using Microsoft.EntityFrameworkCore;
using backend.Models.Entities;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Tube Company API", 
        Version = "v1",
        Description = "API для управления ценами на трубы"
    });
    
    c.EnableAnnotations();
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:8080")
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});
 
// Register services
builder.Services.AddSingleton<ITelegramService, TelegramService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<NomenclatureImporter>();
builder.Services.AddScoped<PriceImporter>();
builder.Services.AddScoped<StockImporter>();
builder.Services.AddScoped<ProductTypeImporter>();
builder.Services.AddScoped<RemnantImporter>();
builder.Services.AddScoped<ITelegramNotificationService, TelegramNotificationService>();

builder.Services.AddSingleton<ITelegramBotClient>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var botToken = configuration["Telegram:BotToken"];
    
    if (string.IsNullOrEmpty(botToken))
        throw new InvalidOperationException("Telegram BotToken is not configured");
    
    return new TelegramBotClient(botToken);
});

var app = builder.Build();

app.MapGet("/setup-admin", async (IConfiguration configuration, ITelegramBotClient botClient) =>
{
    var adminChatIds = configuration.GetSection("Telegram:AdminChatIds").Get<List<long>>() ?? new List<long>();
    
    return Results.Json(new {
        currentAdmins = adminChatIds,
        instruction = "Отправьте боту команду /getchatid чтобы получить ваш Chat ID"
    });
});

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


if (app.Environment.IsDevelopment())
{
    Console.WriteLine("=== Starting data import process ===");
    Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
    
    using var scope = app.Services.CreateScope();
    Console.WriteLine("Service scope created");
    
    try
    {
        // 1. Импорт типов продуктов
        var productTypeImporter = scope.ServiceProvider.GetRequiredService<ProductTypeImporter>();
        Console.WriteLine("ProductTypeImporter resolved successfully");
        
        var productTypeJsonPath = Path.Combine(app.Environment.ContentRootPath, "Data_Json", "types.json");
        Console.WriteLine($"Looking for product types JSON file at: {productTypeJsonPath}");
        
        if (File.Exists(productTypeJsonPath))
        {
            Console.WriteLine("Product types JSON file found, starting import...");
            try
            {
                await productTypeImporter.ImportProductTypesFromJsonAsync(productTypeJsonPath);
                Console.WriteLine("✅ Product types data imported successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Product types data import failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
        else
        {
            Console.WriteLine($"❌ Product types JSON file not found at: {productTypeJsonPath}");
        }

        // 2. Импорт складов
        var stockImporter = scope.ServiceProvider.GetRequiredService<StockImporter>();
        Console.WriteLine("StockImporter resolved successfully");
        
        var stockJsonPath = Path.Combine(app.Environment.ContentRootPath, "Data_Json", "stocks.json");
        Console.WriteLine($"Looking for stocks JSON file at: {stockJsonPath}");
        
        if (File.Exists(stockJsonPath))
        {
            Console.WriteLine("Stocks JSON file found, starting import...");
            try
            {
                await stockImporter.ImportStocksFromJsonAsync(stockJsonPath);
                Console.WriteLine("✅ Stocks data imported successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Stocks data import failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
        else
        {
            Console.WriteLine($"❌ Stocks JSON file not found at: {stockJsonPath}");
        }

        // 3. Импорт номенклатуры
        var nomenclatureImporter = scope.ServiceProvider.GetRequiredService<NomenclatureImporter>();
        Console.WriteLine("NomenclatureImporter resolved successfully");
        
        var nomenclatureJsonPath = Path.Combine(app.Environment.ContentRootPath, "Data_Json", "nomenclature.json");
        Console.WriteLine($"Looking for nomenclature JSON file at: {nomenclatureJsonPath}");
        
        if (File.Exists(nomenclatureJsonPath))
        {
            Console.WriteLine("Nomenclature JSON file found, starting import...");
            try
            {
                await nomenclatureImporter.ImportNomenclatureFromJsonAsync(nomenclatureJsonPath);
                Console.WriteLine("✅ Nomenclature data imported successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Nomenclature data import failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
        else
        {
            Console.WriteLine($"❌ Nomenclature JSON file not found at: {nomenclatureJsonPath}");
        }

        // 4. Импорт остатков (должен быть после номенклатуры и складов)
        var remnantImporter = scope.ServiceProvider.GetRequiredService<RemnantImporter>();
        Console.WriteLine("RemnantImporter resolved successfully");
        
        var remnantJsonPath = Path.Combine(app.Environment.ContentRootPath, "Data_Json", "remnants.json");
        Console.WriteLine($"Looking for remnants JSON file at: {remnantJsonPath}");
        
        if (File.Exists(remnantJsonPath))
        {
            Console.WriteLine("Remnants JSON file found, starting import...");
            try
            {
                await remnantImporter.ImportRemnantsFromJsonAsync(remnantJsonPath);
                Console.WriteLine("✅ Remnants data imported successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Remnants data import failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
        else
        {
            Console.WriteLine($"❌ Remnants JSON file not found at: {remnantJsonPath}");
        }

        // 5. Импорт цен (должен быть последним)
        var priceImporter = scope.ServiceProvider.GetRequiredService<PriceImporter>();
        Console.WriteLine("PriceImporter resolved successfully");
        
        var priceJsonPath = Path.Combine(app.Environment.ContentRootPath, "Data_Json", "prices.json");
        Console.WriteLine($"Looking for prices JSON file at: {priceJsonPath}");
        
        if (File.Exists(priceJsonPath))
        {
            Console.WriteLine("Prices JSON file found, starting import...");
            try
            {
                await priceImporter.ImportPricesFromJsonAsync(priceJsonPath);
                Console.WriteLine("✅ Prices data imported successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Prices data import failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
        else
        {
            Console.WriteLine($"❌ Prices JSON file not found at: {priceJsonPath}");
        }

        // Покажем какие файлы есть в папке Data_Json
        var dataJsonPath = Path.Combine(app.Environment.ContentRootPath, "Data_Json");
        if (Directory.Exists(dataJsonPath))
        {
            var files = Directory.GetFiles(dataJsonPath);
            Console.WriteLine($"Files in Data_Json directory: {string.Join(", ", files)}");
        }
        else
        {
            Console.WriteLine("Data_Json directory does not exist");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Failed to resolve services: {ex.Message}");
    }
    
    Console.WriteLine("=== Data import process completed ===");
}
app.Run();

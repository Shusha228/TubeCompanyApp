using System.Text.Json;
using backend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using backend.Data;

namespace backend.Services
{
    public class PriceImporter
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PriceImporter> _logger;

        public PriceImporter(ApplicationDbContext context, ILogger<PriceImporter> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task ImportPricesFromJsonAsync(string jsonFilePath)
        {
            try
            {
                _logger.LogInformation("Starting prices import from {FilePath}", jsonFilePath);

                // Чтение JSON файла
                var jsonString = await File.ReadAllTextAsync(jsonFilePath);
                _logger.LogInformation("JSON file read successfully, length: {Length} characters", jsonString.Length);
                
                // Десериализация JSON
                var jsonData = JsonSerializer.Deserialize<PriceJsonRoot>(jsonString);
                
                if (jsonData?.ArrayOfPricesEl == null)
                {
                    throw new Exception("Invalid JSON format or empty data");
                }

                _logger.LogInformation("Found {Count} price records in JSON", jsonData.ArrayOfPricesEl.Count);

                // Проверяем существование связанных сущностей
                await ValidateRelatedEntities(jsonData.ArrayOfPricesEl);

                // Преобразование данных в сущности
                var priceEntities = new List<Price>();
                foreach (var item in jsonData.ArrayOfPricesEl)
                {
                    try
                    {
                        var price = new Price
                        {
                            ID = int.Parse(item.ID),
                            IDStock = item.IDStock, // Теперь просто присваиваем строку (GUID)
                            PriceT = item.PriceT,
                            PriceLimitT1 = item.PriceLimitT1 == 0 ? null : (decimal?)item.PriceLimitT1,
                            PriceT1 = item.PriceT1 == 0 ? null : (decimal?)item.PriceT1,
                            PriceLimitT2 = item.PriceLimitT2 == 0 ? null : (decimal?)item.PriceLimitT2,
                            PriceT2 = item.PriceT2 == 0 ? null : (decimal?)item.PriceT2,
                            PriceM = item.PriceM,
                            PriceLimitM1 = item.PriceLimitM1 == 0 ? null : (decimal?)item.PriceLimitM1,
                            PriceM1 = item.PriceM1 == 0 ? null : (decimal?)item.PriceM1,
                            PriceLimitM2 = item.PriceLimitM2 == 0 ? null : (decimal?)item.PriceLimitM2,
                            PriceM2 = item.PriceM2 == 0 ? null : (decimal?)item.PriceM2,
                            NDS = item.NDS
                        };
                        priceEntities.Add(price);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("Failed to parse price item with ID {ID}: {Error}", item.ID, ex.Message);
                    }
                }

                _logger.LogInformation("Successfully parsed {Count} price entities", priceEntities.Count);

                // Очистка существующих данных
                _logger.LogInformation("Clearing existing prices...");
                _context.Prices.RemoveRange(_context.Prices);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Existing prices cleared");

                // Добавление новых данных
                _logger.LogInformation("Adding new prices...");
                await _context.Prices.AddRangeAsync(priceEntities);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully imported {Count} price records", priceEntities.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing price data from {FilePath}", jsonFilePath);
                throw;
            }
        }

        private async Task ValidateRelatedEntities(List<PriceJsonItem> items)
        {
            // Получаем уникальные ID номенклатур и складов
            var nomenclatureIds = items.Select(x => int.Parse(x.ID)).Distinct().ToList();
            var stockGuids = items.Select(x => x.IDStock).Distinct().ToList();

            _logger.LogInformation("Validating {NomenclatureCount} nomenclatures and {StockCount} stocks", 
                nomenclatureIds.Count, stockGuids.Count);

            // Проверяем существование номенклатур
            var existingNomenclatures = await _context.Nomenclatures
                .Where(n => nomenclatureIds.Contains(n.ID))
                .Select(n => n.ID)
                .ToListAsync();

            var missingNomenclatures = nomenclatureIds.Except(existingNomenclatures).ToList();
            if (missingNomenclatures.Any())
            {
                _logger.LogWarning("Missing nomenclatures: {MissingIds}", string.Join(", ", missingNomenclatures));
            }

            // Проверяем существование складов
            var existingStocks = await _context.Stocks
                .Where(s => stockGuids.Contains(s.IDStock))
                .Select(s => s.IDStock)
                .ToListAsync();

            var missingStocks = stockGuids.Except(existingStocks).ToList();
            if (missingStocks.Any())
            {
                _logger.LogWarning("Missing stocks: {MissingIds}", string.Join(", ", missingStocks));
            }

            _logger.LogInformation("Validation completed. Found {NomenclatureFound}/{NomenclatureTotal} nomenclatures and {StockFound}/{StockTotal} stocks",
                existingNomenclatures.Count, nomenclatureIds.Count,
                existingStocks.Count, stockGuids.Count);
        }

        // Классы для десериализации JSON
        public class PriceJsonRoot
        {
            public List<PriceJsonItem> ArrayOfPricesEl { get; set; } = new();
        }

        public class PriceJsonItem
        {
            public string ID { get; set; } = string.Empty;
            public string IDStock { get; set; } = string.Empty;
            public decimal PriceT { get; set; }
            public decimal PriceLimitT1 { get; set; }
            public decimal PriceT1 { get; set; }
            public decimal PriceLimitT2 { get; set; }
            public decimal PriceT2 { get; set; }
            public decimal PriceM { get; set; }
            public decimal PriceLimitM1 { get; set; }
            public decimal PriceM1 { get; set; }
            public decimal PriceLimitM2 { get; set; }
            public decimal PriceM2 { get; set; }
            public decimal NDS { get; set; }
        }
    }
}
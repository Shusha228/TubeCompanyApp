using System.Text.Json;
using backend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using backend.Data;

namespace backend.Services
{
    public class RemnantImporter
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RemnantImporter> _logger;

        public RemnantImporter(ApplicationDbContext context, ILogger<RemnantImporter> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task ImportRemnantsFromJsonAsync(string jsonFilePath)
        {
            try
            {
                _logger.LogInformation("Starting remnants import from {FilePath}", jsonFilePath);

                // Чтение JSON файла
                var jsonString = await File.ReadAllTextAsync(jsonFilePath);
                _logger.LogInformation("JSON file read successfully, length: {Length} characters", jsonString.Length);
                
                // Десериализация JSON
                var jsonData = JsonSerializer.Deserialize<RemnantJsonRoot>(jsonString);
                
                if (jsonData?.ArrayOfRemnantsEl == null)
                {
                    throw new Exception("Invalid JSON format or empty data");
                }

                _logger.LogInformation("Found {Count} remnant records in JSON", jsonData.ArrayOfRemnantsEl.Count);

                // Проверяем существование связанных сущностей
                await ValidateRelatedEntities(jsonData.ArrayOfRemnantsEl);

                // Преобразование данных в сущности
                var remnantEntities = new List<Remnant>();
                foreach (var item in jsonData.ArrayOfRemnantsEl)
                {
                    try
                    {
                        var remnant = new Remnant
                        {
                            ID = int.Parse(item.ID),
                            IDStock = item.IDStock,
                            InStockT = (decimal)item.InStockT,
                            InStockM = (decimal)item.InStockM,
                            SoonArriveT = item.SoonArriveT == 0 ? null : (decimal?)item.SoonArriveT,
                            SoonArriveM = item.SoonArriveM == 0 ? null : (decimal?)item.SoonArriveM,
                            ReservedT = item.ReservedT == 0 ? null : (decimal?)item.ReservedT,
                            ReservedM = item.ReservedM == 0 ? null : (decimal?)item.ReservedM,
                            AvgTubeLength = item.AvgTubeLength == 0 ? null : (decimal?)item.AvgTubeLength,
                            AvgTubeWeight = item.AvgTubeWeight == 0 ? null : (decimal?)item.AvgTubeWeight
                        };
                        remnantEntities.Add(remnant);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("Failed to parse remnant item with ID {ID} and Stock {Stock}: {Error}", 
                            item.ID, item.IDStock, ex.Message);
                    }
                }

                _logger.LogInformation("Successfully parsed {Count} remnant entities", remnantEntities.Count);

                // Очистка существующих данных
                _logger.LogInformation("Clearing existing remnants...");
                _context.Remnants.RemoveRange(_context.Remnants);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Existing remnants cleared");

                // Добавление новых данных
                _logger.LogInformation("Adding new remnants...");
                await _context.Remnants.AddRangeAsync(remnantEntities);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully imported {Count} remnant records", remnantEntities.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing remnant data from {FilePath}", jsonFilePath);
                throw;
            }
        }

        private async Task ValidateRelatedEntities(List<RemnantJsonItem> items)
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

        public async Task<Remnant?> GetRemnantAsync(int nomenclatureId, string stockId)
        {
            return await _context.Remnants
                .FirstOrDefaultAsync(r => r.ID == nomenclatureId && r.IDStock == stockId);
        }

        // Классы для десериализации JSON
        public class RemnantJsonRoot
        {
            public List<RemnantJsonItem> ArrayOfRemnantsEl { get; set; } = new();
        }

        public class RemnantJsonItem
        {
            public string ID { get; set; } = string.Empty;
            public string IDStock { get; set; } = string.Empty;
            public double InStockT { get; set; }
            public double InStockM { get; set; }
            public double SoonArriveT { get; set; }
            public double SoonArriveM { get; set; }
            public double ReservedT { get; set; }
            public double ReservedM { get; set; }
            public bool UnderTheOrder { get; set; }
            public double AvgTubeLength { get; set; }
            public double AvgTubeWeight { get; set; }
        }
    }
}
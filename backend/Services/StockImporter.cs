using System.Text.Json;
using backend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using backend.Data;

namespace backend.Services
{
    public class StockImporter
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<StockImporter> _logger;

        public StockImporter(ApplicationDbContext context, ILogger<StockImporter> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task ImportStocksFromJsonAsync(string jsonFilePath)
        {
            try
            {
                _logger.LogInformation("Starting stocks import from {FilePath}", jsonFilePath);

                // Чтение JSON файла
                var jsonString = await File.ReadAllTextAsync(jsonFilePath);
                _logger.LogInformation("JSON file read successfully, length: {Length} characters", jsonString.Length);
                
                // Десериализация JSON
                var jsonData = JsonSerializer.Deserialize<StockJsonRoot>(jsonString);
                
                if (jsonData?.ArrayOfStockEl == null)
                {
                    throw new Exception("Invalid JSON format or empty data");
                }

                _logger.LogInformation("Found {Count} stock records in JSON", jsonData.ArrayOfStockEl.Count);

                // Преобразование данных в сущности
                var stockEntities = new List<Stock>();
                foreach (var item in jsonData.ArrayOfStockEl)
                {
                    try
                    {
                        var stock = new Stock
                        {
                            IDStock = item.IDStock,
                            City = item.Stock, // Используем Stock как City
                            StockName = item.StockName,
                            Address = string.IsNullOrEmpty(item.Address) ? null : item.Address,
                            Schedule = string.IsNullOrEmpty(item.Schedule) ? null : item.Schedule,
                            FIASId = string.IsNullOrEmpty(item.FIASId) ? null : item.FIASId,
                            OwnerInn = string.IsNullOrEmpty(item.OwnerInn) ? null : item.OwnerInn,
                            OwnerKpp = string.IsNullOrEmpty(item.OwnerKpp) ? null : item.OwnerKpp,
                            OwnerFullName = string.IsNullOrEmpty(item.OwnerFullName) ? null : item.OwnerFullName,
                            OwnerShortName = string.IsNullOrEmpty(item.OwnerShortName) ? null : item.OwnerShortName,
                            RailwayStation = string.IsNullOrEmpty(item.RailwayStation) ? null : item.RailwayStation,
                            ConsigneeCode = string.IsNullOrEmpty(item.ConsigneeCode) ? null : item.ConsigneeCode
                        };
                        stockEntities.Add(stock);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("Failed to parse stock item with ID {ID}: {Error}", item.IDStock, ex.Message);
                    }
                }

                _logger.LogInformation("Successfully parsed {Count} stock entities", stockEntities.Count);

                // Очистка существующих данных
                _logger.LogInformation("Clearing existing stocks...");
                _context.Stocks.RemoveRange(_context.Stocks);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Existing stocks cleared");

                // Добавление новых данных
                _logger.LogInformation("Adding new stocks...");
                await _context.Stocks.AddRangeAsync(stockEntities);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully imported {Count} stock records", stockEntities.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing stock data from {FilePath}", jsonFilePath);
                throw;
            }
        }

        public async Task<List<Stock>> GetAllStocksAsync()
        {
            return await _context.Stocks
                .OrderBy(s => s.City)
                .ThenBy(s => s.StockName)
                .ToListAsync();
        }

        public async Task<Stock?> GetStockByIdAsync(string idStock)
        {
            return await _context.Stocks
                .FirstOrDefaultAsync(s => s.IDStock == idStock);
        }

        public async Task<List<Stock>> GetStocksByCityAsync(string city)
        {
            return await _context.Stocks
                .Where(s => s.City.ToLower() == city.ToLower())
                .OrderBy(s => s.StockName)
                .ToListAsync();
        }

        // Классы для десериализации JSON
        public class StockJsonRoot
        {
            public List<StockJsonItem> ArrayOfStockEl { get; set; } = new();
        }

        public class StockJsonItem
        {
            public string IDStock { get; set; } = string.Empty;
            public string Stock { get; set; } = string.Empty;
            public string StockName { get; set; } = string.Empty;
            public string Address { get; set; } = string.Empty;
            public string Schedule { get; set; } = string.Empty;
            public string IDDivision { get; set; } = string.Empty;
            public bool CashPayment { get; set; }
            public bool CardPayment { get; set; }
            public string FIASId { get; set; } = string.Empty;
            public string OwnerInn { get; set; } = string.Empty;
            public string OwnerKpp { get; set; } = string.Empty;
            public string OwnerFullName { get; set; } = string.Empty;
            public string OwnerShortName { get; set; } = string.Empty;
            public string RailwayStation { get; set; } = string.Empty;
            public string ConsigneeCode { get; set; } = string.Empty;
        }
    }
}
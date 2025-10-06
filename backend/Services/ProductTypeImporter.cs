using System.Text.Json;
using backend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using backend.Data;

namespace backend.Services
{
    public class ProductTypeImporter
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductTypeImporter> _logger;

        public ProductTypeImporter(ApplicationDbContext context, ILogger<ProductTypeImporter> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task ImportProductTypesFromJsonAsync(string jsonFilePath)
        {
            try
            {
                _logger.LogInformation("Starting product types import from {FilePath}", jsonFilePath);

                var jsonString = await File.ReadAllTextAsync(jsonFilePath);
                var jsonData = JsonSerializer.Deserialize<ProductTypeJsonRoot>(jsonString);
                
                if (jsonData?.ArrayOfTypeEl == null)
                {
                    throw new Exception("Invalid JSON format or empty data");
                }

                _logger.LogInformation("Found {Count} product type records in JSON", jsonData.ArrayOfTypeEl.Count);

                // Создаем словарь для маппинга GUID -> int ID
                var guidToIdMapping = new Dictionary<string, int>();
                var productTypeEntities = new List<ProductType>();
                int currentId = 1;

                foreach (var item in jsonData.ArrayOfTypeEl)
                {
                    try
                    {
                        var productType = new ProductType
                        {
                            IDType = currentId,
                            Type = item.Type,
                            IDParentType = string.IsNullOrEmpty(item.IDParentType) ? null : item.IDParentType,
                            OriginalGuid = item.IDType // Сохраняем оригинальный GUID
                        };
                        
                        guidToIdMapping[item.IDType] = currentId;
                        productTypeEntities.Add(productType);
                        currentId++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("Failed to parse product type item with GUID {GUID}: {Error}", item.IDType, ex.Message);
                    }
                }

                _logger.LogInformation("Successfully parsed {Count} product type entities", productTypeEntities.Count);

                // Очистка существующих данных
                _context.ProductTypes.RemoveRange(_context.ProductTypes);
                await _context.SaveChangesAsync();

                // Добавление новых данных
                await _context.ProductTypes.AddRangeAsync(productTypeEntities);
                await _context.SaveChangesAsync();

                // Сохраняем маппинг для использования в NomenclatureImporter
                // Можно сохранить в статическое поле или в базу данных
                ProductTypeMapping.SetMapping(guidToIdMapping);

                _logger.LogInformation("Successfully imported {Count} product type records", productTypeEntities.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing product type data from {FilePath}", jsonFilePath);
                throw;
            }
        }

        // Классы для десериализации JSON
        public class ProductTypeJsonRoot
        {
            public List<ProductTypeJsonItem> ArrayOfTypeEl { get; set; } = new();
        }

        public class ProductTypeJsonItem
        {
            public string IDType { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
            public string IDParentType { get; set; } = string.Empty;
        }
    }

    // Статический класс для хранения маппинга между GUID и int ID
    public static class ProductTypeMapping
    {
        private static Dictionary<string, int> _mapping = new();

        public static void SetMapping(Dictionary<string, int> mapping)
        {
            _mapping = mapping;
        }

        public static int GetIdByGuid(string guid)
        {
            return _mapping.TryGetValue(guid, out int id) ? id : 0;
        }

        public static bool ContainsGuid(string guid)
        {
            return _mapping.ContainsKey(guid);
        }
    }
}
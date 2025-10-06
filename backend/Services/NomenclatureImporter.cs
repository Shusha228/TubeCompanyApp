using System.Text.Json;
using backend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using backend.Data;

namespace backend.Services
{
    public class NomenclatureImporter
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NomenclatureImporter> _logger;

        public NomenclatureImporter(ApplicationDbContext context, ILogger<NomenclatureImporter> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task ImportNomenclatureFromJsonAsync(string jsonFilePath)
        {
            try
            {
                _logger.LogInformation("Starting nomenclature import from {FilePath}", jsonFilePath);

                var jsonString = await File.ReadAllTextAsync(jsonFilePath);
                var jsonData = JsonSerializer.Deserialize<NomenclatureJsonRoot>(jsonString);
                
                if (jsonData?.ArrayOfNomenclatureEl == null)
                {
                    throw new Exception("Invalid JSON format or empty data");
                }

                _logger.LogInformation("Found {Count} nomenclature records in JSON", jsonData.ArrayOfNomenclatureEl.Count);

                // Преобразование данных в сущности
                var nomenclatureEntities = new List<Nomenclature>();
                foreach (var item in jsonData.ArrayOfNomenclatureEl)
                {
                    try
                    {
                        // Конвертируем строковый GUID в int ID используя маппинг
                        int idType = ProductTypeMapping.GetIdByGuid(item.IDType);
                        if (idType == 0)
                        {
                            _logger.LogWarning("Product type with GUID {TypeGuid} not found in mapping, skipping nomenclature {ID}", item.IDType, item.ID);
                            continue;
                        }

                        var nomenclature = new Nomenclature
                        {
                            ID = int.Parse(item.ID),
                            IDCat = item.IDCat,
                            IDType = idType, // Теперь int
                            IDTypeNew = item.IDTypeNew,
                            ProductionType = item.ProductionType,
                            IDFunctionType = item.IDFunctionType,
                            Name = item.Name,
                            Gost = item.Gost,
                            FormOfLength = item.FormOfLength,
                            Manufacturer = item.Manufacturer,
                            SteelGrade = item.SteelGrade,
                            Diameter = (decimal)item.Diameter,
                            ProfileSize2 = item.ProfileSize2 == 0 ? null : (decimal?)item.ProfileSize2,
                            PipeWallThickness = (decimal)item.PipeWallThickness,
                            Status = item.Status.ToString(),
                            Koef = (decimal)item.Koef
                        };
                        nomenclatureEntities.Add(nomenclature);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("Failed to parse nomenclature item with ID {ID}: {Error}", item.ID, ex.Message);
                    }
                }

                _logger.LogInformation("Successfully parsed {Count} nomenclature entities", nomenclatureEntities.Count);

                // Очистка существующих данных
                _context.Nomenclatures.RemoveRange(_context.Nomenclatures);
                await _context.SaveChangesAsync();

                // Добавление новых данных
                await _context.Nomenclatures.AddRangeAsync(nomenclatureEntities);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully imported {Count} nomenclature records", nomenclatureEntities.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing nomenclature data from {FilePath}", jsonFilePath);
                throw;
            }
        }

        // Классы для десериализации JSON
        public class NomenclatureJsonRoot
        {
            public List<NomenclatureJsonItem> ArrayOfNomenclatureEl { get; set; } = new();
        }

        public class NomenclatureJsonItem
        {
            public string ID { get; set; } = string.Empty;
            public string IDCat { get; set; } = string.Empty;
            public string IDType { get; set; } = string.Empty;
            public string IDTypeNew { get; set; } = string.Empty;
            public string ProductionType { get; set; } = string.Empty;
            public string IDFunctionType { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Gost { get; set; } = string.Empty;
            public string FormOfLength { get; set; } = string.Empty;
            public string Manufacturer { get; set; } = string.Empty;
            public string SteelGrade { get; set; } = string.Empty;
            public double Diameter { get; set; }
            public double ProfileSize2 { get; set; }
            public double PipeWallThickness { get; set; }
            public int Status { get; set; }
            public double Koef { get; set; }
        }
    }
}
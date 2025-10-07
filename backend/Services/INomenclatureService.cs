using backend.Models.Entities;
using backend.Data;
using backend.Models.DTOs.Nomenclature;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public interface INomenclatureService
    {
        Task<List<Nomenclature>> GetAllAsync();
        Task<Nomenclature?> GetByIdAsync(int id);
        Task<Nomenclature> CreateAsync(Nomenclature nomenclature);
        Task<Nomenclature?> UpdateAsync(int id, Nomenclature nomenclature);
        Task<bool> DeleteAsync(int id);
        Task<List<Nomenclature>> GetByTypeAsync(int typeId);
        Task<List<Nomenclature>> SearchAsync(string searchTerm);
        
        Task<NomenclaturePaginationResponse> GetPagedAsync(int from, int to);
        Task<NomenclaturePaginationResponse> GetByTypePagedAsync(int typeId, int from, int to);
        Task<NomenclaturePaginationResponse> SearchPagedAsync(string searchTerm, int from, int to);
        
        Task<PriceCalculationResult> CalculatePriceAsync(PriceCalculationRequest request);
    }

    public class NomenclatureService : INomenclatureService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NomenclatureService> _logger;

        public NomenclatureService(ApplicationDbContext context, ILogger<NomenclatureService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Nomenclature>> GetAllAsync()
        {
            try
            {
                var nomenclatures = await _context.Nomenclatures
                    .OrderBy(n => n.ID)
                    .ToListAsync();

                // Явно загружаем Remnants для каждой номенклатуры
                await LoadRemnantsForNomenclatures(nomenclatures);

                return nomenclatures;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all nomenclature");
                throw;
            }
        }

        public async Task<NomenclaturePaginationResponse> GetPagedAsync(int from, int to)
        {
            try
            {
                if (from < 0) throw new ArgumentException("From cannot be negative");
                if (to <= from) throw new ArgumentException("To must be greater than from");
                if (to - from > 100) throw new ArgumentException("Page size cannot exceed 100 items");
                
                var totalCount = await _context.Nomenclatures.CountAsync();

                var items = await _context.Nomenclatures
                    .OrderBy(n => n.ID)
                    .Skip(from)
                    .Take(to - from)
                    .ToListAsync();

                // Явно загружаем Remnants для полученных номенклатур
                await LoadRemnantsForNomenclatures(items);

                var pageSize = to - from;
                var currentPage = from / pageSize;
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                return new NomenclaturePaginationResponse
                {
                    Items = items,
                    Meta = new NomenclaturePaginationMeta
                    {
                        TotalPages = totalPages,
                        Page = currentPage,
                        PageLimit = pageSize,
                        TotalCount = totalCount
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting paged nomenclature from: {from}, to: {to}");
                throw;
            }
        }

        public async Task<Nomenclature?> GetByIdAsync(int id)
        {
            try
            {
                var nomenclature = await _context.Nomenclatures
                    .FirstOrDefaultAsync(n => n.ID == id);

                if (nomenclature != null)
                {
                    // Явно загружаем Remnants для этой номенклатуры
                    var remnants = await _context.Remnants
                        .Where(r => r.ID == id)
                        .ToListAsync();
                    
                    nomenclature.Remnants = remnants;
                }

                return nomenclature;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting nomenclature by ID: {id}");
                throw;
            }
        }

        public async Task<Nomenclature> CreateAsync(Nomenclature nomenclature)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                var existing = await _context.Nomenclatures
                    .FirstOrDefaultAsync(n => n.ID == nomenclature.ID);
                
                if (existing != null)
                {
                    throw new InvalidOperationException($"Nomenclature with ID {nomenclature.ID} already exists");
                }

                // Не сохраняем Remnants при создании - они должны создаваться отдельно
                nomenclature.Remnants = null;

                _context.Nomenclatures.Add(nomenclature);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation($"Created nomenclature with ID: {nomenclature.ID}");
                return nomenclature;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating nomenclature");
                throw;
            }
        }

        public async Task<Nomenclature?> UpdateAsync(int id, Nomenclature nomenclature)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                var existing = await _context.Nomenclatures
                    .FirstOrDefaultAsync(n => n.ID == id);

                if (existing == null)
                {
                    return null;
                }
                
                // Обновляем только основные поля
                existing.IDCat = nomenclature.IDCat;
                existing.IDType = nomenclature.IDType;
                existing.IDTypeNew = nomenclature.IDTypeNew;
                existing.ProductionType = nomenclature.ProductionType;
                existing.IDFunctionType = nomenclature.IDFunctionType;
                existing.Name = nomenclature.Name;
                existing.Gost = nomenclature.Gost;
                existing.FormOfLength = nomenclature.FormOfLength;
                existing.Manufacturer = nomenclature.Manufacturer;
                existing.SteelGrade = nomenclature.SteelGrade;
                existing.Diameter = nomenclature.Diameter;
                existing.ProfileSize2 = nomenclature.ProfileSize2;
                existing.PipeWallThickness = nomenclature.PipeWallThickness;
                existing.Status = nomenclature.Status;
                existing.Koef = nomenclature.Koef;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Загружаем актуальные Remnants после обновления
                await LoadRemnantsForNomenclatures(new List<Nomenclature> { existing });

                _logger.LogInformation($"Updated nomenclature with ID: {id}");
                return existing;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Error updating nomenclature with ID: {id}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                var nomenclature = await _context.Nomenclatures
                    .FirstOrDefaultAsync(n => n.ID == id);

                if (nomenclature == null)
                {
                    return false;
                }

                _context.Nomenclatures.Remove(nomenclature);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation($"Deleted nomenclature with ID: {id}");
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Error deleting nomenclature with ID: {id}");
                throw;
            }
        }

        public async Task<List<Nomenclature>> GetByTypeAsync(int typeId)
        {
            try
            {
                var nomenclatures = await _context.Nomenclatures
                    .Where(n => n.IDType == typeId)
                    .OrderBy(n => n.ID)
                    .ToListAsync();

                // Явно загружаем Remnants для найденных номенклатур
                await LoadRemnantsForNomenclatures(nomenclatures);

                return nomenclatures;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting nomenclature by type: {typeId}");
                throw;
            }
        }

        public async Task<NomenclaturePaginationResponse> GetByTypePagedAsync(int typeId, int from, int to)
        {
            try
            {
                if (from < 0) throw new ArgumentException("From cannot be negative");
                if (to <= from) throw new ArgumentException("To must be greater than from");
                if (to - from > 100) throw new ArgumentException("Page size cannot exceed 100 items");
                
                var query = _context.Nomenclatures
                    .Where(n => n.IDType == typeId);

                var totalCount = await query.CountAsync();

                var items = await query
                    .OrderBy(n => n.ID)
                    .Skip(from)
                    .Take(to - from)
                    .ToListAsync();

                // Явно загружаем Remnants для полученных номенклатур
                await LoadRemnantsForNomenclatures(items);

                var pageSize = to - from;
                var currentPage = from / pageSize;
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                return new NomenclaturePaginationResponse
                {
                    Items = items,
                    Meta = new NomenclaturePaginationMeta
                    {
                        TotalPages = totalPages,
                        Page = currentPage,
                        PageLimit = pageSize,
                        TotalCount = totalCount,
                        TypeId = typeId
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting paged nomenclature by type: {typeId}, from: {from}, to: {to}");
                throw;
            }
        }

        public async Task<List<Nomenclature>> SearchAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return await GetAllAsync();
                }

                var term = searchTerm.ToLower();
                var nomenclatures = await _context.Nomenclatures
                    .Where(n => n.Name.ToLower().Contains(term) ||
                               n.Gost.ToLower().Contains(term) ||
                               n.SteelGrade.ToLower().Contains(term) ||
                               n.Manufacturer.ToLower().Contains(term))
                    .OrderBy(n => n.ID)
                    .ToListAsync();

                // Явно загружаем Remnants для найденных номенклатур
                await LoadRemnantsForNomenclatures(nomenclatures);

                return nomenclatures;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching nomenclature with term: {searchTerm}");
                throw;
            }
        }

        public async Task<NomenclaturePaginationResponse> SearchPagedAsync(string searchTerm, int from, int to)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return await GetPagedAsync(from, to);
                }

                if (from < 0) throw new ArgumentException("From cannot be negative");
                if (to <= from) throw new ArgumentException("To must be greater than from");
                if (to - from > 100) throw new ArgumentException("Page size cannot exceed 100 items");

                var term = searchTerm.ToLower();
                var query = _context.Nomenclatures
                    .Where(n => n.Name.ToLower().Contains(term) ||
                               n.Gost.ToLower().Contains(term) ||
                               n.SteelGrade.ToLower().Contains(term) ||
                               n.Manufacturer.ToLower().Contains(term));

                var totalCount = await query.CountAsync();

                var items = await query
                    .OrderBy(n => n.ID)
                    .Skip(from)
                    .Take(to - from)
                    .ToListAsync();

                // Явно загружаем Remnants для полученных номенклатур
                await LoadRemnantsForNomenclatures(items);

                var pageSize = to - from;
                var currentPage = from / pageSize;
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                return new NomenclaturePaginationResponse
                {
                    Items = items,
                    Meta = new NomenclaturePaginationMeta
                    {
                        TotalPages = totalPages,
                        Page = currentPage,
                        PageLimit = pageSize,
                        TotalCount = totalCount,
                        SearchTerm = searchTerm
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching paged nomenclature with term: {searchTerm}, from: {from}, to: {to}");
                throw;
            }
        }

        /// <summary>
        /// Вспомогательный метод для загрузки Remnants для списка номенклатур
        /// </summary>
        private async Task LoadRemnantsForNomenclatures(List<Nomenclature> nomenclatures)
        {
            if (!nomenclatures.Any()) return;

            var nomenclatureIds = nomenclatures.Select(n => n.ID).ToList();

            // Получаем все Remnants для этих номенклатур одним запросом
            var allRemnants = await _context.Remnants
                .Where(r => nomenclatureIds.Contains(r.ID))
                .ToListAsync();

            // Группируем Remnants по ID номенклатуры
            var remnantsByNomenclatureId = allRemnants
                .GroupBy(r => r.ID)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Назначаем Remnants каждой номенклатуре
            foreach (var nomenclature in nomenclatures)
            {
                if (remnantsByNomenclatureId.TryGetValue(nomenclature.ID, out var remnants))
                {
                    nomenclature.Remnants = remnants;
                }
                else
                {
                    nomenclature.Remnants = new List<Remnant>();
                }
            }
        }
        
        public async Task<PriceCalculationResult> CalculatePriceAsync(PriceCalculationRequest request)
        {
            try
            {
                var nomenclature = await _context.Nomenclatures
                    .FirstOrDefaultAsync(n => n.ID == request.NomenclatureId);

                if (nomenclature == null)
                {
                    throw new ArgumentException($"Номенклатура с ID {request.NomenclatureId} не найдена");
                }
                
                var prices = await _context.Prices
                    .Where(p => p.ID == request.NomenclatureId && p.IDStock == request.StockId)
                    .FirstOrDefaultAsync();

                if (prices == null)
                {
                    throw new ArgumentException($"Цены для номенклатуры {request.NomenclatureId} на складе {request.StockId} не найдены");
                }
                
                var remnant = await _context.Remnants
                    .FirstOrDefaultAsync(r => r.ID == request.NomenclatureId && r.IDStock == request.StockId);

                decimal basePrice, finalPrice;
                decimal quantity = request.Quantity;

                if (request.IsInMeters)
                {
                    basePrice = prices.PriceM * quantity;
                    
                    if (prices.PriceLimitM2.HasValue && quantity >= prices.PriceLimitM2.Value && prices.PriceM2.HasValue)
                    {
                        finalPrice = prices.PriceM2.Value * quantity;
                    }
                    else if (prices.PriceLimitM1.HasValue && quantity >= prices.PriceLimitM1.Value && prices.PriceM1.HasValue)
                    {
                        finalPrice = prices.PriceM1.Value * quantity;
                    }
                    else
                    {
                        finalPrice = basePrice;
                    }
                }
                else
                {
                    basePrice = prices.PriceT * quantity;
                    
                    if (prices.PriceLimitT2.HasValue && quantity >= prices.PriceLimitT2.Value && prices.PriceT2.HasValue)
                    {
                        finalPrice = prices.PriceT2.Value * quantity;
                    }
                    else if (prices.PriceLimitT1.HasValue && quantity >= prices.PriceLimitT1.Value && prices.PriceT1.HasValue)
                    {
                        finalPrice = prices.PriceT1.Value * quantity;
                    }
                    else
                    {
                        finalPrice = basePrice;
                    }
                }
                
                var discountPercent = basePrice > 0 ? (basePrice - finalPrice) / basePrice * 100 : 0;
                
                decimal convertedQuantity = request.Quantity;
                if (request.IsInMeters && request.ConvertToTons)
                {
                    convertedQuantity = request.Quantity * nomenclature.Koef;
                }
                else if (!request.IsInMeters && request.ConvertToMeters)
                {
                    convertedQuantity = request.Quantity / nomenclature.Koef;
                }

                var result = new PriceCalculationResult
                {
                    NomenclatureId = request.NomenclatureId,
                    NomenclatureName = nomenclature.Name,
                    StockId = request.StockId,
                    Quantity = request.Quantity,
                    IsInMeters = request.IsInMeters,
                    BasePrice = basePrice,
                    FinalPrice = finalPrice,
                    DiscountPercent = discountPercent,
                    UnitPrice = request.IsInMeters ? prices.PriceM : prices.PriceT,
                    DiscountedUnitPrice = finalPrice / request.Quantity,
                    AvailableStock = remnant?.InStockM ?? 0,
                    AvailableStockTons = remnant?.InStockT ?? 0,
                    Koef = nomenclature.Koef,
                    ConvertedQuantity = convertedQuantity,
                    ConversionType = request.ConvertToTons ? "meters_to_tons" : 
                                   request.ConvertToMeters ? "tons_to_meters" : "none"
                };

                _logger.LogInformation($"Расчет стоимости для номенклатуры {request.NomenclatureId}: " +
                                     $"Количество: {request.Quantity} {(request.IsInMeters ? "м" : "т")}, " +
                                     $"Итоговая цена: {finalPrice:0.00} руб.");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка расчета стоимости для номенклатуры {request.NomenclatureId}");
                throw;
            }
        }
    }

    public class NomenclaturePaginationResponse
    {
        public List<Nomenclature> Items { get; set; } = new List<Nomenclature>();
        public NomenclaturePaginationMeta Meta { get; set; } = new NomenclaturePaginationMeta();
    }

    public class NomenclaturePaginationMeta
    {
        public int TotalPages { get; set; }
        public int Page { get; set; }
        public int PageLimit { get; set; }
        public int TotalCount { get; set; }
        public string? SearchTerm { get; set; }
        public int? TypeId { get; set; }
    }
}
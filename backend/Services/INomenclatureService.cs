using backend.Models.Entities;
using backend.Data;
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
        
        /// <summary>
        /// Расчет стоимости для номенклатуры
        /// </summary>
        Task<Models.Nomenclature.PriceCalculationResult> CalculatePriceAsync(Models.Nomenclature.PriceCalculationRequest request);
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
                return await _context.Nomenclatures
                    .OrderBy(n => n.ID)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all nomenclature");
                throw;
            }
        }

        public async Task<Nomenclature?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Nomenclatures
                    .FirstOrDefaultAsync(n => n.ID == id);
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
                return await _context.Nomenclatures
                    .Where(n => n.IDType == typeId)
                    .OrderBy(n => n.ID)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting nomenclature by type: {typeId}");
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
                return await _context.Nomenclatures
                    .Where(n => n.Name.ToLower().Contains(term) ||
                               n.Gost.ToLower().Contains(term) ||
                               n.SteelGrade.ToLower().Contains(term) ||
                               n.Manufacturer.ToLower().Contains(term))
                    .OrderBy(n => n.ID)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching nomenclature with term: {searchTerm}");
                throw;
            }
        }
        
        /// <summary>
        /// Расчет стоимости для номенклатуры с учетом скидок
        /// </summary>
        public async Task<Models.Nomenclature.PriceCalculationResult> CalculatePriceAsync(Models.Nomenclature.PriceCalculationRequest request)
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

                var result = new Models.Nomenclature.PriceCalculationResult
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
}
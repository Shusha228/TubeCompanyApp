// Services/UpdateService.cs
using backend.Data;
using backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class UpdateService : IUpdateService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdateService> _logger;

        public UpdateService(ApplicationDbContext context, ILogger<UpdateService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<UpdateResult> ProcessPriceUpdatesAsync(List<PriceUpdate> updates)
        {
            try
            {
                if (updates == null || !updates.Any())
                {
                    _logger.LogInformation("No price updates to process");
                    return new UpdateResult { ProcessedCount = 0 };
                }

                foreach (var update in updates)
                {
                    update.UpdatedAt = DateTime.UtcNow;
                    update.IsProcessed = false;
                }

                _context.PriceUpdates.AddRange(updates);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Added {updates.Count} price updates to queue");
                return new UpdateResult { ProcessedCount = updates.Count };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing price updates");
                throw;
            }
        }

        public async Task<UpdateResult> ProcessRemnantUpdatesAsync(List<RemnantUpdate> updates)
        {
            try
            {
                if (updates == null || !updates.Any())
                {
                    _logger.LogInformation("No remnant updates to process");
                    return new UpdateResult { ProcessedCount = 0 };
                }

                foreach (var update in updates)
                {
                    update.UpdatedAt = DateTime.UtcNow;
                    update.IsProcessed = false;
                }

                _context.RemnantUpdates.AddRange(updates);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Added {updates.Count} remnant updates to queue");
                return new UpdateResult { ProcessedCount = updates.Count };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing remnant updates");
                throw;
            }
        }


        public async Task<int> ProcessPriceUpdatesBatchAsync()
        {
            try
            {
                _logger.LogInformation("Starting price updates processing...");

                var unprocessedUpdates = await _context.PriceUpdates
                    .Where(u => !u.IsProcessed)
                    .ToListAsync();

                _logger.LogInformation($"Found {unprocessedUpdates.Count} unprocessed price updates");

                if (!unprocessedUpdates.Any())
                    return 0;

                var processedCount = 0;
                var errorCount = 0;

                foreach (var update in unprocessedUpdates)
                {
                    try
                    {
                        // Теперь ProductId и StockId уже содержат правильные значения из JSON
                        int effectiveProductId = update.ProductId;
                        string effectiveStockId = update.StockId;

                        _logger.LogDebug($"Processing price update - ProductId: {effectiveProductId}, StockId: {effectiveStockId}");

                        // ПРОВЕРЯЕМ: существует ли номенклатура и склад
                        var nomenclatureExists = await _context.Nomenclatures
                            .AnyAsync(n => n.ID == effectiveProductId);
                        
                        var stockExists = await _context.Stocks
                            .AnyAsync(s => s.IDStock == effectiveStockId);

                        _logger.LogInformation($"Checking existence - ProductId: {effectiveProductId} exists: {nomenclatureExists}, StockId: {effectiveStockId} exists: {stockExists}");

                        if (!nomenclatureExists || !stockExists)
                        {
                            _logger.LogWarning($"Skipping price update - Nomenclature {effectiveProductId} exists: {nomenclatureExists}, Stock {effectiveStockId} exists: {stockExists}");
                            
                            update.IsProcessed = true;
                            _context.UpdateLogs.Add(new UpdateLog
                            {
                                EntityType = "Price",
                                EntityId = $"{effectiveProductId}_{effectiveStockId}",
                                Operation = "ERROR",
                                Timestamp = DateTime.UtcNow,
                                Details = $"Missing related entities: Nomenclature {effectiveProductId} exists: {nomenclatureExists}, Stock {effectiveStockId} exists: {stockExists}"
                            });
                            errorCount++;
                            continue;
                        }

                        var existingPrice = await _context.Prices
                            .FirstOrDefaultAsync(p => p.ID == effectiveProductId && p.IDStock == effectiveStockId);

                        if (existingPrice != null)
                        {
                            // Обновляем существующую запись (автоматически исправляем отрицательные цены)
                            if (update.PriceT.HasValue) existingPrice.PriceT = Math.Abs(update.PriceT.Value);
                            if (update.PriceT1.HasValue) existingPrice.PriceT1 = Math.Abs(update.PriceT1.Value);
                            if (update.PriceT2.HasValue) existingPrice.PriceT2 = Math.Abs(update.PriceT2.Value);
                            if (update.PriceM.HasValue) existingPrice.PriceM = Math.Abs(update.PriceM.Value);
                            if (update.PriceM1.HasValue) existingPrice.PriceM1 = Math.Abs(update.PriceM1.Value);
                            if (update.PriceM2.HasValue) existingPrice.PriceM2 = Math.Abs(update.PriceM2.Value);
                            if (update.PriceLimitT1.HasValue) existingPrice.PriceLimitT1 = Math.Abs(update.PriceLimitT1.Value);
                            if (update.PriceLimitT2.HasValue) existingPrice.PriceLimitT2 = Math.Abs(update.PriceLimitT2.Value);
                            if (update.PriceLimitM1.HasValue) existingPrice.PriceLimitM1 = Math.Abs(update.PriceLimitM1.Value);
                            if (update.PriceLimitM2.HasValue) existingPrice.PriceLimitM2 = Math.Abs(update.PriceLimitM2.Value);
                            if (update.NDS.HasValue) existingPrice.NDS = update.NDS.Value;

                            _context.Prices.Update(existingPrice);
                            _logger.LogDebug($"Updated existing price for product {effectiveProductId} at stock {effectiveStockId}");
                        }
                        else
                        {
                            // Создаем новую запись (автоматически исправляем отрицательные цены)
                            var newPrice = new Price
                            {
                                ID = effectiveProductId,
                                IDStock = effectiveStockId,
                                PriceT = Math.Abs(update.PriceT ?? 0),
                                PriceT1 = update.PriceT1.HasValue ? Math.Abs(update.PriceT1.Value) : null,
                                PriceT2 = update.PriceT2.HasValue ? Math.Abs(update.PriceT2.Value) : null,
                                PriceM = Math.Abs(update.PriceM ?? 0),
                                PriceM1 = update.PriceM1.HasValue ? Math.Abs(update.PriceM1.Value) : null,
                                PriceM2 = update.PriceM2.HasValue ? Math.Abs(update.PriceM2.Value) : null,
                                PriceLimitT1 = update.PriceLimitT1.HasValue ? Math.Abs(update.PriceLimitT1.Value) : null,
                                PriceLimitT2 = update.PriceLimitT2.HasValue ? Math.Abs(update.PriceLimitT2.Value) : null,
                                PriceLimitM1 = update.PriceLimitM1.HasValue ? Math.Abs(update.PriceLimitM1.Value) : null,
                                PriceLimitM2 = update.PriceLimitM2.HasValue ? Math.Abs(update.PriceLimitM2.Value) : null,
                                NDS = update.NDS ?? 20
                            };

                            _context.Prices.Add(newPrice);
                            _logger.LogDebug($"Created new price for product {effectiveProductId} at stock {effectiveStockId}");
                        }

                        update.IsProcessed = true;
                        processedCount++;

                        // Логируем успешное обновление
                        _context.UpdateLogs.Add(new UpdateLog
                        {
                            EntityType = "Price",
                            EntityId = $"{effectiveProductId}_{effectiveStockId}",
                            Operation = "UPDATE",
                            Timestamp = DateTime.UtcNow,
                            Details = $"Price updated for product {effectiveProductId} at stock {effectiveStockId}"
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error processing price update for product {update.ProductId}, stock {update.StockId}");
                        
                        _context.UpdateLogs.Add(new UpdateLog
                        {
                            EntityType = "Price",
                            EntityId = $"{update.ProductId}_{update.StockId}",
                            Operation = "ERROR",
                            Timestamp = DateTime.UtcNow,
                            Details = $"Error: {ex.Message}"
                        });
                        errorCount++;
                    }
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation($"Successfully processed {processedCount} price updates, {errorCount} errors");
                return processedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fatal error during price updates processing");
                throw;
            }
        }
        public async Task<int> ProcessRemnantUpdatesBatchAsync()
        {
            var unprocessedUpdates = await _context.RemnantUpdates
                .Where(u => !u.IsProcessed)
                .ToListAsync();

            if (!unprocessedUpdates.Any())
                return 0;

            var processedCount = 0;
            var errorCount = 0;

            foreach (var update in unprocessedUpdates)
            {
                try
                {
                    int effectiveProductId = update.ProductId;
                    string effectiveStockId = update.StockId;

                    _logger.LogDebug($"Processing remnant update - ProductId: {effectiveProductId}, StockId: {effectiveStockId}");

                    // ПРОВЕРКА СУЩЕСТВОВАНИЯ через отдельные запросы
                    var nomenclatureExists = await _context.Nomenclatures
                        .AnyAsync(n => n.ID == effectiveProductId);
                    
                    var stockExists = await _context.Stocks
                        .AnyAsync(s => s.IDStock == effectiveStockId);

                    if (!nomenclatureExists || !stockExists)
                    {
                        _logger.LogWarning($"Skipping remnant update - Missing related entities");
                        update.IsProcessed = true;
                        _context.UpdateLogs.Add(new UpdateLog
                        {
                            EntityType = "Remnant",
                            EntityId = $"{effectiveProductId}_{effectiveStockId}",
                            Operation = "ERROR",
                            Timestamp = DateTime.UtcNow,
                            Details = $"Missing related entities: Nomenclature {effectiveProductId} exists: {nomenclatureExists}, Stock {effectiveStockId} exists: {stockExists}"
                        });
                        errorCount++;
                        continue;
                    }

                    // ИСПОЛЬЗУЕМ SQL-запрос напрямую чтобы обойти проблему с навигационными свойствами
                    var existingRemnant = await _context.Remnants
                        .AsNoTracking() // Отключаем отслеживание изменений
                        .FirstOrDefaultAsync(r => r.ID == effectiveProductId && r.IDStock == effectiveStockId);

                    if (existingRemnant != null)
                    {
                        // Обновляем через прямой SQL или создаем новый объект
                        var updatedRemnant = new Remnant
                        {
                            ID = effectiveProductId,
                            IDStock = effectiveStockId,
                            InStockT = Math.Abs(update.InStockT ?? 0),
                            InStockM = Math.Abs(update.InStockM ?? 0),
                            SoonArriveT = update.SoonArriveT.HasValue ? Math.Abs(update.SoonArriveT.Value) : null,
                            SoonArriveM = update.SoonArriveM.HasValue ? Math.Abs(update.SoonArriveM.Value) : null,
                            ReservedT = update.ReservedT.HasValue ? Math.Abs(update.ReservedT.Value) : null,
                            ReservedM = update.ReservedM.HasValue ? Math.Abs(update.ReservedM.Value) : null,
                            AvgTubeLength = update.AvgTubeLength.HasValue ? Math.Abs(update.AvgTubeLength.Value) : null,
                            AvgTubeWeight = update.AvgTubeWeight.HasValue ? Math.Abs(update.AvgTubeWeight.Value) : null
                        };

                        _context.Remnants.Update(updatedRemnant);
                        _logger.LogDebug($"Updated existing remnant for product {effectiveProductId} at stock {effectiveStockId}");
                    }
                    else
                    {
                        // Создаем новую запись
                        var newRemnant = new Remnant
                        {
                            ID = effectiveProductId,
                            IDStock = effectiveStockId,
                            InStockT = Math.Abs(update.InStockT ?? 0),
                            InStockM = Math.Abs(update.InStockM ?? 0),
                            SoonArriveT = update.SoonArriveT.HasValue ? Math.Abs(update.SoonArriveT.Value) : null,
                            SoonArriveM = update.SoonArriveM.HasValue ? Math.Abs(update.SoonArriveM.Value) : null,
                            ReservedT = update.ReservedT.HasValue ? Math.Abs(update.ReservedT.Value) : null,
                            ReservedM = update.ReservedM.HasValue ? Math.Abs(update.ReservedM.Value) : null,
                            AvgTubeLength = update.AvgTubeLength.HasValue ? Math.Abs(update.AvgTubeLength.Value) : null,
                            AvgTubeWeight = update.AvgTubeWeight.HasValue ? Math.Abs(update.AvgTubeWeight.Value) : null
                        };

                        _context.Remnants.Add(newRemnant);
                        _logger.LogDebug($"Created new remnant for product {effectiveProductId} at stock {effectiveStockId}");
                    }

                    update.IsProcessed = true;
                    processedCount++;

                    _context.UpdateLogs.Add(new UpdateLog
                    {
                        EntityType = "Remnant",
                        EntityId = $"{effectiveProductId}_{effectiveStockId}",
                        Operation = "UPDATE",
                        Timestamp = DateTime.UtcNow,
                        Details = $"Remnant updated for product {effectiveProductId} at stock {effectiveStockId}"
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing remnant update for product {update.ProductId}, stock {update.StockId}");
                    
                    _context.UpdateLogs.Add(new UpdateLog
                    {
                        EntityType = "Remnant",
                        EntityId = $"{update.ProductId}_{update.StockId}",
                        Operation = "ERROR",
                        Timestamp = DateTime.UtcNow,
                        Details = $"Error: {ex.Message}"
                    });
                    errorCount++;
                }
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Processed {processedCount} remnant updates, {errorCount} errors");
            return processedCount;
        }


        public async Task<int> ProcessAllPendingUpdatesAsync()
        {
            var totalProcessed = 0;

            totalProcessed += await ProcessPriceUpdatesBatchAsync();
            totalProcessed += await ProcessRemnantUpdatesBatchAsync();

            _logger.LogInformation($"Total processed updates: {totalProcessed}");
            return totalProcessed;
        }

        public async Task<UpdateStatus> GetUpdateStatusAsync()
        {
            var pendingPrices = await _context.PriceUpdates.CountAsync(u => !u.IsProcessed);
            var pendingRemnants = await _context.RemnantUpdates.CountAsync(u => !u.IsProcessed);
            var pendingStocks = await _context.StockUpdates.CountAsync(u => !u.IsProcessed);

            var lastProcessed = await _context.UpdateLogs
                .Where(l => l.Operation == "UPDATE" || l.Operation == "DELETE")
                .OrderByDescending(l => l.Timestamp)
                .FirstOrDefaultAsync();

            return new UpdateStatus
            {
                PendingPriceUpdates = pendingPrices,
                PendingRemnantUpdates = pendingRemnants,
                PendingStockUpdates = pendingStocks,
                LastProcessedTime = lastProcessed?.Timestamp,
                TotalPending = pendingPrices + pendingRemnants + pendingStocks
            };
        }

        public async Task<List<UpdateLog>> GetUpdateLogsAsync(DateTime from, DateTime to)
        {
            return await _context.UpdateLogs
                .Where(l => l.Timestamp >= from && l.Timestamp <= to)
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();
        }

        public async Task<bool> ForceFullSyncAsync()
        {
            try
            {
                _logger.LogInformation("Forced full sync initiated");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during forced full sync");
                return false;
            }
        }

        public async Task<bool> CleanupProcessedUpdatesAsync(DateTime olderThan)
        {
            try
            {
                var processedPriceUpdates = _context.PriceUpdates
                    .Where(u => u.IsProcessed && u.UpdatedAt < olderThan);
                _context.PriceUpdates.RemoveRange(processedPriceUpdates);

                var processedRemnantUpdates = _context.RemnantUpdates
                    .Where(u => u.IsProcessed && u.UpdatedAt < olderThan);
                _context.RemnantUpdates.RemoveRange(processedRemnantUpdates);

                var processedStockUpdates = _context.StockUpdates
                    .Where(u => u.IsProcessed && u.UpdatedAt < olderThan);
                _context.StockUpdates.RemoveRange(processedStockUpdates);

                var oldLogs = _context.UpdateLogs
                    .Where(l => l.Timestamp < olderThan);
                _context.UpdateLogs.RemoveRange(oldLogs);

                var removedCount = await _context.SaveChangesAsync();
                _logger.LogInformation($"Cleaned up {removedCount} processed updates older than {olderThan}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up processed updates");
                return false;
            }
        }
    }
}
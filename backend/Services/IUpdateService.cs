// Services/IUpdateService.cs
using backend.Models.Entities;

namespace backend.Services
{
    public interface IUpdateService
    {
        // Основные методы обработки (теперь принимают списки объектов)
        Task<UpdateResult> ProcessPriceUpdatesAsync(List<PriceUpdate> updates);
        Task<UpdateResult> ProcessRemnantUpdatesAsync(List<RemnantUpdate> updates);
        Task<UpdateResult> ProcessStockUpdatesAsync(List<StockUpdate> updates);
        
        // Пакетная обработка
        Task<int> ProcessAllPendingUpdatesAsync();
        Task<int> ProcessPriceUpdatesBatchAsync();
        Task<int> ProcessRemnantUpdatesBatchAsync();
        Task<int> ProcessStockUpdatesBatchAsync();
        
        // Статус и мониторинг
        Task<UpdateStatus> GetUpdateStatusAsync();
        Task<List<UpdateLog>> GetUpdateLogsAsync(DateTime from, DateTime to);
        
        // Ручное управление
        Task<bool> ForceFullSyncAsync();
        Task<bool> CleanupProcessedUpdatesAsync(DateTime olderThan);
    }

    public class UpdateResult
    {
        public int ProcessedCount { get; set; }
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    }
}
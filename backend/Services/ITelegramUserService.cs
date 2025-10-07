using backend.Models.Entities;
using backend.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public interface ITelegramUserService
    {
        Task<List<TelegramUser>> GetAllAsync();
        Task<TelegramUser?> GetByIdAsync(long telegramUserId);
        Task<TelegramUser> CreateAsync(TelegramUser user);
        Task<TelegramUser?> UpdateAsync(long telegramUserId, TelegramUser user);
        Task<bool> DeleteAsync(long telegramUserId);
        Task<TelegramUser?> GetByInnAsync(string inn);
        Task<List<TelegramUser>> SearchAsync(string searchTerm);
    }

    public class TelegramUserService : ITelegramUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TelegramUserService> _logger;

        public TelegramUserService(ApplicationDbContext context, ILogger<TelegramUserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<TelegramUser>> GetAllAsync()
        {
            try
            {
                return await _context.TelegramUsers
                    .OrderBy(u => u.TelegramUserId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all Telegram users");
                throw;
            }
        }

        public async Task<TelegramUser?> GetByIdAsync(long telegramUserId)
        {
            try
            {
                return await _context.TelegramUsers
                    .FirstOrDefaultAsync(u => u.TelegramUserId == telegramUserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting Telegram user by ID: {telegramUserId}");
                throw;
            }
        }

        public async Task<TelegramUser> CreateAsync(TelegramUser user)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                // Проверяем, существует ли уже пользователь
                var existing = await _context.TelegramUsers
                    .FirstOrDefaultAsync(u => u.TelegramUserId == user.TelegramUserId);
                
                if (existing != null)
                {
                    throw new InvalidOperationException($"Telegram user with ID {user.TelegramUserId} already exists");
                }

                // Устанавливаем даты
                user.CreatedAt = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;

                _context.TelegramUsers.Add(user);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation($"Created Telegram user with ID: {user.TelegramUserId}");
                return user;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating Telegram user");
                throw;
            }
        }

        public async Task<TelegramUser?> UpdateAsync(long telegramUserId, TelegramUser user)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                var existing = await _context.TelegramUsers
                    .FirstOrDefaultAsync(u => u.TelegramUserId == telegramUserId);

                if (existing == null)
                {
                    return null;
                }

                // Обновляем поля
                existing.FirstName = user.FirstName;
                existing.LastName = user.LastName;
                existing.Inn = user.Inn;
                existing.Email = user.Email;
                existing.Phone = user.Phone;
                existing.Username = user.Username;
                existing.Status = user.Status;
                existing.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation($"Updated Telegram user with ID: {telegramUserId}");
                return existing;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Error updating Telegram user with ID: {telegramUserId}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(long telegramUserId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                var user = await _context.TelegramUsers
                    .FirstOrDefaultAsync(u => u.TelegramUserId == telegramUserId);

                if (user == null)
                {
                    return false;
                }

                _context.TelegramUsers.Remove(user);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation($"Deleted Telegram user with ID: {telegramUserId}");
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Error deleting Telegram user with ID: {telegramUserId}");
                throw;
            }
        }

        public async Task<TelegramUser?> GetByInnAsync(string inn)
        {
            try
            {
                return await _context.TelegramUsers
                    .FirstOrDefaultAsync(u => u.Inn == inn);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting Telegram user by INN: {inn}");
                throw;
            }
        }

        public async Task<List<TelegramUser>> SearchAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return await GetAllAsync();
                }

                var term = searchTerm.ToLower();
                return await _context.TelegramUsers
                    .Where(u => u.FirstName.ToLower().Contains(term) ||
                               u.LastName.ToLower().Contains(term) ||
                               u.Inn.Contains(term) ||
                               u.Email.ToLower().Contains(term) ||
                               u.Phone.Contains(term) ||
                               (u.Username != null && u.Username.ToLower().Contains(term)))
                    .OrderBy(u => u.TelegramUserId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching Telegram users with term: {searchTerm}");
                throw;
            }
        }
    }
}
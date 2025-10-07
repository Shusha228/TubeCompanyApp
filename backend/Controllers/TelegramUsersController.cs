using Microsoft.AspNetCore.Mvc;
using backend.Models.Entities;
using backend.Services;
using backend.Models.DTOs.Telegram;
using Swashbuckle.AspNetCore.Annotations;

namespace backend.Controllers
{
    
    /// API для управления пользователями Telegram
    
    [ApiController]
    [Route("api/[controller]")]
    [SwaggerTag("Управление пользователями Telegram - создание, чтение, обновление и удаление пользователей")]
    public class TelegramUsersController : ControllerBase
    {
        private readonly ITelegramUserService _telegramUserService;
        private readonly ILogger<TelegramUsersController> _logger;

        public TelegramUsersController(ITelegramUserService telegramUserService,
            ILogger<TelegramUsersController> logger)
        {
            _telegramUserService = telegramUserService;
            _logger = logger;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Получить всех пользователей",
            Description = "Возвращает полный список всех зарегистрированных пользователей Telegram")]
        [SwaggerResponse(200, "Успешный запрос", typeof(List<TelegramUser>))]
        public async Task<ActionResult<List<TelegramUser>>> GetAll()
        {
            try
            {
                var users = await _telegramUserService.GetAllAsync();
                return Ok(new
                {
                    success = true,
                    data = users,
                    count = users.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all Telegram users");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{telegramUserId}")]
        [SwaggerOperation(Summary = "Получить пользователя по ID",
            Description = "Возвращает пользователя по его идентификатору в Telegram")]
        [SwaggerResponse(200, "Пользователь найден", typeof(TelegramUser))]
        [SwaggerResponse(404, "Пользователь не найден")]
        public async Task<ActionResult<TelegramUser>> GetById(
            [SwaggerParameter("ID пользователя в Telegram", Required = true)]
            long telegramUserId)
        {
            try
            {
                var user = await _telegramUserService.GetByIdAsync(telegramUserId);

                if (user == null)
                {
                    return NotFound(new { error = $"Telegram user with ID {telegramUserId} not found" });
                }

                return Ok(new { success = true, data = user });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting Telegram user by ID: {telegramUserId}");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Создать нового пользователя",
            Description = "Создаёт новую запись пользователя Telegram")]
        [SwaggerResponse(201, "Пользователь создан", typeof(TelegramUser))]
        [SwaggerResponse(400, "Неверные данные запроса")]
        public async Task<ActionResult<TelegramUser>> Create(
            [SwaggerParameter("Данные для создания пользователя", Required = true)] [FromBody]
            CreateTelegramUserRequest request)
        {
            try
            {
                var user = new TelegramUser
                {
                    TelegramUserId = request.TelegramUserId,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Inn = request.Inn,
                    Email = request.Email,
                    Phone = request.Phone,
                    Username = request.Username
                };

                var created = await _telegramUserService.CreateAsync(user);

                return CreatedAtAction(nameof(GetById), new { telegramUserId = created.TelegramUserId }, new
                {
                    success = true,
                    data = created,
                    message = "Telegram user created successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Telegram user");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{telegramUserId}")]
        [SwaggerOperation(Summary = "Обновить пользователя",
            Description = "Обновляет данные существующего пользователя Telegram")]
        [SwaggerResponse(200, "Пользователь обновлен", typeof(TelegramUser))]
        [SwaggerResponse(404, "Пользователь не найден")]
        public async Task<ActionResult<TelegramUser>> Update(
            [SwaggerParameter("ID пользователя для обновления", Required = true)]
            long telegramUserId,
            [SwaggerParameter("Новые данные пользователя", Required = true)] [FromBody]
            UpdateTelegramUserRequest request)
        {
            try
            {
                var user = new TelegramUser
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Inn = request.Inn,
                    Email = request.Email,
                    Phone = request.Phone,
                    Username = request.Username,
                    Status = request.Status
                };

                var updated = await _telegramUserService.UpdateAsync(telegramUserId, user);

                if (updated == null)
                {
                    return NotFound(new { error = $"Telegram user with ID {telegramUserId} not found" });
                }

                return Ok(new
                {
                    success = true,
                    data = updated,
                    message = "Telegram user updated successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating Telegram user with ID: {telegramUserId}");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{telegramUserId}")]
        [SwaggerOperation(Summary = "Удалить пользователя", Description = "Удаляет пользователя из системы")]
        [SwaggerResponse(200, "Пользователь удален")]
        [SwaggerResponse(404, "Пользователь не найден")]
        public async Task<ActionResult> Delete(
            [SwaggerParameter("ID пользователя для удаления", Required = true)]
            long telegramUserId)
        {
            try
            {
                var result = await _telegramUserService.DeleteAsync(telegramUserId);

                if (!result)
                {
                    return NotFound(new { error = $"Telegram user with ID {telegramUserId} not found" });
                }

                return Ok(new
                {
                    success = true,
                    message = "Telegram user deleted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting Telegram user with ID: {telegramUserId}");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("inn/{inn}")]
        [SwaggerOperation(Summary = "Получить пользователя по ИНН", Description = "Возвращает пользователя по его ИНН")]
        [SwaggerResponse(200, "Пользователь найден", typeof(TelegramUser))]
        [SwaggerResponse(404, "Пользователь не найден")]
        public async Task<ActionResult<TelegramUser>> GetByInn(
            [SwaggerParameter("ИНН пользователя", Required = true)]
            string inn)
        {
            try
            {
                var user = await _telegramUserService.GetByInnAsync(inn);

                if (user == null)
                {
                    return NotFound(new { error = $"Telegram user with INN {inn} not found" });
                }

                return Ok(new { success = true, data = user });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting Telegram user by INN: {inn}");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Поиск пользователей",
            Description = "Выполняет поиск пользователей по имени, фамилии, ИНН, email или телефону")]
        [SwaggerResponse(200, "Результаты поиска", typeof(List<TelegramUser>))]
        public async Task<ActionResult<List<TelegramUser>>> Search(
            [SwaggerParameter("Поисковый запрос", Required = true)] [FromQuery]
            string term)
        {
            try
            {
                var users = await _telegramUserService.SearchAsync(term);
                return Ok(new
                {
                    success = true,
                    data = users,
                    count = users.Count,
                    searchTerm = term
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching Telegram users with term: {term}");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("test")]
        [SwaggerOperation(Summary = "Тест API",
            Description = "Проверка работоспособности контроллера пользователей Telegram")]
        [SwaggerResponse(200, "API работает корректно")]
        public IActionResult Test()
        {
            return Ok(new
            {
                message = "TelegramUsers controller is working!",
                timestamp = DateTime.UtcNow
            });
        }
    }
}
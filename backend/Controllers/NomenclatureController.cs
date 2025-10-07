using Microsoft.AspNetCore.Mvc;
using backend.Models.Entities;
using backend.Services;
using backend.Models.DTOs.Nomenclature;
using Swashbuckle.AspNetCore.Annotations;

namespace backend.Controllers
{
    /// <summary>
    /// API для управления номенклатурой трубной продукции
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [SwaggerTag("Управление справочником трубной продукции - создание, чтение, обновление и удаление позиций")]
    public class NomenclatureController : ControllerBase
    {
        private readonly INomenclatureService _nomenclatureService;
        private readonly ILogger<NomenclatureController> _logger;

        public NomenclatureController(INomenclatureService nomenclatureService, ILogger<NomenclatureController> logger)
        {
            _nomenclatureService = nomenclatureService;
            _logger = logger;
        }

        /// <summary>
        /// Получить весь список номенклатуры
        /// </summary>
        /// <returns>Список всех позиций номенклатуры</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Получить весь список номенклатуры", Description = "Возвращает полный список всех позиций трубной продукции")]
        [SwaggerResponse(200, "Успешный запрос", typeof(List<Nomenclature>))]
        public async Task<ActionResult<List<Nomenclature>>> GetAll()
        {
            try
            {
                var nomenclatures = await _nomenclatureService.GetAllAsync();
                return Ok(new { 
                    success = true, 
                    data = nomenclatures,
                    count = nomenclatures.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all nomenclature");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Получить список номенклатуры с пагинацией
        /// </summary>
        /// <param name="page">Номер страницы (начиная с 0)</param>
        /// <param name="pageSize">Размер страницы (по умолчанию 20, максимум 100)</param>
        /// <returns>Пагинированный список номенклатуры</returns>
        [HttpGet("paged")]
        [SwaggerOperation(Summary = "Получить список с пагинацией", Description = "Возвращает пагинированный список позиций номенклатуры")]
        [SwaggerResponse(200, "Успешный запрос", typeof(NomenclaturePaginationResponse))]
        public async Task<ActionResult<NomenclaturePaginationResponse>> GetPaged(
            [SwaggerParameter("Номер страницы (начиная с 0)", Required = false)] 
            [FromQuery] int page = 0,
            [SwaggerParameter("Размер страницы (по умолчанию 20, максимум 100)", Required = false)] 
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 0)
                {
                    return BadRequest(new { error = "Page cannot be negative" });
                }

                if (pageSize <= 0 || pageSize > 100)
                {
                    return BadRequest(new { error = "PageSize must be between 1 and 100" });
                }

                var from = page * pageSize;
                var to = from + pageSize;

                var result = await _nomenclatureService.GetPagedAsync(from, to);
                
                return Ok(new { 
                    success = true, 
                    data = result.Items,
                    meta = result.Meta
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting paged nomenclature, page: {page}, pageSize: {pageSize}");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Получить позицию номенклатуры по ID
        /// </summary>
        /// <param name="id">ID позиции номенклатуры</param>
        /// <returns>Позиция номенклатуры</returns>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Получить позицию по ID", Description = "Возвращает конкретную позицию номенклатуры по её идентификатору")]
        [SwaggerResponse(200, "Позиция найдена", typeof(Nomenclature))]
        [SwaggerResponse(404, "Позиция не найдена")]
        public async Task<ActionResult<Nomenclature>> GetById(
            [SwaggerParameter("ID позиции номенклатуры", Required = true)] int id)
        {
            try
            {
                var nomenclature = await _nomenclatureService.GetByIdAsync(id);
                
                if (nomenclature == null)
                {
                    return NotFound(new { error = $"Nomenclature with ID {id} not found" });
                }

                return Ok(new { success = true, data = nomenclature });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting nomenclature by ID: {id}");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Создать новую позицию номенклатуры
        /// </summary>
        /// <param name="request">Данные для создания позиции</param>
        /// <returns>Созданная позиция номенклатуры</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Создать новую позицию", Description = "Создаёт новую запись в справочнике номенклатуры")]
        [SwaggerResponse(201, "Позиция создана", typeof(Nomenclature))]
        [SwaggerResponse(400, "Неверные данные запроса")]
        public async Task<ActionResult<Nomenclature>> Create(
            [SwaggerParameter("Данные для создания позиции", Required = true)] 
            [FromBody] CreateNomenclatureRequest request)
        {
            try
            {
                var nomenclature = new Nomenclature
                {
                    ID = request.ID,
                    IDCat = request.IDCat,
                    IDType = request.IDType,
                    IDTypeNew = request.IDTypeNew,
                    ProductionType = request.ProductionType,
                    IDFunctionType = request.IDFunctionType,
                    Name = request.Name,
                    Gost = request.Gost,
                    FormOfLength = request.FormOfLength,
                    Manufacturer = request.Manufacturer,
                    SteelGrade = request.SteelGrade,
                    Diameter = request.Diameter,
                    ProfileSize2 = request.ProfileSize2,
                    PipeWallThickness = request.PipeWallThickness,
                    Status = request.Status,
                    Koef = request.Koef
                };

                var created = await _nomenclatureService.CreateAsync(nomenclature);
                
                return CreatedAtAction(nameof(GetById), new { id = created.ID }, new { 
                    success = true, 
                    data = created,
                    message = "Nomenclature created successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating nomenclature");
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Обновить позицию номенклатуры
        /// </summary>
        /// <param name="id">ID позиции для обновления</param>
        /// <param name="request">Новые данные позиции</param>
        /// <returns>Обновленная позиция номенклатуры</returns>
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Обновить позицию", Description = "Обновляет данные существующей позиции номенклатуры")]
        [SwaggerResponse(200, "Позиция обновлена", typeof(Nomenclature))]
        [SwaggerResponse(404, "Позиция не найдена")]
        public async Task<ActionResult<Nomenclature>> Update(
            [SwaggerParameter("ID позиции для обновления", Required = true)] int id,
            [SwaggerParameter("Новые данные позиции", Required = true)] 
            [FromBody] UpdateNomenclatureRequest request)
        {
            try
            {
                var nomenclature = new Nomenclature
                {
                    IDCat = request.IDCat,
                    IDType = request.IDType,
                    IDTypeNew = request.IDTypeNew,
                    ProductionType = request.ProductionType,
                    IDFunctionType = request.IDFunctionType,
                    Name = request.Name,
                    Gost = request.Gost,
                    FormOfLength = request.FormOfLength,
                    Manufacturer = request.Manufacturer,
                    SteelGrade = request.SteelGrade,
                    Diameter = request.Diameter,
                    ProfileSize2 = request.ProfileSize2,
                    PipeWallThickness = request.PipeWallThickness,
                    Status = request.Status,
                    Koef = request.Koef
                };

                var updated = await _nomenclatureService.UpdateAsync(id, nomenclature);
                
                if (updated == null)
                {
                    return NotFound(new { error = $"Nomenclature with ID {id} not found" });
                }

                return Ok(new { 
                    success = true, 
                    data = updated,
                    message = "Nomenclature updated successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating nomenclature with ID: {id}");
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Удалить позицию номенклатуры
        /// </summary>
        /// <param name="id">ID позиции для удаления</param>
        /// <returns>Результат операции</returns>
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Удалить позицию", Description = "Удаляет позицию из справочника номенклатуры")]
        [SwaggerResponse(200, "Позиция удалена")]
        [SwaggerResponse(404, "Позиция не найдена")]
        public async Task<ActionResult> Delete(
            [SwaggerParameter("ID позиции для удаления", Required = true)] int id)
        {
            try
            {
                var result = await _nomenclatureService.DeleteAsync(id);
                
                if (!result)
                {
                    return NotFound(new { error = $"Nomenclature with ID {id} not found" });
                }

                return Ok(new { 
                    success = true, 
                    message = "Nomenclature deleted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting nomenclature with ID: {id}");
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Получить номенклатуру по типу продукции
        /// </summary>
        /// <param name="typeId">ID типа продукции</param>
        /// <returns>Список позиций указанного типа</returns>
        [HttpGet("type/{typeId}")]
        [SwaggerOperation(Summary = "Получить по типу продукции", Description = "Возвращает список позиций номенклатуры по указанному типу продукции")]
        [SwaggerResponse(200, "Список позиций по типу", typeof(List<Nomenclature>))]
        public async Task<ActionResult<List<Nomenclature>>> GetByType(
            [SwaggerParameter("ID типа продукции", Required = true)] int typeId)
        {
            try
            {
                var nomenclatures = await _nomenclatureService.GetByTypeAsync(typeId);
                return Ok(new { 
                    success = true, 
                    data = nomenclatures,
                    count = nomenclatures.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting nomenclature by type: {typeId}");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Получить номенклатуру по типу продукции с пагинацией
        /// </summary>
        /// <param name="typeId">ID типа продукции</param>
        /// <param name="page">Номер страницы (начиная с 0)</param>
        /// <param name="pageSize">Размер страницы (по умолчанию 20, максимум 100)</param>
        /// <returns>Пагинированный список позиций указанного типа</returns>
        [HttpGet("type/{typeId}/paged")]
        [SwaggerOperation(Summary = "Получить по типу продукции с пагинацией", Description = "Возвращает пагинированный список позиций номенклатуры по указанному типу продукции")]
        [SwaggerResponse(200, "Пагинированный список позиций по типу", typeof(NomenclaturePaginationResponse))]
        public async Task<ActionResult<NomenclaturePaginationResponse>> GetByTypePaged(
            [SwaggerParameter("ID типа продукции", Required = true)] int typeId,
            [SwaggerParameter("Номер страницы (начиная с 0)", Required = false)] 
            [FromQuery] int page = 0,
            [SwaggerParameter("Размер страницы (по умолчанию 20, максимум 100)", Required = false)] 
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 0)
                {
                    return BadRequest(new { error = "Page cannot be negative" });
                }

                if (pageSize <= 0 || pageSize > 100)
                {
                    return BadRequest(new { error = "PageSize must be between 1 and 100" });
                }

                var from = page * pageSize;
                var to = from + pageSize;

                var result = await _nomenclatureService.GetByTypePagedAsync(typeId, from, to);
                
                return Ok(new { 
                    success = true, 
                    data = result.Items,
                    meta = result.Meta
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting paged nomenclature by type: {typeId}, page: {page}, pageSize: {pageSize}");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Поиск по номенклатуре
        /// </summary>
        /// <param name="term">Поисковый запрос</param>
        /// <returns>Найденные позиции</returns>
        [HttpGet("search")]
        [SwaggerOperation(Summary = "Поиск по номенклатуре", Description = "Выполняет поиск позиций по названию, ГОСТу, марке стали или производителю")]
        [SwaggerResponse(200, "Результаты поиска", typeof(List<Nomenclature>))]
        public async Task<ActionResult<List<Nomenclature>>> Search(
            [SwaggerParameter("Поисковый запрос", Required = true)] 
            [FromQuery] string term)
        {
            try
            {
                var nomenclatures = await _nomenclatureService.SearchAsync(term);
                return Ok(new { 
                    success = true, 
                    data = nomenclatures,
                    count = nomenclatures.Count,
                    searchTerm = term
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching nomenclature with term: {term}");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Поиск по номенклатуре с пагинацией
        /// </summary>
        /// <param name="term">Поисковый запрос</param>
        /// <param name="page">Номер страницы (начиная с 0)</param>
        /// <param name="pageSize">Размер страницы (по умолчанию 20, максимум 100)</param>
        /// <returns>Пагинированные результаты поиска</returns>
        [HttpGet("search/paged")]
        [SwaggerOperation(Summary = "Поиск по номенклатуре с пагинацией", Description = "Выполняет поиск позиций по названию, ГОСТу, марке стали или производителю с пагинацией")]
        [SwaggerResponse(200, "Пагинированные результаты поиска", typeof(NomenclaturePaginationResponse))]
        public async Task<ActionResult<NomenclaturePaginationResponse>> SearchPaged(
            [SwaggerParameter("Поисковый запрос", Required = true)] 
            [FromQuery] string term,
            [SwaggerParameter("Номер страницы (начиная с 0)", Required = false)] 
            [FromQuery] int page = 0,
            [SwaggerParameter("Размер страницы (по умолчанию 20, максимум 100)", Required = false)] 
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term))
                {
                    return BadRequest(new { error = "Search term cannot be empty" });
                }

                if (page < 0)
                {
                    return BadRequest(new { error = "Page cannot be negative" });
                }

                if (pageSize <= 0 || pageSize > 100)
                {
                    return BadRequest(new { error = "PageSize must be between 1 and 100" });
                }

                var from = page * pageSize;
                var to = from + pageSize;

                var result = await _nomenclatureService.SearchPagedAsync(term, from, to);
                
                return Ok(new { 
                    success = true, 
                    data = result.Items,
                    meta = result.Meta
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching paged nomenclature with term: {term}, page: {page}, pageSize: {pageSize}");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Тестовый endpoint
        /// </summary>
        /// <returns>Статус работы API</returns>
        [HttpGet("test")]
        [SwaggerOperation(Summary = "Тест API", Description = "Проверка работоспособности контроллера номенклатуры")]
        [SwaggerResponse(200, "API работает корректно")]
        public IActionResult Test()
        {
            return Ok(new { 
                message = "Nomenclature controller is working!",
                timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Ответ пагинации для номенклатуры
    /// </summary>
    public class NomenclaturePaginationResponse
    {
        /// <summary>
        /// Список позиций номенклатуры
        /// </summary>
        public List<Nomenclature> Items { get; set; } = new List<Nomenclature>();
        
        /// <summary>
        /// Метаданные пагинации
        /// </summary>
        public NomenclaturePaginationMeta Meta { get; set; } = new NomenclaturePaginationMeta();
    }

    /// <summary>
    /// Метаданные пагинации для номенклатуры
    /// </summary>
    public class NomenclaturePaginationMeta
    {
        /// <summary>
        /// Общее количество страниц
        /// </summary>
        public int TotalPages { get; set; }
        
        /// <summary>
        /// Текущая страница
        /// </summary>
        public int Page { get; set; }
        
        /// <summary>
        /// Размер страницы
        /// </summary>
        public int PageLimit { get; set; }
        
        /// <summary>
        /// Общее количество элементов
        /// </summary>
        public int TotalCount { get; set; }
        
        /// <summary>
        /// Поисковый запрос (если применимо)
        /// </summary>
        public string? SearchTerm { get; set; }
        
        /// <summary>
        /// ID типа продукции (если применимо)
        /// </summary>
        public int? TypeId { get; set; }
    }
}
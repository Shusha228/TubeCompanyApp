using Microsoft.AspNetCore.Mvc;
using backend.Models.Entities;
using backend.Services;
using backend.Models.Nomenclature;
using Swashbuckle.AspNetCore.Annotations;

namespace backend.Controllers
{
    
    /// API для управления номенклатурой трубной продукции
    
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
}
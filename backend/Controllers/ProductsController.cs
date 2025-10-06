using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Services;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetProducts([FromQuery] ProductFilter filter)
        {
            try
            {
                var products = await _productService.GetFilteredProductsAsync(filter);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("filters")]
        public async Task<ActionResult<FilterOptions>> GetFilterOptions()
        {
            try
            {
                var options = await _productService.GetFilterOptionsAsync();
                return Ok(options);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("calculate-price")]
        public async Task<ActionResult<PriceCalculationResponse>> CalculatePrice([FromBody] PriceCalculationRequest request)
        {
            try
            {
                var result = await _productService.CalculatePriceAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
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
        var products = await _productService.GetFilteredProductsAsync(filter);
        return Ok(products);
    }

    [HttpGet("filters")]
    public async Task<ActionResult<FilterOptions>> GetFilterOptions()
    {
        var products = await _productService.GetFilteredProductsAsync(new ProductFilter());
        
        return Ok(new FilterOptions
        {
            Warehouses = products.Select(p => p.Warehouse).Distinct().ToList(),
            Types = products.Select(p => p.Type).Distinct().ToList(),
            Diameters = products.Select(p => p.Diameter).Distinct().ToList(),
            WallThicknesses = products.Select(p => p.WallThickness).Distinct().ToList(),
            Standards = products.Select(p => p.Standard).Distinct().ToList(),
            SteelGrades = products.Select(p => p.SteelGrade).Distinct().ToList()
        });
    }

    [HttpPost("calculate-price")]
    public async Task<ActionResult<PriceCalculationResponse>> CalculatePrice([FromBody] PriceCalculationRequest request)
    {
        var price = await _productService.CalculateDiscountedPrice(
            request.ProductId, request.Quantity, request.IsInMeters);
        
        return Ok(new PriceCalculationResponse { FinalPrice = price });
    }
}
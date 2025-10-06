using backend.Models.Entities;

namespace backend.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetFilteredProductsAsync(ProductFilter filter);
        Task<FilterOptions> GetFilterOptionsAsync();
        Task<Models.PriceCalculationResponse> CalculatePriceAsync(Models.PriceCalculationRequest request);
        Task LoadDataAsync();
        Task<Product?> GetProductByIdAsync(int productId);
    }

    public class ProductService : IProductService
    {
        private List<ProductType> _types = new();
        private List<Nomenclature> _nomenclature = new();
        private List<Price> _prices = new();
        private List<Remnant> _remnants = new();
        private List<Stock> _stocks = new();

        public async Task LoadDataAsync()
        {
            var dataPath = Path.Combine(Directory.GetCurrentDirectory(), "Data");
            
            if (File.Exists(Path.Combine(dataPath, "types.json")))
            {
                var typesJson = await File.ReadAllTextAsync(Path.Combine(dataPath, "types.json"));
                _types = System.Text.Json.JsonSerializer.Deserialize<List<ProductType>>(typesJson) ?? new();
            }

            if (File.Exists(Path.Combine(dataPath, "nomenclature.json")))
            {
                var nomJson = await File.ReadAllTextAsync(Path.Combine(dataPath, "nomenclature.json"));
                _nomenclature = System.Text.Json.JsonSerializer.Deserialize<List<Nomenclature>>(nomJson) ?? new();
            }

            if (File.Exists(Path.Combine(dataPath, "prices.json")))
            {
                var pricesJson = await File.ReadAllTextAsync(Path.Combine(dataPath, "prices.json"));
                _prices = System.Text.Json.JsonSerializer.Deserialize<List<Price>>(pricesJson) ?? new();
            }

            if (File.Exists(Path.Combine(dataPath, "remnants.json")))
            {
                var remnantsJson = await File.ReadAllTextAsync(Path.Combine(dataPath, "remnants.json"));
                _remnants = System.Text.Json.JsonSerializer.Deserialize<List<Remnant>>(remnantsJson) ?? new();
            }

            if (File.Exists(Path.Combine(dataPath, "stock.json")))
            {
                var stockJson = await File.ReadAllTextAsync(Path.Combine(dataPath, "stock.json"));
                _stocks = System.Text.Json.JsonSerializer.Deserialize<List<Stock>>(stockJson) ?? new();
            }
        }

        public async Task<List<Product>> GetFilteredProductsAsync(ProductFilter filter)
        {
            var products = await GetCombinedProductsAsync();

            return products.Where(p =>
                (string.IsNullOrEmpty(filter.WarehouseId) || _stocks.Any(s => s.IDStock == filter.WarehouseId && s.StockName == p.Warehouse)) &&
                (string.IsNullOrEmpty(filter.Type) || p.Type.Contains(filter.Type)) &&
                (!filter.Diameter.HasValue || p.Diameter == filter.Diameter.Value) &&
                (!filter.WallThickness.HasValue || p.WallThickness == filter.WallThickness.Value) &&
                (string.IsNullOrEmpty(filter.Standard) || p.Standard.Contains(filter.Standard)) &&
                (string.IsNullOrEmpty(filter.SteelGrade) || p.SteelGrade.Contains(filter.SteelGrade))
            ).ToList();
        }

        public async Task<FilterOptions> GetFilterOptionsAsync()
        {
            var products = await GetCombinedProductsAsync();
            
            return new FilterOptions
            {
                Warehouses = _stocks,
                Types = products.Select(p => p.Type).Distinct().ToList(),
                Diameters = products.Select(p => p.Diameter).Distinct().OrderBy(d => d).ToList(),
                WallThicknesses = products.Select(p => p.WallThickness).Distinct().OrderBy(w => w).ToList(),
                Standards = products.Select(p => p.Standard).Distinct().ToList(),
                SteelGrades = products.Select(p => p.SteelGrade).Distinct().ToList()
            };
        }

        public async Task<Models.PriceCalculationResponse> CalculatePriceAsync(Models.PriceCalculationRequest request)
        {
            var products = await GetCombinedProductsAsync();
            var product = products.FirstOrDefault(p => p.ID == request.ProductId);
            if (product == null)
                throw new ArgumentException("Product not found");

            decimal basePrice, finalPrice;
            decimal quantity = request.Quantity;

            if (request.IsInMeters)
            {
                basePrice = product.PricePerMeter * quantity;
                
                if (product.PriceLimitM2.HasValue && quantity >= product.PriceLimitM2.Value && product.PriceM2.HasValue)
                {
                    finalPrice = product.PriceM2.Value * quantity;
                }
                else if (product.PriceLimitM1.HasValue && quantity >= product.PriceLimitM1.Value && product.PriceM1.HasValue)
                {
                    finalPrice = product.PriceM1.Value * quantity;
                }
                else
                {
                    finalPrice = basePrice;
                }
            }
            else
            {
                basePrice = product.PricePerTon * quantity;
                
                if (product.PriceLimitT2.HasValue && quantity >= product.PriceLimitT2.Value && product.PriceT2.HasValue)
                {
                    finalPrice = product.PriceT2.Value * quantity;
                }
                else if (product.PriceLimitT1.HasValue && quantity >= product.PriceLimitT1.Value && product.PriceT1.HasValue)
                {
                    finalPrice = product.PriceT1.Value * quantity;
                }
                else
                {
                    finalPrice = basePrice;
                }
            }

            var discountPercent = basePrice > 0 ? (basePrice - finalPrice) / basePrice * 100 : 0;

            return new Models.PriceCalculationResponse
            {
                FinalPrice = finalPrice,
                BasePrice = basePrice,
                DiscountPercent = discountPercent
            };
        }

        private async Task<List<Product>> GetCombinedProductsAsync()
        {
            var products = new List<Product>();

            foreach (var nom in _nomenclature)
            {
                var type = _types.FirstOrDefault(t => t.IDType == nom.IDType);
                var prices = _prices.Where(p => p.ID == nom.ID).ToList();
                var remnants = _remnants.Where(r => r.ID == nom.ID).ToList();

                foreach (var price in prices)
                {
                    var remnant = remnants.FirstOrDefault(r => r.IDStock == price.IDStock);
                    var stock = _stocks.FirstOrDefault(s => s.IDStock == price.IDStock);

                    if (remnant != null && stock != null)
                    {
                        var product = new Product
                        {
                            ID = nom.ID,
                            Name = nom.Name,
                            Type = type?.Type ?? "Unknown",
                            Diameter = nom.Diameter,
                            WallThickness = nom.PipeWallThickness,
                            Standard = nom.Gost,
                            SteelGrade = nom.SteelGrade,
                            Warehouse = stock.City,
                            WarehouseName = stock.StockName,
                            PricePerMeter = price.PriceM,
                            PricePerTon = price.PriceT,
                            StockMeters = remnant.InStockM,
                            StockTons = remnant.InStockT,
                            PriceLimitM1 = price.PriceLimitM1,
                            PriceM1 = price.PriceM1,
                            PriceLimitM2 = price.PriceLimitM2,
                            PriceM2 = price.PriceM2,
                            PriceLimitT1 = price.PriceLimitT1,
                            PriceT1 = price.PriceT1,
                            PriceLimitT2 = price.PriceLimitT2,
                            PriceT2 = price.PriceT2,
                            Koef = nom.Koef,
                            AvgTubeLength = remnant.AvgTubeLength,
                            AvgTubeWeight = remnant.AvgTubeWeight
                        };

                        products.Add(product);
                    }
                }
            }

            return products;
        }
        
        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            var products = await GetCombinedProductsAsync();
            return products.FirstOrDefault(p => p.ID == productId);
        }
    }
}
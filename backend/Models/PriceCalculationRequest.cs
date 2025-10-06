namespace backend.Models
{
public class PriceCalculationRequest
{
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
    public bool IsInMeters { get; set; } = true;
}

public class PriceCalculationResponse
{
    public decimal FinalPrice { get; set; }
    public decimal BasePrice { get; set; }
    public decimal DiscountPercent { get; set; }
}
}
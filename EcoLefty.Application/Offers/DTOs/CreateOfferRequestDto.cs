namespace EcoLefty.Application.Offers.DTOs;

public record CreateOfferRequestDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal UnitPrice { get; set; } // unit price (discounted) of a product
    public int TotalQuantity { get; set; } // quantity of a product
    public DateTime StartDateUtc { get; set; }
    public DateTime ExpiryDateUtc { get; set; }
    public int ProductId { get; set; }
}

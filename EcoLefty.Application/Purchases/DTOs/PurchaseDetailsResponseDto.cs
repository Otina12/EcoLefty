using EcoLefty.Application.ApplicationUsers.DTOs;
using EcoLefty.Application.Offers.DTOs;
using EcoLefty.Domain.Common.Enums;

namespace EcoLefty.Application.Purchases.DTOs;

public class PurchaseDetailsResponseDto
{
    public int Id { get; set; }
    public PurchaseStatus PurchaseStatus { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime PurchaseDateUtc { get; set; }
    public virtual OfferDetailsResponseDto Offer { get; set; }
    public virtual ApplicationUserResponseDto Customer { get; set; }
}

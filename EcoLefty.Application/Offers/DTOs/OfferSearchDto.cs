namespace EcoLefty.Application.Offers.DTOs;

public record OfferSearchDto
{
    public string? CompanyId { get; set; }
    public string? SearchText { get; set; }
    public bool OnlyActive { get; set; }
    public bool OnlyFollowedCategories { get; set; }
    public string? SortByColumn { get; set; }
    public bool SortByAscending { get; set; }
    public int? CategoryId { get; set; } // If this is null, don't filter by category
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 6;
}
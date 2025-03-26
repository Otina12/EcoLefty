using Asp.Versioning;
using EcoLefty.Application.Contracts;
using EcoLefty.Application.Offers.DTOs;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EcoLefty.Api.Controllers;

/// <summary>
/// Controller for managing offers.
/// </summary>
[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiversion}/[controller]")]
public class OffersControllerV2 : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public OffersControllerV2(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all offers",
        Description = "Retrieves a list of all offers available in the system.",
        OperationId = "Offers.GetAll"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "List of offers retrieved successfully.", typeof(IEnumerable<OfferDetailsResponseDto>))]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        IEnumerable<OfferDetailsResponseDto> offers = await _serviceManager.OfferService.GetAllAsync(cancellationToken);
        return Ok(offers);
    }

    [HttpGet("active")]
    [SwaggerOperation(
        Summary = "Get all active offers",
        Description = "Retrieves a list of all active offers available in the system.",
        OperationId = "Offers.GetAll"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "List of offers retrieved successfully.", typeof(IEnumerable<OfferDetailsResponseDto>))]
    public async Task<IActionResult> GetAllActive(CancellationToken cancellationToken)
    {
        IEnumerable<OfferDetailsResponseDto> offers = await _serviceManager.OfferService.GetActiveOffersAsync(cancellationToken);
        return Ok(offers);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Get offer by ID",
        Description = "Retrieves an offer using its unique identifier.",
        OperationId = "Offers.GetById"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Offer retrieved successfully.", typeof(OfferDetailsResponseDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Offer not found.")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        OfferDetailsResponseDto offer = await _serviceManager.OfferService.GetByIdAsync(id, cancellationToken);
        return Ok(offer);
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new offer",
        Description = "Creates a new offer with the provided information.",
        OperationId = "Offers.Create"
    )]
    [SwaggerResponse(StatusCodes.Status201Created, "Offer created successfully.", typeof(OfferDetailsResponseDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid offer data.")]
    public async Task<IActionResult> Create([FromBody] CreateOfferRequestDto createDto, CancellationToken cancellationToken)
    {
        OfferDetailsResponseDto createdOffer = await _serviceManager.OfferService.CreateAsync(createDto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = createdOffer.Id }, createdOffer);
    }

    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Update an offer",
        Description = "Updates an existing offer with new values.",
        OperationId = "Offers.Update"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Offer updated successfully.", typeof(OfferDetailsResponseDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Offer not found.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid offer update data.")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateOfferRequestDto updateDto, CancellationToken cancellationToken)
    {
        OfferDetailsResponseDto updatedOffer = await _serviceManager.OfferService.UpdateAsync(id, updateDto, cancellationToken);
        return Ok(updatedOffer);
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Delete an offer",
        Description = "Deletes an offer using its ID.",
        OperationId = "Offers.Delete"
    )]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Offer deleted successfully.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Offer not found.")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _serviceManager.OfferService.DeleteAsync(id, cancellationToken);
        if (deleted)
            return NoContent();
        return NotFound();
    }
}

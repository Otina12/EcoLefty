using EcoLefty.Application;
using EcoLefty.Application.Offers.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EcoLefty.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OffersController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public OffersController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var offers = await _serviceManager.OfferService.GetAllAsync(cancellationToken);
        return Ok(offers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var offer = await _serviceManager.OfferService.GetByIdAsync(id, cancellationToken);
        return Ok(offer);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOfferRequestDto createDto, CancellationToken cancellationToken)
    {
        var createdOffer = await _serviceManager.OfferService.CreateAsync(createDto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = createdOffer.Id }, createdOffer);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateOfferRequestDto updateDto, CancellationToken cancellationToken)
    {
        var updatedOffer = await _serviceManager.OfferService.UpdateAsync(id, updateDto, cancellationToken);
        return Ok(updatedOffer);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _serviceManager.OfferService.DeleteAsync(id, cancellationToken);
        if (deleted)
            return NoContent();
        return NotFound();
    }
}

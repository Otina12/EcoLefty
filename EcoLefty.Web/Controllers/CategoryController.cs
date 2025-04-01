using AutoMapper;
using EcoLefty.Application.Categories.DTOs;
using EcoLefty.Application.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcoLefty.Web.Controllers;

public class CategoryController : Controller
{
    private readonly IServiceManager _serviceManager;
    private readonly IMapper _mapper;

    public CategoryController(IServiceManager serviceManager, IMapper mapper)
    {
        _serviceManager = serviceManager;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index(CancellationToken token)
    {
        if (User.Identity!.IsAuthenticated && User.FindFirst(ClaimTypes.Role)?.Value == "User")
        {
            var curUserId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var userFollowedCategories = await _serviceManager.CategoryService.GetAllFollowedCategoriesByUserIdAsync(curUserId, token);
            ViewBag.FollowedCategoryIDs = userFollowedCategories.Select(x => x.Id).ToList();
        }

        var categories = await _serviceManager.CategoryService.GetAllAsync();
        return View(categories);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Follow([FromBody] FollowUnfollowCategoryRequestDto request, CancellationToken token)
    {
        await _serviceManager.CategoryService.FollowCategory(request.CategoryId, token);
        return Ok();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Unfollow([FromBody] FollowUnfollowCategoryRequestDto request, CancellationToken token)
    {
        await _serviceManager.CategoryService.UnfollowCategory(request.CategoryId, token);
        return Ok();
    }

}
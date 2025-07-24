using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using trainingCenter.Common.Exceptions;
using trainingCenter.Domain.Enums;
using trainingCenter.Domain.Models;
using trainingCenter.Domain.Models.DTOs;
using trainingCenter.Services.Foundation.Interfaces;

namespace trainingCenter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = nameof(Role.Admin))]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService categoryService;
    private readonly IMapper mapper;
    private readonly ICurrentUserService currentUser;

    public CategoriesController(
        ICategoryService categoryService,
        IMapper mapper,
        ICurrentUserService currentUser)
    {
        this.categoryService = categoryService ?? throw new NullArgumentException(nameof(categoryService));
        this.mapper = mapper ?? throw new NullArgumentException(nameof(mapper));
        this.currentUser = currentUser ?? throw new NullArgumentException(nameof(currentUser));
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateDto categoryDto)
    {
        var category = mapper.Map<Category>(categoryDto);
        category.TenantId = currentUser.TenantId;

        var created = await categoryService.RegisterCategoryAsync(category);
        return CreatedAtAction(nameof(GetCategoryById), new { id = created.Id }, mapper.Map<CategoryDto>(created));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        var categories = await categoryService.RetrieveAllCategoriesAsync();
        var filtered = categories.Where(c => c.TenantId == currentUser.TenantId);
        return Ok(mapper.Map<List<CategoryDto>>(filtered));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        var category = await categoryService.RetrieveCategoryByIdAsync(id);
        if (category.TenantId != currentUser.TenantId)
            return Forbid("Bu category boshqa o‘quv markazga tegishli.");

        return Ok(mapper.Map<CategoryDto>(category));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryUpdateDto dto)
    {
        if (id != dto.Id)
            return BadRequest("ID mos kelmadi.");

        var existing = await categoryService.RetrieveCategoryByIdAsync(id);
        if (existing.TenantId != currentUser.TenantId)
            return Forbid("Siz bu categoryga o‘zgartirish kiritolmaysiz.");

        var updated = await categoryService.ModifyCategoryAsync(mapper.Map<Category>(dto));
        return Ok(mapper.Map<CategoryDto>(updated));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var existing = await categoryService.RetrieveCategoryByIdAsync(id);
        if (existing.TenantId != currentUser.TenantId)
            return Forbid("Siz bu categoryni o‘chira olmaysiz.");

        await categoryService.RemoveCategoryAsync(id);
        return NoContent();
    }
}

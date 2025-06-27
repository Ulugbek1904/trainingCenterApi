using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using trainingCenter.Common.Exceptions;
using trainingCenter.Domain.Models;
using trainingCenter.Domain.Models.DTOs;
using trainingCenter.Services.Foundation.Interfaces;
using ArgumentException = trainingCenter.Common.Exceptions.ArgumentException;

namespace trainingCenter.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService categoryService;
        private readonly IMapper mapper;

        public CategoriesController(ICategoryService categoryService, IMapper mapper)
        {
            this.categoryService = categoryService ?? throw new NullArgumentException(nameof(categoryService));
            this.mapper = mapper ?? throw new NullArgumentException(nameof(mapper));
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateDto categoryDto)
        {
            try
            {
                var category = mapper.Map<Category>(categoryDto);
                var createdCategory = await categoryService.RegisterCategoryAsync(category);
                var resultDto = mapper.Map<CategoryDto>(createdCategory);
                return CreatedAtAction(nameof(GetCategoryById), new { id = resultDto.Id }, resultDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await categoryService.RetrieveAllCategoriesAsync();
            var resultDtos = mapper.Map<List<CategoryDto>>(categories);
            return Ok(resultDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await categoryService.RetrieveCategoryByIdAsync(id);
            var resultDto = mapper.Map<CategoryDto>(category);
            return Ok(resultDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryUpdateDto categoryDto)
        {
            if (id != categoryDto.Id)
                throw new ArgumentException("ID mismatch.");

            var category = mapper.Map<Category>(categoryDto);
            var updatedCategory = await categoryService.ModifyCategoryAsync(category);
            var resultDto = mapper.Map<CategoryDto>(updatedCategory);
            return Ok(resultDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await categoryService.RemoveCategoryAsync(id);
            return NoContent();
        }
    }
}
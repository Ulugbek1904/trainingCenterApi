using trainingCenter.Domain.Models;

namespace trainingCenter.Services.Foundation.Interfaces;

public interface ICategoryService
{
    Task<Category> RegisterCategoryAsync(Category category);
    Task<List<Category>> RetrieveAllCategoriesAsync();
    Task<Category> RetrieveCategoryByIdAsync(int categoryId);
    Task<Category> ModifyCategoryAsync(Category category);
    Task<Category> RemoveCategoryAsync(int categoryId);
}

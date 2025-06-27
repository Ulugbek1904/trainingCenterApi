using Microsoft.EntityFrameworkCore;
using trainingCenter.Common.Exceptions;
using trainingCenter.Domain.Models;
using trainingCenter.Infrastructure.brokers.storage;
using trainingCenter.Services.Foundation.Interfaces;
using ArgumentException = trainingCenter.Common.Exceptions.ArgumentException;

namespace trainingCenter.Services.Foundation;

public class CategoryService : ICategoryService
{
    private readonly IStorageBroker storageBroker;

    public CategoryService(IStorageBroker storageBroker)
    {
        this.storageBroker = storageBroker;
    }

    public async Task<Category> RegisterCategoryAsync(Category category)
    {
        ValidateCategory(category);
        if (await storageBroker.SelectAll<Category>()
            .AnyAsync(c => c.Name == category.Name))
        {
            throw new ArgumentException("Category with the same name already exists");
        }

        return await storageBroker.InsertAsync(category);
    }

    public async Task<List<Category>> RetrieveAllCategoriesAsync()
    {
        return await storageBroker.SelectAll<Category>().ToListAsync();
    }

    public async Task<Category> RetrieveCategoryByIdAsync(int categoryId)
    {
        if (categoryId == 0)
            throw new ArgumentException("Category ID cannot be zero");

        var category = await storageBroker.SelectByIdAsync<Category>(categoryId);
        return category ?? throw new NotFoundException($"Category with ID {categoryId} not found");
    }

    public async Task<Category> ModifyCategoryAsync(Category category)
    {
        if (category.Id == 0)
            throw new ArgumentException("Category ID cannot be zero");

        ValidateCategory(category);
        var existing = await storageBroker.SelectByIdAsync<Category>(category.Id);
        return await storageBroker.UpdateAsync(category);
    }

    public async Task<Category> RemoveCategoryAsync(int categoryId)
    {
        if (categoryId == 0)
            throw new ArgumentException("Category ID cannot be zero");

        var category = await storageBroker.SelectByIdAsync<Category>(categoryId);
        return await storageBroker.DeleteAsync(category);
    }

    private static void ValidateCategory(Category category)
    {
        if (category == null || string.IsNullOrEmpty(category.Name))
            throw new ArgumentException("Category cannot be null or have empty Name");
    }
}
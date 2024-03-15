using AnalyticsMicroservice.Contexts;
using AnalyticsMicroservice.Models;
using Microsoft.EntityFrameworkCore;

namespace AnalyticsMicroservice.Services {
    public interface ICategoryService {
        Task<CategoryModel?> GetCategoryById(uint id);
        Task<CategoryModel?> GetCategoryByName(string name);
        Task<IEnumerable<CategoryModel>> SearchCategories(string name, int limit, int offset);
        Task<CategoryModel> AddCategory(CategoryModel model);
        Task<IEnumerable<CategoryModel>> AddCategories(IEnumerable<CategoryModel> models);
    }

    public class CategoryService(ApplicationContext context) : ICategoryService {
        private readonly ApplicationContext _context = context;

        public async Task<CategoryModel?> GetCategoryById(uint id) {
            var category = await _context.Categories.FirstOrDefaultAsync(data => data.Id == id);
            return category;
        }

        public async Task<CategoryModel?> GetCategoryByName(string name) {
            var category = await _context.Categories.FirstOrDefaultAsync(data => data.Name == name);
            return category;
        }

        public async Task<IEnumerable<CategoryModel>> SearchCategories(string name, int limit, int offset) {
            var categories = await _context.Categories.Where(data => EF.Functions.Like(data.Name.ToLower(), $"%{name.ToLower()}%")).OrderBy(data => data.Id).Take(limit).Skip(offset).ToListAsync();
            return categories;
        }

        public async Task<CategoryModel> AddCategory(CategoryModel model) {
            var category = await _context.Categories.AddAsync(model);
            await _context.SaveChangesAsync();
            return category.Entity;
        }

        public async Task<IEnumerable<CategoryModel>> AddCategories(IEnumerable<CategoryModel> models) {
            await _context.Categories.AddRangeAsync(models);
            await _context.SaveChangesAsync();
            return models;
        }
    }
}

using AnalyticsMicroservice.Contexts;
using AnalyticsMicroservice.Models;
using MassTransit.Middleware;
using Microsoft.EntityFrameworkCore;

namespace AnalyticsMicroservice.Services {
    public interface IPriceService {
        Task<PriceModel?> GetPriceByNames(string categoryName, string locationName);
        Task<PriceModel> AddPrice(PriceModel model);
    }

    public class PriceService(ApplicationContext context, ICategoryService categoryService, ILocationService locationService, IMatrixService matrixService) : IPriceService {
        private readonly ApplicationContext _context = context;
        private readonly ICategoryService _categoryService = categoryService;
        private readonly ILocationService _locationService = locationService;
    
        public async Task<PriceModel?> GetPriceByNames(string categoryName, string locationName) {
            var category = await _categoryService.GetCategoryByName(categoryName);
            var location = await _locationService.GetLocationByName(locationName);
            if (category == null || location == null) return null;
            while (true) {
                while (true) {
                    var price = await _context.Prices.FirstOrDefaultAsync(data => data.LocationId == location.Id && data.CategoryId == category.Id);
                    if (price != null) return price;

                    if (location.ParentId == null) break;
                    location = await _locationService.GetLocationById(Convert.ToUInt32(location.ParentId));
                }
                if (category.ParentId == null) break;
                category = await _categoryService.GetCategoryById(Convert.ToUInt32(category.ParentId));
            }
            return null;
        }

        public async Task<PriceModel> AddPrice(PriceModel model) {
            var price = await _context.Prices.AddAsync(model);
            await _context.SaveChangesAsync();
            return price.Entity;
        }
    }
}

using AnalyticsMicroservice.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Interfaces.Analytics;
using Shared.Results;

namespace AnalyticsMicroservice.Controllers {
    [Route("api/analytics/price")]
    [ApiController]
    public class PriceController(IPriceService priceService, IConfiguration configuration, ICategoryService categoryService, ILocationService locationService, IMatrixService matrixService) : ControllerBase {
        private readonly IPriceService _priceService = priceService;
        private readonly IConfiguration _configuration = configuration;
        private readonly ICategoryService _categoryService = categoryService;
        private readonly ILocationService _locationService = locationService;
        private readonly IMatrixService _matrixService = matrixService;

        [HttpGet("get")]
        public async Task<ActionResult<ApiResult<IPrice>>> GetPrice(string categoryName, string locationName) {
            var generator = new ApiResult<IPrice>.Generator();

            var price = await _priceService.GetPriceByNames(categoryName, locationName);
            if (price == null) return BadRequest(generator.Error("Price not found"));

            var location = await _locationService.GetLocationById(price.LocationId);
            var category = await _categoryService.GetCategoryById(price.CategoryId);
            var matrix = await _matrixService.GetMatrixById(price.MatrixId);

            return Ok(generator.Success(new() {
                Id = price.Id,
                Category = new() {
                    Id = category.Id,
                    Name = category.Name,
                    ParentId = Convert.ToUInt32(category.ParentId)
                },
                Location = new() {
                    Id = location.Id,
                    Name = location.Name,
                    ParentId = Convert.ToUInt32(location.ParentId)
                },
                Matrix = new() {
                    Id = matrix.Id,
                    Name = matrix.Name,
                    UserId = matrix.UserId
                },
                
            }));
        }

        [HttpPost("add")]
        public async Task<ActionResult<ApiResult<IPrice>>> AddPrice([FromBody] IAddPriceHttpRequest data) {
            var generator = new ApiResult<IPrice>.Generator();

            var matrix = await _matrixService.GetMatrixByName(data.MatrixName);
            if (matrix == null) return BadRequest(generator.Error("Matrix not found"));
            var location = await _locationService.GetLocationByName(data.LocationName);
            if (location == null) return BadRequest(generator.Error("Location not found"));
            var category = await _categoryService.GetCategoryByName(data.CategoryName);
            if (category == null) return BadRequest(generator.Error("Category not found"));

            var price = await _priceService.AddPrice(new() {
                CategoryId = category.Id,
                LocationId = location.Id,
                MatrixId = matrix.Id,
                Price = data.Price
            });

            return Ok(generator.Success(new() {
                Id = price.Id,
                Category = new() {
                    Id = category.Id,
                    Name = category.Name,
                    ParentId = Convert.ToUInt32(category.ParentId)
                },
                Location = new() {
                    Id = location.Id,
                    Name = location.Name,
                    ParentId = Convert.ToUInt32(location.ParentId)
                },
                Matrix = new() {
                    Id = matrix.Id,
                    Name = matrix.Name,
                    UserId = matrix.UserId
                }
            }));
        }
    }
}

using AnalyticsMicroservice.Models;
using AnalyticsMicroservice.Services;
using MassTransit.Initializers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto;
using Shared.Configuration.Extensions;
using Shared.Interfaces.Analytics;
using Shared.Results;

namespace AnalyticsMicroservice.Controllers {
    [Route("api/analytics/categories")]
    [ApiController]
    public class CategoryController(ICategoryService categoryService, IConfiguration configuration) : ControllerBase {
        private readonly ICategoryService _categoryService = categoryService;
        private readonly IConfiguration _configuration = configuration;

        [HttpGet("get")]
        public async Task<ActionResult<ApiResult<ICategory>>> GetCategory(string name) {
            var generator = new ApiResult<ICategory>.Generator();

            var category = await _categoryService.GetCategoryByName(name);

            if (category == null) return BadRequest(generator.Error("Category not found"));

            return Ok(generator.Success(new() {
                Id = category.Id,
                Name = category.Name,
                ParentId = Convert.ToUInt32(category.ParentId)
            }));
        }

        [HttpGet("search")]
        public async Task<ActionResult<ApiResult<IEnumerable<ICategory>>>> SearchCategories(string name, int limit = 10, int offset = 0) {
            var generator = new ApiResult<IEnumerable<ICategory>>.Generator();

            var categories = await _categoryService.SearchCategories(name, limit, offset);

            return Ok(generator.Success(categories.Select(category => new ICategory() {
                Id = category.Id,
                Name = category.Name,
                ParentId = Convert.ToUInt32(category.ParentId)
            })));
        }

        [HttpPost("add")]
        public async Task<ActionResult<ApiResult<IEnumerable<ICategory>>>> AddCategory([FromBody] IAddCategoryHttpRequest data) {
            var generator = new ApiResult<IEnumerable<ICategory>>.Generator();

            var userId = Convert.ToUInt32(Request.Headers[_configuration.GetOrThrow("ApiGateway:Http:Headers:UserId")]);

            var parentCategory = await _categoryService.GetCategoryByName(data.ParentName);
            if (parentCategory == null) return BadRequest(generator.Error("Parent category not found"));

            var categories = await _categoryService.AddCategories(data.Names.Select(name => new CategoryModel() {
                Name = name,
                ParentId = parentCategory.Id,
                UserId = userId
            }));

            return Ok(generator.Success(categories.Select(data => new ICategory() {
                Id = data.Id,
                Name = data.Name,
                ParentId = Convert.ToUInt32(data.ParentId),
                UserId = userId
            })));
        }
    }
}

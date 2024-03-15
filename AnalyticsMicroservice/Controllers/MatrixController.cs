using AnalyticsMicroservice.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Configuration.Extensions;
using Shared.Interfaces.Analytics;
using Shared.Results;

namespace AnalyticsMicroservice.Controllers {
    [Route("api/analytics/matrices")]
    [ApiController]
    public class MatrixController(IMatrixService matrixService, IConfiguration configuration) : ControllerBase {
        private readonly IMatrixService _matrixService = matrixService;
        private readonly IConfiguration _configuration = configuration;

        [HttpGet("get")]
        public async Task<ActionResult<ApiResult<IMatrix>>> GetMatrix(string name) {
            var generator = new ApiResult<IMatrix>.Generator();
            var matrix = await _matrixService.GetMatrixByName(name);

            if (matrix == null) return BadRequest(generator.Error("Location not found"));

            return Ok(generator.Success(new() {
                Id = matrix.Id,
                Name = matrix.Name,
                UserId = matrix.UserId,
            }));
        }

        [HttpGet("search")]
        public async Task<ActionResult<ApiResult<IEnumerable<IMatrix>>>> SearchMatrices(string name, int limit = 10, int offset = 0) {
            var generator = new ApiResult<IEnumerable<IMatrix>>.Generator();

            var matrices = await _matrixService.SearchMatrix(name, limit, offset);

            return Ok(generator.Success(matrices.Select(matrix => new IMatrix() {
                Id = matrix.Id,
                Name = matrix.Name,
                UserId = matrix.UserId
            })));
        }

        [HttpPost("add")]
        public async Task<ActionResult<ApiResult<IMatrix>>> AddMatrix([FromBody] IAddMatrixHttpRequest data) {
            var generator = new ApiResult<IMatrix>.Generator();

            var userId = Convert.ToUInt32(Request.Headers[_configuration.GetOrThrow("ApiGateway:Http:Headers:UserId")]);

            var category = await _matrixService.AddMatrix(new() {
                Name = data.Name,
                UserId = userId
            });

            return Ok(generator.Success(new() {
                Id = category.Id,
                Name = category.Name,
                UserId = userId
            }));
        }
    }
}

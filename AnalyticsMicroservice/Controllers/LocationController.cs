using AnalyticsMicroservice.Models;
using AnalyticsMicroservice.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Shared.Configuration.Extensions;
using Shared.Interfaces.Analytics;
using Shared.Results;
using System.Text;
using System.Web;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AnalyticsMicroservice.Controllers {
    [Route("api/analytics/locations")]
    [ApiController]
    public class LocationController(ILocationService locationService, IConfiguration configuration) : ControllerBase {
        private readonly ILocationService _locationService = locationService;
        private readonly IConfiguration _configuration = configuration;

        [HttpGet("get")]
        public async Task<ActionResult<ApiResult<ILocation>>> GetLocation(string name) {
            var generator = new ApiResult<ILocation>.Generator();
            var location = await _locationService.GetLocationByName(name);

            if (location == null) return BadRequest(generator.Error("Location not found"));

            return Ok(generator.Success(new() {
                Id = location.Id,
                Name = location.Name,
                ParentId = Convert.ToUInt32(location.ParentId)
            }));
        }

        [HttpGet("search")]
        public async Task<ActionResult<ApiResult<IEnumerable<ILocation>>>> SearchLocations(string name, int limit = 10, int offset = 0) {
            var generator = new ApiResult<IEnumerable<ILocation>>.Generator();

            var locations = await _locationService.SearchLocations(name, limit, offset);

            return Ok(generator.Success(locations.Select(location => new ILocation() {
                Id = location.Id,
                Name = location.Name,
                ParentId = Convert.ToUInt32(location.ParentId)
            })));
        }

        [HttpPost("add")]
        public async Task<ActionResult<ApiResult<IEnumerable<ILocation>>>> AddLocation([FromBody] IAddLocationHttpRequest data) {
            var generator = new ApiResult<IEnumerable<ILocation>>.Generator();

            var userId = Convert.ToUInt32(Request.Headers[_configuration.GetOrThrow("ApiGateway:Http:Headers:UserId")]);

            var parentLocation = await _locationService.GetLocationByName(data.ParentName);
            if (parentLocation == null) return BadRequest(generator.Error("Parent category not found"));

            var locations = await _locationService.AddLocations(data.Names.Select(name => new LocationModel() {
                Name = name,
                ParentId = parentLocation.Id,
                UserId = userId
            }));

            return Ok(generator.Success(locations.Select(category => new ILocation() {
                Id = category.Id,
                Name = category.Name,
                ParentId = Convert.ToUInt32(category.ParentId),
                UserId = category.UserId
            })));
        }
    }
}

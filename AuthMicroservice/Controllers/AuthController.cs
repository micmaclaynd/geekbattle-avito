using AuthMicroservice.Services;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Shared.Configuration.Extensions;
using Shared.Interfaces.Auth;
using Shared.Results;

namespace AuthMicroservice.Controllers {
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IConfiguration configuration, IAuthService authService) : ControllerBase {
        private readonly IConfiguration _configuration = configuration;
        private readonly IAuthService _authService = authService;

        [HttpPost("login")]
        public async Task<ActionResult<ApiResult<ILoginHttpResponse>>> Login([FromBody] ILoginHttpRequest data) {
            var generator = new ApiResult<ILoginHttpResponse>.Generator();
            var token = await _authService.Login(data.Username, data.Password);

            if (!token.IsSuccess) {
                return BadRequest(generator.Error(token.Error));
            }

            HttpContext.Response.Cookies.Append(_configuration.GetOrThrow("ApiGateway:Http:Cookies:AuthToken"), token.Result);

            return Ok(generator.Success(new() {
                Token = token.Result
            }));
        }

        [HttpPost("register")]
        public async Task<ActionResult<IRegisterHttpResponse>> Register([FromBody] IRegisterHttpRequest data) {
            var generator = new ApiResult<IRegisterHttpResponse>.Generator();
            var token = await _authService.Register(data.Username, data.Password);

            if (!token.IsSuccess) {
                return BadRequest(generator.Error(token.Error));
            }

            HttpContext.Response.Cookies.Append(_configuration.GetOrThrow("ApiGateway:Http:Cookies:AuthToken"), token.Result);

            return Ok(generator.Success(new() {
                Token = token.Result
            }));
        }
    }
}
using AuthMicroservice.Services;
using MassTransit;
using Shared.Interfaces.Auth;
using Shared.Interfaces.Users;
using Shared.Results;

namespace AuthMicroservice.Consumers {
    public class LoginConsumer(IAuthService authService) : IConsumer<ILoginRequest> {
        private readonly IAuthService _authService = authService;

        public async Task Consume(ConsumeContext<ILoginRequest> context) {
            var token = await _authService.Login(context.Message.Username, context.Message.Password);

            if (!token.IsSuccess) {
                await context.RespondAsync<IServiceError>(new() {
                    Error = token.Error
                });
            } else {
                await context.RespondAsync<ILoginResponse>(new() {
                    Token = token.Result
                });
            }
        }
    }
}

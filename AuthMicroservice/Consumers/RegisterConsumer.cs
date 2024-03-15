using AuthMicroservice.Services;
using MassTransit;
using Shared.Interfaces.Auth;
using Shared.Results;

namespace AuthMicroservice.Consumers {
    public class RegisterConsumer(IAuthService authService) : IConsumer<IRegisterRequest> {
        private readonly IAuthService _authService = authService;

        public async Task Consume(ConsumeContext<IRegisterRequest> context) {
            var token = await _authService.Register(context.Message.Username, context.Message.Password);

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

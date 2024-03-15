using AuthMicroservice.Services;
using MassTransit;
using Shared.Interfaces.Auth;
using Shared.Results;

namespace AuthMicroservice.Consumers {
    public class VerifyConsumer(IAuthService authService) : IConsumer<IVerifyRequest> {
        private readonly IAuthService _authService = authService;

        public async Task Consume(ConsumeContext<IVerifyRequest> context) {
            var payload = _authService.Verify(context.Message.Token);

            if (!payload.IsSuccess) {
                await context.RespondAsync<IServiceError>(new() {
                    Error = payload.Error
                });
            } else {
                await context.RespondAsync<IVerifyResponse>(new() {
                    TokenPayload = payload.Result
                });
            }
        }
    }
}

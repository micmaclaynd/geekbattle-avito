using MassTransit;
using Shared.Interfaces.Users;
using UsersMicroservice.Services;
using Shared.Results;

namespace UsersMicroservice.Consumers {
    public class GetUserByIdConsumer(IUserService userService) : IConsumer<IGetUserByIdRequest> {
        private readonly IUserService _userService = userService;

        public async Task Consume(ConsumeContext<IGetUserByIdRequest> context) {
            var user = await _userService.GetUserById(context.Message.Id);

            if (!user.IsSuccess) {
                await context.RespondAsync<IServiceError>(new() {
                    Error = user.Error
                });
            } else {
                await context.RespondAsync<IGetUserByIdResponse>(new() {
                    Id = user.Result.Id,
                    Username = user.Result.Username,
                    Password = user.Result.Password
                });
            }
        }
    }

    public class GetUserByUsernameConsumer(IUserService userService) : IConsumer<IGetUserByUsernameRequest> {
        private readonly IUserService _userService = userService;

        public async Task Consume(ConsumeContext<IGetUserByUsernameRequest> context) {
            var user = await _userService.GetUserByUsername(context.Message.Username);

            if (!user.IsSuccess) {
                await context.RespondAsync<IServiceError>(new() {
                    Error = user.Error
                });
            } else {
                await context.RespondAsync<IGetUserByUsernameResponse>(new() {
                    Id = user.Result.Id,
                    Username = user.Result.Username,
                    Password = user.Result.Password
                });
            }
        }
    }
}
using MassTransit;
using Shared.Interfaces.Users;
using UsersMicroservice.Services;

namespace UsersMicroservice.Consumers {
    public class CreateUserConsumer(IUserService userService) : IConsumer<ICreateUserRequest> {
        private readonly IUserService _userService = userService;

        public async Task Consume(ConsumeContext<ICreateUserRequest> context) {
            var user = await _userService.CreateUser(new() {
                Username = context.Message.Username,
                Password = context.Message.Password
            });

            await context.RespondAsync<ICreateUserResponse>(new() {
                Id = user.Result.Id,
                Username = user.Result.Username,
                Password = user.Result.Password
            });
        }
    }
}

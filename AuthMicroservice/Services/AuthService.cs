using AuthMicroservice.Utils;
using MassTransit;
using Shared.Configuration.Extensions;
using Shared.Interfaces.Auth;
using Shared.Interfaces.Users;
using Shared.Results;

namespace AuthMicroservice.Services {
    public interface IAuthService {
        Task<ServiceResult<string>> Login(string username, string password);
        Task<ServiceResult<string>> Register(string username, string password);
        ServiceResult<ITokenPayload> Verify(string token);
    }

    public class AuthService(IConfiguration configuration, IRequestClient<IGetUserByUsernameRequest> getUserByUsernameClient, IRequestClient<ICreateUserRequest> createUserClient, IJwtService jwtService) : IAuthService {
        private readonly IRequestClient<IGetUserByUsernameRequest> _getUserByUsernameClient = getUserByUsernameClient;
        private readonly IRequestClient<ICreateUserRequest> _createUserClient = createUserClient;
        private readonly IConfiguration _configuration = configuration;
        private readonly IJwtService _jwtService = jwtService;

        public ServiceResult<ITokenPayload> Verify(string token) {
            var generator = new ServiceResult<ITokenPayload>.Generator();

            try {
                var verifyToken = _jwtService.VerifyToken(token);
                return generator.Success(verifyToken);
            } catch {
                return generator.Error("Bad token");
            }
        }

        public async Task<ServiceResult<string>> Login(string username, string password) {
            var generator = new ServiceResult<string>.Generator();

            var getUserResponse = await _getUserByUsernameClient.GetResponse<IGetUserByUsernameResponse, IServiceError>(new() {
                Username = username
            }, responseConfig => {
                responseConfig.UseExecute(executeConfig => {
                    executeConfig.SetRoutingKey(_configuration.GetOrThrow("Users:Endpoints:GetUserByUsername:RoutingKey"));
                });
            });

            if (getUserResponse.Is(out Response<IServiceError> error)) {
                return generator.Error(error.Message.Error);
            } else if (getUserResponse.Is(out Response<IGetUserByUsernameResponse> user)) {
                if (Password.Verify(password, user.Message.Password)) {
                    return generator.Success(_jwtService.GenerateToken(new() {
                        UserId = user.Message.Id
                    }));
                } else {
                    return generator.Error("Wrong password");
                }
            }

            throw new InvalidOperationException();
        }

        public async Task<ServiceResult<string>> Register(string username, string password) {
            var generator = new ServiceResult<string>.Generator();

            var getUserResponse = await _getUserByUsernameClient.GetResponse<IGetUserByUsernameResponse, IServiceError>(new() {
                Username = username
            }, responseConfig => {
                responseConfig.UseExecute(executeConfig => {
                    executeConfig.SetRoutingKey(_configuration.GetOrThrow("Users:Endpoints:GetUserByUsername:RoutingKey"));
                });
            });

            if (getUserResponse.Is(out Response<IServiceError> getUserError)) {
                var createUserResponse = await _createUserClient.GetResponse<ICreateUserResponse, IServiceError>(new() {
                    Username = username,
                    Password = Password.Hash(password)
                });

                if (createUserResponse.Is(out Response<ICreateUserResponse> createUser)) {
                    return generator.Success(_jwtService.GenerateToken(new() {
                        UserId = createUser.Message.Id
                    }));
                }
            } else if (getUserResponse.Is(out Response<IGetUserByUsernameResponse> getUser)) {
                return generator.Error("User is already registered");
            }

            throw new InvalidOperationException();
        }
    }
}

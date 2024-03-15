using UsersMicroservice.Contexts;
using UsersMicroservice.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Results;

namespace UsersMicroservice.Services
{
    public interface IUserService {
        Task<ServiceResult<UserModel?>> GetUserById(uint id);
        Task<ServiceResult<UserModel?>> GetUserByUsername(string username);
        Task<ServiceResult<UserModel>> CreateUser(UserModel userData);
    }

    public class UserService(ApplicationContext context) : IUserService {
        private readonly ApplicationContext _context = context;

        public async Task<ServiceResult<UserModel?>> GetUserById(uint id) {
            var generator = new ServiceResult<UserModel?>.Generator();

            var user = await _context.Users.FirstOrDefaultAsync(data => data.Id == id);

            if (user == null) {
                return generator.Error("User not found");
            }

            return generator.Success(user);
        }

        public async Task<ServiceResult<UserModel?>> GetUserByUsername(string username) {
            var generator = new ServiceResult<UserModel?>.Generator();

            var user = await _context.Users.FirstOrDefaultAsync(data => data.Username == username);

            if (user == null) {
                return generator.Error("User not found");
            }

            return generator.Success(user);
        }

        public async Task<ServiceResult<UserModel>> CreateUser(UserModel userData) {
            var generator = new ServiceResult<UserModel>.Generator();

            var user = await _context.Users.AddAsync(userData);
            await _context.SaveChangesAsync();

            return generator.Success(user.Entity);
        }
    }
}

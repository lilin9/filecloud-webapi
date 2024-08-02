using Domain.Entities;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.RepositoryImpl {
    public class UserRepository(SqlServerDbContext dbContext): IUserRepository {
        public Task<UserInfo?> FindOneByUserNameAsync(string userName) {
            return dbContext.UserInfos.SingleOrDefaultAsync(u => u.UserName == userName);
        }

        public Task<UserInfo?> FindOneByEmailAsync(string email) {
            return dbContext.UserInfos.SingleOrDefaultAsync(u => u.Email == email);
        }

        public Task<UserInfo?> FindOneByUserIdAsync(Guid userId) {
            return dbContext.UserInfos.SingleOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<UserInfo?> SaveOneAsync(UserInfo userInfo) {
            var result = await dbContext.UserInfos.AddAsync(userInfo);
            return result.Entity;
        }
    }
}

using Domain.Entities;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.RepositoryImpl {
    public class UserRepository(SqlServerDbContext dbContext, IConfiguration configuration): IUserRepository {
        private readonly string _basicAvatarPath = configuration["CustomStrings:BasicFilePath"]! + "/Avatar/";
        private readonly string _onlineFilePath = configuration["CustomStrings:OnlineFilePath"]! + "/Avatar/";

        public async Task<UserInfo?> FindOneByUserNameAsync(string userName) {
            var user = await dbContext.UserInfos.SingleOrDefaultAsync(u => u.UserName == userName);
            user?.CompletePath(_basicAvatarPath, _onlineFilePath);
            return user;
        }

        public async Task<UserInfo?> FindOneByEmailAsync(string email) {
            var user = await dbContext.UserInfos.SingleOrDefaultAsync(u => email == u.Email);
            user?.CompletePath(_basicAvatarPath, _onlineFilePath);
            return user;
        }

        public async Task<UserInfo?> FindOneByUserIdAsync(Guid userId) {
            var user = await dbContext.UserInfos.SingleOrDefaultAsync(u => u.UserId == userId);
            user?.CompletePath(_basicAvatarPath, _onlineFilePath);
            return user;
        }

        public async Task<UserInfo?> SaveOneAsync(UserInfo userInfo) {
            var result = await dbContext.UserInfos.AddAsync(userInfo);
            return result.Entity;
        }

        public UserInfo UpdateOne(UserInfo userInfo) {
            return dbContext.UserInfos.Update(userInfo).Entity;
        }
    }
}

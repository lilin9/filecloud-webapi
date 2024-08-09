using Domain.Entities;

namespace Domain.Repository {
    public interface IUserRepository {
        /// <summary>
        /// 根据用户名查询单个用户
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public Task<UserInfo?> FindOneByUserNameAsync(string userName);

        /// <summary>
        /// 根据邮箱查询单个用户
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task<UserInfo?> FindOneByEmailAsync(string email);

        /// <summary>
        /// 根据用户Id查询单个用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<UserInfo?> FindOneByUserIdAsync(Guid userId);

        /// <summary>
        /// 保存单个用户
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public Task<UserInfo?> SaveOneAsync(UserInfo userInfo);

        /// <summary>
        /// 修改单个用户
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public UserInfo UpdateOne(UserInfo userInfo);
    }
}

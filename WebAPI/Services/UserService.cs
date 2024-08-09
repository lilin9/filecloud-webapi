using System.Data;
using System.Data.SqlTypes;
using Domain;
using Domain.Entities;
using Domain.Repository;
using Domain.Vo;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Services {
    public class UserService(IUserRepository userRepository, SqlServerDbContext dbContext) {
        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="userVo"></param>
        /// <returns></returns>
        public async Task<UserInfo?> Register(UserInfoVm userVo) {
            //用户名和邮箱是否已经注册
            if (userRepository.FindOneByUserNameAsync(userVo.UserName).Result != null) {
                throw new CustomReplyException("用户名已存在");
            }

            if (userRepository.FindOneByEmailAsync(userVo.Email).Result != null) {
                throw new CustomReplyException("邮箱已经被注册");
            }


            var userInfo = new UserInfo(userVo.UserName, userVo.Email, userVo.Password);
            return await userRepository.SaveOneAsync(userInfo);
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="userVo"></param>
        /// <returns></returns>
        /// <exception cref="DataException"></exception>
        public async Task<UserInfo?> Login(UserInfoVm userVo) {
            //验证邮箱是否存在
            var userInfo = await userRepository.FindOneByEmailAsync(userVo.Email);
            if (userInfo == null) {
                throw new CustomReplyException("请先注册邮箱");
            }

            return userInfo.CheckPassword(userVo.Password) ? userInfo : null;
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="email"></param
        /// <param name="password"></param
        /// <returns></returns>
        /// <exception cref="CustomReplyException"></exception>
        public async Task<UserInfo> ResetPassword(string email, string password) {
            var userInfo = await userRepository.FindOneByEmailAsync(email);
            if (userInfo == null) {
                throw new CustomReplyException("当前用户没有注册信息");
            }

            userInfo.LockPass(password);
            await dbContext.UserInfos
                .Where(u => u.UserId == userInfo.UserId)
                .ExecuteUpdateAsync(u => u.SetProperty(p => p.Password, userInfo.Password));
            return userInfo;
        }

        /// <summary>
        /// 重置邮箱
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        /// <exception cref="CustomReplyException"></exception>
        public async Task<bool> ResetEmail(UserInfo userInfo) {
            var result = await userRepository.FindOneByUserIdAsync(userInfo.UserId);
            if (result == null) {
                throw new CustomReplyException("当前用户没有注册信息");
            }

            if (await userRepository.FindOneByEmailAsync(userInfo.Email) != null) {
                throw new CustomReplyException("邮箱已经被注册");
            }

            var row = await dbContext.UserInfos
                .Where(u => u.UserId == userInfo.UserId)
                .ExecuteUpdateAsync(u => u.SetProperty(e => e.Email, userInfo.Email));
            return row > 0;
        }

        /// <summary>
        /// 确认密码是否正确
        /// </summary>
        /// <param name="userInfoVo"></param>
        /// <returns></returns>
        public async Task<bool> CheckPassword(UserInfoVm userInfoVo) {
            var user =await userRepository.FindOneByUserIdAsync(Guid.Parse(userInfoVo.UserId!));
            return user?.CheckPassword(userInfoVo.Password) ?? false;
        }

        /// <summary>
        /// 重置用户头像
        /// </summary>
        /// <param name="avatarFile"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<UserInfo> ResetAvatar(IFormFile avatarFile, Guid userId) {
            try
            {
                var user = await userRepository.FindOneByUserIdAsync(userId);
                if (user == null) {
                    throw new SqlNullValueException("用户数据不存在");
                }

                user.UpdateAvatar(avatarFile);
                user = userRepository.UpdateOne(user);
                return user;
            }
            catch (Exception e) {
                throw new CustomReplyException("上传文件失败，请重试");
            }
        }
    }
}
     
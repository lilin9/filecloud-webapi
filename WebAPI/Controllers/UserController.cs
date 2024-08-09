using Domain;
using Domain.Entities;
using Domain.Entities.SalveModel;
using Domain.ViewModel;
using Domain.Vo;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Attributes;
using WebAPI.Services;

namespace WebAPI.Controllers {
    [ApiController]
    [Route("user")]
    public class UserController(UserService userService, IConfiguration configuration): ControllerBase {
        private readonly string _basicFilePath = configuration["CustomStrings:BasicFilePath"]!;


        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="userVo"></param>
        /// <returns></returns>
        [HttpPost("register")]
        [UnityOfWork(typeof(SqlServerDbContext))]
        public ActionResult Register(UserInfoVm userVo) {
            if (string.IsNullOrEmpty(userVo.UserName)) {
                throw new CustomReplyException("用户名必须提供");
            }

            if (string.IsNullOrEmpty(userVo.Email)) {
                throw new CustomReplyException("邮箱必须提供");
            }

            if (string.IsNullOrEmpty(userVo.Password)) {
                throw new CustomReplyException("密码必须提供");
            }

            var result = userService.Register(userVo).Result;
            return result == null ? BadRequest("注册失败，请重试") : Ok();
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="userVo"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public ActionResult Login(UserInfoVm userVo) {
            if (string.IsNullOrEmpty(userVo.Password)) {
                throw new CustomReplyException("密码必须提供");
            }

            if (string.IsNullOrEmpty(userVo.Email)) {
                throw new CustomReplyException("邮箱必须提供");
            }

            var result = userService.Login(userVo).Result;
            return result == null ? BadRequest("用户名或密码错误，请重试") : Ok(result);
        }

        /// <summary>
        /// 密码重置
        /// </summary>
        /// <param name="userInfoVm"></param>
        /// <returns></returns>
        [HttpPost("resetPass")]
        [UnityOfWork(typeof(SqlServerDbContext))]
        public ActionResult ResetPassword(UserInfoVm userInfoVm) {
            if (string.IsNullOrEmpty(userInfoVm.Email)) {
                throw new CustomReplyException("需要提供邮箱地址");
            }

            if (string.IsNullOrEmpty(userInfoVm.Password)) {
                throw new CustomReplyException("需要提供用户密码");
            }

            var result = userService.ResetPassword(userInfoVm.Email, userInfoVm.Password!).Result;
            return Ok(result);
        }

        /// <summary>
        /// 邮箱重置
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        /// <exception cref="CustomReplyException"></exception>
        [HttpPost("resetEmail")]
        [UnityOfWork(typeof(SqlServerDbContext))]
        public ActionResult ResetEmail(UserInfo userInfo) {
            if (userInfo.UserId == Guid.Empty) {
                throw new CustomReplyException("需要提供用户Id");
            }

            if (string.IsNullOrEmpty(userInfo.Email)) {
                throw new CustomReplyException("邮箱必须提供");
            }

            var result = userService.ResetEmail(userInfo).Result;
            return result ? Ok(userInfo) : BadRequest("邮箱重置失败，请重试");
        }

        /// <summary>
        /// 对密码进行确认
        /// </summary>
        /// <param name="userInfoVo"></param>
        /// <returns></returns>
        [HttpPost("checkPass")]
        public ActionResult CheckPassword(UserInfoVm userInfoVo) {
            if (string.IsNullOrEmpty(userInfoVo.UserId)) {
                throw new ArgumentException("用户Id不可以为空");
            }

            var result = userService.CheckPassword(userInfoVo).Result;
            return Ok(result);
        }


        /// <summary>
        /// 重置用户头像
        /// </summary>
        /// <param name="avatarVm"></param>
        /// <returns></returns>
        [HttpPost("resetAvatar")]
        [UnityOfWork(typeof(SqlServerDbContext))]
        public async Task<ActionResult> ResetAvatar(UserAvatarVm avatarVm) {
            if (avatarVm.AvatarFile.Length == 0) {
                throw new ArgumentException("需要提供头像图片");
            }

            if (string.IsNullOrEmpty(avatarVm.UserId)) {
                throw new ArgumentException("需要提供用户Id");
            }

            var user = await userService.ResetAvatar(avatarVm.AvatarFile, Guid.Parse(avatarVm.UserId));
            return Ok(user);
        }

        /// <summary>
        /// 查询个人磁盘情况
        /// </summary>
        /// <returns></returns>
        [HttpPost("diskInfo")]
        public ActionResult PersonalDiskInfo() {
            if (!Directory.Exists(_basicFilePath)) {
                return BadRequest("未申明磁盘空间");
            }

            var driveInfo = new DriveInfo(Path.GetPathRoot(_basicFilePath)!);
            var totalSize = driveInfo.TotalSize;
            var usedSize = driveInfo.TotalSize - driveInfo.AvailableFreeSpace;

            var totalSpace = new MyFile(totalSize);
            var usedSpace = new MyFile(usedSize);
            var ratio = totalSize == 0 ? 0 : (usedSize / totalSize) * 100;

            return Ok(new { totalSpace, usedSpace, ratio });
        }
    }
}

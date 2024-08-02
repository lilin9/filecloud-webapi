using Domain;
using Domain.Entities;
using Domain.Vo;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Attributes;
using WebAPI.Services;

namespace WebAPI.Controllers {
    [ApiController]
    [Route("user")]
    public class UserController(UserService userService): ControllerBase {

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="userVo"></param>
        /// <returns></returns>
        [HttpPost("register")]
        [UnityOfWork(typeof(SqlServerDbContext))]
        public ActionResult Register(UserInfoVo userVo) {
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
        public ActionResult Login(UserInfoVo userVo) {
            if (string.IsNullOrEmpty(userVo.Password)) {
                throw new CustomReplyException("密码必须提供");
            }

            if (string.IsNullOrEmpty(userVo.Email)) {
                throw new CustomReplyException("邮箱必须提供");
            }

            var result = userService.Login(userVo).Result;
            return result == null ? BadRequest("登录失败，请重试") : Ok(result);
        }

        /// <summary>
        /// 密码重置
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        [HttpPost("resetPass")]
        [UnityOfWork(typeof(SqlServerDbContext))]
        public ActionResult ResetPassword(UserInfo userInfo) {
            if (userInfo.UserId == Guid.Empty) {
                throw new CustomReplyException("需要提供用户Id");
            }

            var result = userService.ResetPassword(userInfo).Result;
            return result ? Ok(userInfo) : BadRequest("密码重置失败，请重试");
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
    }
}

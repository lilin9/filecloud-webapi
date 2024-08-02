using Domain;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using WebAPI.Services;

namespace WebAPI.Controllers {
    [ApiController]
    [Route("verifyCode")]
    public class VerifyCodeController(VerifyCodeService verifyCodeService): ControllerBase {
        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost("senderCode/{email}")]
        public ActionResult SenderVerifyCode(string email) {
            const string emailPattern = @"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$";
            if (string.IsNullOrEmpty(email) || !Regex.IsMatch(email, emailPattern)) {
                throw new CustomReplyException("需提供合法邮箱地址");
            }

            var result = verifyCodeService.SenderVerifyCode(email).Result;
            return result ? Ok() : BadRequest("验证码发送失败，请重试");
        }

        /// <summary>
        /// 验证码校验
        /// </summary>
        /// <param name="email"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        [HttpPost("checkCode/{email}/{code:int}")]
        public ActionResult CheckVerifyCode(string email, int code) {
            if (code == 0) {
                throw new CustomReplyException("需要提供验证码");
            }

            return verifyCodeService.CheckVerifyCode(email, code) ? Ok() : BadRequest();
        }
    }
}

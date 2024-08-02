using System.Text;
using Domain.Repository;
using Microsoft.Extensions.Caching.Memory;

namespace WebAPI.Services {
    public class VerifyCodeService(IEmailRepository emailRepository, IMemoryCache memoryCache) {
        /// <summary>
        /// 验证码发送
        /// </summary>
        /// <param name="email"></param>
        public async Task<bool> SenderVerifyCode(string email) {
            var verifyCode = GenerateCode(6);
            //将验证码保存到缓存中，设置过期时间，2分钟后过期
            CacheVerifyCode($"VerifyCode_{email}", verifyCode, DateTimeOffset.Now.AddMinutes(2));
            //发送电子邮件
            return await emailRepository.SenderEmail(email, GenerateSubject(), GenerateBody(verifyCode));
        }

        /// <summary>
        /// 校验验证码
        /// </summary>
        /// <param name="email"></param>
        /// <param name="verifyCode"></param>
        /// <returns></returns>
        public bool CheckVerifyCode(string email, int verifyCode) {
            var key = $"VerifyCode_{email}";
            var cacheVerifyCode = GetVerifyCodeByCache(key);
            //拿到验证码后将其从缓存移除
            RemoveVerifyCodeByCache(key);
            return cacheVerifyCode == verifyCode;
        }

        /// <summary>
        /// 从缓存中移除验证码
        /// </summary>
        /// <param name="key"></param>
        private void RemoveVerifyCodeByCache(string key) {
            memoryCache.Remove(key);
        }

        /// <summary>
        /// 从缓存中获取验证码
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private int GetVerifyCodeByCache(string key) {
            var verifyCode = memoryCache.Get(key)?.ToString();
            return string.IsNullOrEmpty(verifyCode) ? 0 : int.Parse(verifyCode);
        }

        /// <summary>
        /// 将验证码缓存到内存中
        /// </summary>
        /// <param name="key"></param>
        /// <param name="verifyCode"></param>
        /// <param name="expiration"></param>
        private void CacheVerifyCode(string key, string verifyCode, DateTimeOffset expiration) {
            memoryCache.Set(key, verifyCode, expiration);
        }

        /// <summary>
        /// 生成数字验证码
        /// </summary>
        /// <param name="len">验证码长度</param>
        /// <returns></returns>
        private static string GenerateCode(int len) {
            const string baseStr = "0123456789";
            var random = new Random();
            var stringBuilder = new StringBuilder(len);

            for (var i = 0; i < len; i++) {
                stringBuilder.Append(baseStr[random.Next(baseStr.Length)]);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// 获得邮件主题
        /// </summary>
        /// <returns></returns>
        private static string GenerateSubject() {
            return "ebulent.com: Here's your Verification Code";
        }

        /// <summary>
        /// 获取邮件信息主体
        /// </summary>
        /// <param name="verifyCode"></param>
        /// <returns></returns>
        private static string GenerateBody(string verifyCode) {
            var bodyStr = "<p>If this was you, your verification code is:</p>" +
                              $"<p style=\"background-color: rgb(211, 211, 211); text-align: left; font-size: 20px; font-weight: bold; font-family: &quot;Amazon Ember&quot;, Arial, sans-serif; padding: 15px 1px 10px 10px; border-radius: 10px; --darkreader-inline-bgcolor: #313537;\">{verifyCode}</p> + " +
                              "<p>Don’t share it with others.</p>";

            return bodyStr;
        }
    }
}

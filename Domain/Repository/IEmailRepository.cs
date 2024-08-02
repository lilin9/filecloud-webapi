namespace Domain.Repository {
    public interface IEmailRepository {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="toEmail">要发往的邮箱</param>
        /// <param name="subject">邮箱主题</param>
        /// <param name="msgBody">邮件内容</param>
        /// <returns></returns>
        Task<bool> SenderEmail(string toEmail, string subject, string msgBody);
    }
}

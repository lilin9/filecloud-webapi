using Domain.Repository;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;

namespace Infrastructure.RepositoryImpl {
    public class EmailRepository(IConfiguration configuration): IEmailRepository {
        private readonly string _smtpServer = configuration["CustomStrings:SmtpServer"]!;    //SMTP服务器
        private readonly string _fromEmail = configuration["CustomStrings:FromEmail"]!;      //发信邮箱
        private readonly string _authPwd = configuration["CustomStrings:AuthPassword"]!;     //密码或授权码

        public async Task<bool> SenderEmail(string toEmail, string subject, string msgBody) {
            if (_smtpServer == "" || _fromEmail == "" || _authPwd == "") {
                return false;
            }

            using var msg = new MailMessage();
            /*
             * msg.To.Add("b@b.com");可以发送给多人
             */
            msg.To.Add(toEmail); //设置收件人

            /*
             * msg.CC.Add("c@c.com");
             * msg.CC.Add("c@c.com");可以抄送给多人
             */

            /* 3个参数分别是 发件人地址（可以随便写），发件人姓名，编码 */
            msg.From = new MailAddress(_fromEmail, _fromEmail, System.Text.Encoding.UTF8);


            msg.Subject = subject; //邮件标题   
            msg.SubjectEncoding = System.Text.Encoding.UTF8; //邮件标题编码   
            msg.Body = msgBody; //邮件内容   
            msg.BodyEncoding = System.Text.Encoding.UTF8; //邮件内容编码   
            msg.IsBodyHtml = true; //是否是HTML邮件   
            msg.Priority = MailPriority.Normal; //邮件优先级

            var client = new SmtpClient(_smtpServer, 587); //邮件服务器地址及端口号
            client.EnableSsl = true; //ssl加密发送
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new System.Net.NetworkCredential(_fromEmail, _authPwd); //邮箱账号  密码
            client.Timeout = 6000; //6秒超时

            try {
                await client.SendMailAsync(msg); //发送邮件

                client.Dispose(); //释放资源
                return true;
            } catch (SmtpException) {
                return false;
            }
        }
    }
}

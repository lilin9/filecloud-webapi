using Domain.Entities.Enum;
using Domain.Entities.SalveModel;
using Microsoft.EntityFrameworkCore;
using NETCore.Encrypt;

namespace Domain.Entities {
    public class UserInfo {
        private UserInfo() { }

        public UserInfo(string username, string email, string password) {
            UserName = username;
            Email = email;
            Password = password;

            UserId = Guid.NewGuid();
            CreateTime = DateTime.Now;
            UpdateTime = DateTime.Now;
        }

        /// <summary>
        /// 数据库自增Id，必须
        /// </summary>
        [Comment("数据库自增Id，必须")]
        public int Id { get; }

        /// <summary>
        /// 用户唯一标识，必须
        /// </summary>
        [Comment("用户唯一标识，必须")]
        public Guid UserId { get; init; }

        /// <summary>
        /// 当前账户是否可用，必须
        /// </summary>
        [Comment("当前账户是否可用，必须")]
        public bool Available { get; private set; } = true;

        /// <summary>
        /// 创建时间，必须
        /// </summary>
        [Comment("创建时间，必须")]
        private DateTime createTime;
        public DateTime CreateTime { get { return createTime; } private set { createTime = value; } }

        /// <summary>
        /// 更新时间，必须
        /// </summary>
        [Comment("更新时间，必须")]
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 用户头像，必须
        /// </summary>
        [Comment("用户头像，必须")]
        public string? Avatar { get; private set; }

        /// <summary>
        /// 账户被封禁时间，-1：永久；0：正常，必须
        /// </summary>
        [Comment("账户被封禁时间，-1：永久；0：正常，必须")]
        public MyTime BanTime { get; set; } = new MyTime {Value = 0,Unit = TimeUnit.Hour};

        /// <summary>
        /// 用户邮箱，必须
        /// </summary>
        [Comment("用户邮箱，必须")]
        public string Email { get; init; }

        /// <summary>
        /// 用户名，必须
        /// </summary>
        [Comment("用户名，必须")]
        public string UserName { get; init; }

        /// <summary>
        /// 用户密码，必须
        /// </summary>
        [Comment("用户密码，必须")]
        private string password;
        public string Password {
            get => password;
            set => password = LockPass(value);
        }

        /// <summary>
        /// 账户禁用原因，可选
        /// </summary>
        [Comment("账户禁用原因，可选")]
        public string? DisableReason { get; set; }

        /// <summary>
        /// 账户解封时间，可选
        /// </summary>
        [Comment("账户解封时间，可选")]
        public DateTime? UnLockTime { get; set; }


        /// <summary>
        /// 验证密码是否相同
        /// </summary>
        /// <param name="unLockPass">未加密密码</param>
        /// <returns></returns>
        public bool CheckPassword(string unLockPass) {
            return Password.Equals(LockPass(unLockPass));
        }

        /// <summary>
        /// 对用户密码进行md5加密
        /// </summary>
        /// <param name="pass"></param>
        /// <returns></returns>
        public string LockPass(string pass) {
            return EncryptProvider.Md5(pass);
        }
    }
}

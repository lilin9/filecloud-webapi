using Domain.Entities.SalveModel;
using Microsoft.EntityFrameworkCore;
using NETCore.Encrypt;

namespace Domain.Entities {
    public class UserInfo {
        public UserInfo() { }

        public UserInfo(string username, string email) {
            UserName = username;
            Email = email;

            UserId = Guid.NewGuid();
            CreateTime = DateTime.Now;
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
        public string Avatar { get; set; }

        /// <summary>
        /// 账户被封禁时间，-1：永久；0：正常，必须
        /// </summary>
        [Comment("账户被封禁时间，-1：永久；0：正常，必须")]
        public MyTime BanTime { get; set; }

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
        public string Password { get => password;
            set => password = EncryptProvider.Md5(password);
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
        /// 用户磁盘使用情况，必须
        /// </summary>
        [Comment("用户磁盘使用情况，必须")]
        public Disk UserDisk { get; set; }
    }
}

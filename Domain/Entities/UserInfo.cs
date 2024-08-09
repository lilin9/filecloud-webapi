using Domain.Entities.Enum;
using Domain.Entities.SalveModel;
using Microsoft.AspNetCore.Http;
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
        public DateTime UpdateTime { get; private set; }

        /// <summary>
        /// 用户头像，必须
        /// </summary>
        [Comment("用户头像，必须")]
        public string? Avatar { get; private set; }

        /// <summary>
        /// 头像静态访问地址
        /// </summary>
        public string? StaticAvatar { get; private set; }

        /// <summary>
        /// 头像在线访问地址
        /// </summary>
        public string? OnlineAvatar { get; private set; }

        /// <summary>
        /// 账户被封禁时间，-1：永久；0：正常，必须
        /// </summary>
        [Comment("账户被封禁时间，-1：永久；0：正常，必须")]
        public MyTime BanTime { get; private set; } = new MyTime { Value = 0, Unit = TimeUnit.Hour };

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
            private set => password = value;
        }

        /// <summary>
        /// 账户禁用原因，可选
        /// </summary>
        [Comment("账户禁用原因，可选")]
        public string? DisableReason { get; private set; }

        /// <summary>
        /// 账户解封时间，可选
        /// </summary>
        [Comment("账户解封时间，可选")]
        public DateTime? UnLockTime { get; private set; }


        /// <summary>
        /// 验证密码是否相同
        /// </summary>
        /// <param name="unLockPass">未加密密码</param>
        /// <returns></returns>
        public bool CheckPassword(string? unLockPass) {
            if (string.IsNullOrEmpty(unLockPass)) {
                return false;
            }
            return Password.Equals(LockPassword(unLockPass));
        }

        /// <summary>
        /// 对用户密码进行md5加密
        /// </summary>
        /// <param name="pass"></param>
        /// <returns></returns>
        public void LockPass(string pass) {
            Password = EncryptProvider.Md5(pass);
        }

        /// <summary>
        /// 对用户密码进行md5加密
        /// </summary>
        /// <param name="pass"></param>
        /// <returns></returns>
        public string LockPassword(string pass) {
            return EncryptProvider.Md5(pass);
        }


        /// <summary>
        /// 修改用户头像
        /// </summary>
        /// <param name="avatarFile">头像文件</param>
        public async void UpdateAvatar(IFormFile avatarFile) {
            if (string.IsNullOrEmpty(Avatar)) {
                CompletePath(avatarFile.FileName);
            }
            //原来的用户头像存在，就删除再替换
            if (File.Exists(StaticAvatar)) {
                File.Delete(StaticAvatar);
            }

            await using var fs = new FileStream(StaticAvatar!, FileMode.Create, FileAccess.Write);
            await avatarFile.CopyToAsync(fs);
        }


        /// <summary>
        /// 补全文件路径
        /// </summary>
        /// <param name="basicAvatarPath">物理文件路径的基础路径</param>
        /// <param name="onlineFilePath">在线访问文件基础路径</param>
        public void CompletePath(string basicAvatarPath, string onlineFilePath) {
            //物理目录路径不存在，创建一下下
            if (!Directory.Exists(basicAvatarPath)) {
                Directory.CreateDirectory(basicAvatarPath);
            }

            StaticAvatar = basicAvatarPath + Avatar;
            OnlineAvatar = onlineFilePath + Avatar;
        }

        /// <summary>
        /// 补全文件路径
        /// </summary>
        /// <param name="avatarName">头像文件名</param>
        public void CompletePath(string avatarName) {
            Avatar = UserId + Path.GetExtension(avatarName);
            StaticAvatar += Avatar;
            OnlineAvatar += Avatar;
        }

        /// <summary>
        /// 设置修改时间
        /// </summary>
        public void SetUpdateTimeNow() {
            UpdateTime = DateTime.Now;
        }
    }
}

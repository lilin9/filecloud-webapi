using Domain.Entities.Enum;
using Domain.Entities.SalveModel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities {
    public class Files {
        private Files() { }

        public Files(Guid? parentId, string fileName, Guid? userInfoId, 
            MyFile? fileSize, string? fileMimeType, bool isFolder) {
            FileId = Guid.NewGuid();
            UserInfoId = userInfoId;
            ParentId = parentId;
            FileName = fileName;
            CreateTime = DateTime.Now;
            UpdateTime = DateTime.Now;
            FileSize = fileSize ?? new MyFile(0, FileUnit.Bytes);
            FileMimeType = fileMimeType;
            FileOnlyTag = Path.GetFileNameWithoutExtension(fileName) + fileSize + DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            IsFolder = isFolder;
        }

        /// <summary>
        /// 数据库自增Id
        /// </summary>

        [Comment("数据库自增Id")]
        public int Id { get; }

        /// <summary>
        /// 唯一标识
        /// </summary>
        [Comment("唯一标识")]
        public Guid FileId { get; init; }

        /// <summary>
        /// 用户信息Id
        /// </summary>
        [Comment("用户信息Id")]
        public Guid? UserInfoId { get; private set; }

        /// <summary>
        /// 父级Id
        /// </summary>
        [Comment("父级Id")]
        public Guid? ParentId { get; private set; }

        /// <summary>
        /// 文件名
        /// </summary>
        [Comment("文件名")]
        public string FileName { get; private set; }

        /// <summary>
        /// 文件的静态下载路径
        /// </summary>
        public string StaticDownloadUrl { get; set; } = "/";

        /// <summary>
        /// 文件的动态访问链接
        /// </summary>
        public string? DynamicDownloadUrl { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [field: Comment("创建时间")]
        public DateTime CreateTime { get; init; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [Comment("修改时间")]
        public DateTime UpdateTime { get; private set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        [Comment("文件大小")]
        public MyFile FileSize { get; private set; } = new(0, FileUnit.Bytes);

        /// <summary>
        /// 文件的mime类型
        /// </summary>
        [Comment("文件的mime类型")]
        public string? FileMimeType { get; private set; }

        /// <summary>
        /// 文件的唯一标识，表示给用户看的
        /// </summary>
        [Comment("文件的唯一标识，表示给用户看的")]
        public string? FileOnlyTag { get; private set; }

        /// <summary>
        /// 当前文件是否被禁止被访问
        /// </summary>
        [Comment("当前文件是否被禁止被访问")]
        public bool IsDisable { get; private set; } = false;

        /// <summary>
        /// 是否为文件夹
        /// </summary>
        [Comment("是否为文件夹")]
        public bool IsFolder { get; private set; }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="savePath">文件夹路径</param>
        public void CreateFolder(string savePath) {
            if (Directory.Exists(savePath)) {return;}
            Directory.CreateDirectory(savePath);
        }

        /// <summary>
        /// 删除指定的目录
        /// </summary>
        /// <param name="folderPath">目录路径</param>
        public void DeleteFolder(string folderPath) {
            Directory.Delete(folderPath, true);
        }

        /// <summary>
        /// 重命名文件名称
        /// </summary>
        /// <param name="newFilePath">新文件路径</param>
        /// <param name="oldFilePath">旧文件路径</param>
        public void RenameFile(string newFilePath, string oldFilePath) {
            if (IsFolder) {
                Directory.Move(oldFilePath, newFilePath);
            } else {
                File.Move(oldFilePath, newFilePath);
            }
            FileName = newFilePath[(newFilePath.LastIndexOf('/') + 1)..];
            UpdateTime = DateTime.Now;
        }

        /// <summary>
        /// 判断文件或者文件夹是否存在
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public bool ExistsFile(string filePath) {
            return File.Exists(filePath) || Directory.Exists(filePath);
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="file">文件数据</param>
        /// <param name="folderPath">文件的保存路径</param>
        public async void UploadFile(IFormFile file, string folderPath) {
            //保存文件
            await using var fileStream = new FileStream(folderPath, FileMode.Create);
            await file.CopyToAsync(fileStream);
        }

        /// <summary>
        /// 删除指定的文件
        /// </summary>
        /// <param name="filePath">文件的路径</param>
        public void DeleteFile(string filePath) {
            File.Delete(filePath);
        }
    }
}

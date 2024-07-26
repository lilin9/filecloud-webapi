using Domain.Entities;
using Domain.Entities.Enum;
using Domain.Entities.SalveModel;
using Domain.Repository;

namespace WebAPI.Services {
    public class FileService(IFilesRepository filesRepository, IConfiguration configuration) {
        //读取文件的物理保存路径
        private readonly string _basicFilePath = configuration["CustomStrings:BasicFilePath"]!;

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="file">上传的文件流</param>
        /// <param name="parentId">父文件Id</param>
        /// <returns></returns>
        public async Task<Files> UploadFile(IFormFile file, Guid? parentId) {
            var fileModel = new Files(
                parentId,
                file.FileName,
                null,
                new MyFile(file.Length, FileUnit.Bytes),
                file.ContentType,
                false
            );
            fileModel.UploadFile(file, _basicFilePath);
            await filesRepository.SaveOneAsync(fileModel);

            return fileModel;
        }

        /// <summary>
        /// 创建新文件夹
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Files> CreateFolder(Guid? parentId, string folderName) {
            var folderModel = new Files(
                parentId,
                folderName,
                null,
                new MyFile(0, FileUnit.Bytes),
                null,
                true
            );
            //写入数据库和创建本地文件夹
            folderModel.CreateFolder(_basicFilePath);
            await filesRepository.SaveOneAsync(folderModel);

            return folderModel;
        }

        /// <summary>
        /// 修改文件名
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="newFileName"></param>
        /// <returns></returns>
        public async Task<Files> RenameFile(Guid fileId, string newFileName) {
            var fileModel =  await filesRepository.GetOne(fileId);
            var oldFilePath = _basicFilePath + fileModel.StaticDownloadUrl;
            var newFilePath = _basicFilePath + fileModel.StaticDownloadUrl.Replace(fileModel.FileName, newFileName);

            //判断一下原文件是否是存在的
            if (!fileModel.ExistsFile(oldFilePath)) {
                throw new FileNotFoundException("文件不存在");
            }

            //判断新文件名是否已经存在
            if (fileModel.ExistsFile(newFilePath)) {
                throw new Exception("文件名已存在");
            }

            //修改文件名
            fileModel.RenameFile(newFileName, _basicFilePath);
            return fileModel;
        }

        /// <summary>
        /// 获取文件访问的静态链接
        /// </summary>
        /// <param name="parentFileModel">父文件</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        private string GetStaticDownloadFilePath(Files? parentFileModel, string fileName) {
            return parentFileModel == null ? "/" + fileName : parentFileModel.StaticDownloadUrl + "/" + fileName;
        }
    }
}

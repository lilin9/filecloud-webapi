using Domain;
using Domain.Entities;
using Domain.Entities.Enum;
using Domain.Entities.SalveModel;
using Domain.Repository;
using Domain.Vo;

namespace WebAPI.Services {
    public class FileService(IFilesRepository filesRepository) {

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
                new MyFile(file.Length),
                file.ContentType,
                false
            );
            //补全文件静态路径
            filesRepository.CompleteFileUrl(fileModel);
            fileModel.UploadFile(file, fileModel.StaticDownloadUrl);
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

            //补全文件的静态路径
            filesRepository.CompleteFileUrl(folderModel);

            //写入数据库和创建本地文件夹
            folderModel!.CreateFolder(folderModel.StaticDownloadUrl);
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
            var fileModel = await filesRepository.GetOne(fileId);
            if (fileModel == null) {
                throw new CustomReplyException("文件在数据库无记录");
            }
            //查找文件相对路径
            filesRepository.CompleteFileUrl(fileModel);

            //拼出文件路径
            var oldFilePath = fileModel!.StaticDownloadUrl;
            var newFilePath = oldFilePath[..(oldFilePath.LastIndexOf('/') + 1)] + newFileName;


            //判断一下原文件是否是存在的
            if (!fileModel.ExistsFile(oldFilePath)) {
                throw new CustomReplyException("文件不存在");
            }

            //判断新文件名是否已经存在
            if (fileModel.ExistsFile(newFilePath)) {
                throw new CustomReplyException("文件名已存在");
            }

            //修改文件名
            fileModel.RenameFile(newFilePath, oldFilePath);
            return fileModel;
        }

        /// <summary>
        /// 获取文件列表
        /// </summary>
        /// <param name="listVo"></param>
        /// <returns></returns>
        public Task<List<Files>> GetList(FileListVo listVo) {
               return filesRepository.FindList(listVo.PageIndex, listVo.PageSize,listVo.GuidParentId);
        }
    }
}

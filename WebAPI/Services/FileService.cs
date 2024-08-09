using Domain;
using Domain.Entities;
using Domain.Entities.Enum;
using Domain.Entities.SalveModel;
using Domain.Repository;
using Domain.Vo;
using Infrastructure;

namespace WebAPI.Services {
    public class FileService(IFilesRepository filesRepository, SqlServerDbContext dbContext) {
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="file">上传的文件流</param>
        /// <param name="parentId">父文件Id</param>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public async Task<Files> UploadFile(IFormFile file, Guid? parentId) {
            await using var transaction = await dbContext.Database.BeginTransactionAsync();
            try {
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
                //检查文件名是否重复
                if (fileModel.CheckRepeatedName()) {
                    throw new CustomReplyException("文件名重复");
                }

                //分别将文件传到本地目录和数据库
                await fileModel.UploadFileAsync(file, fileModel.StaticDownloadUrl);
                await filesRepository.SaveOneAsync(fileModel);

                //提交事务
                await transaction.CommitAsync();
                return fileModel;
            } catch (Exception e) {
                //出现异常就回滚事务
                await transaction.RollbackAsync();
                throw e;
            }
        }

        /// <summary>
        /// 创建新文件夹
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Files> CreateFolder(Guid? parentId, string folderName) {
            await using var transaction = await dbContext.Database.BeginTransactionAsync();
            try {
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
                //判断文件夹名是否重复
                if (folderModel.CheckRepeatedName()) {
                    throw new CustomReplyException("文件夹名字重复");
                }

                //写入数据库和创建本地文件夹
                folderModel!.CreateFolder(folderModel.StaticDownloadUrl);
                await filesRepository.SaveOneAsync(folderModel);

                //提交事务
                await transaction.CommitAsync();
                return folderModel;
            } catch (Exception e) {
                //出现异常就回滚事务
                await transaction.RollbackAsync();
                throw new IOException("创建文件夹失败，请重试！");
            }
        }

        /// <summary>
        /// 修改文件名
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="newFileName"></param>
        /// <returns></returns>
        public async Task<Files> RenameFile(Guid fileId, string newFileName) {
            try {
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
                if (!fileModel.CheckRepeatedName(oldFilePath)) {
                    throw new CustomReplyException("文件不存在");
                }

                //判断新文件名是否已经存在
                if (fileModel.CheckRepeatedName(newFilePath)) {
                    throw new CustomReplyException("文件名已存在");
                }

                //修改文件名
                fileModel.RenameFile(newFilePath, oldFilePath);
                return fileModel;
            } catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// 获取文件列表
        /// </summary>
        /// <param name="listVo"></param>
        /// <returns></returns>
        public Task<List<Files>> GetList(FileListVm listVo) {
            return filesRepository.FindList(listVo.PageIndex, listVo.PageSize, listVo.GuidParentId);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public async Task DeleteFile(Guid fileId) {
            //确认文件在数据存在
            var file = await filesRepository.FindOne(fileId);
            if (file == null) {
                throw new CustomReplyException("文件不存在，请刷新后重试");
            }

            await filesRepository.DeleteOne(file);
            //删除磁盘文件
            file.DeleteFile();
        }
    }
}

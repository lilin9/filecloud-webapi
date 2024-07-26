using Domain.Entities;
using Domain.Repository;

namespace Infrastructure.RepositoryImpl {
    public class FilesRepository(SqlServerDbContext dbContext): IFilesRepository {

        public async Task<Files?> FindOne(Guid fileId) {
            var file = await Task.FromResult(dbContext.Files.SingleOrDefault(f => f.FileId == fileId));
            return await FindStaticDownloadUrl(file);
        }

        public async Task SaveOneAsync(Files file) {
            await dbContext.Files.AddAsync(file);
        }

        public Task<Files> GetOne(Guid fileId) {
            var result = FindOne(fileId).Result;
            return result == null ? throw new FileNotFoundException($"FileId={fileId}的文件不存在") : Task.FromResult(result);
        }

        public Task<Files?> FindStaticDownloadUrl(Files? file) {
            if (file == null) {
                return Task.FromResult(file);
            }

            var allFileList = dbContext.Files.ToList();
            file.StaticDownloadUrl = RecursionJoint(file, "", allFileList);
            return Task.FromResult(file)!;
        }

        /// <summary>
        /// 通过递归拼接出路径地址
        /// </summary>
        /// <param name="file">当前文件</param>
        /// <param name="filePath">当前文件拼接好的路径</param>
        /// <param name="allFilesList">所有文件实体列表</param>
        /// <returns></returns>
        private string RecursionJoint(Files? file, string filePath, List<Files> allFilesList) {
            if (file == null || file.ParentId == Guid.Empty) {
                return file == null ? filePath : "/" + file.FileName + filePath;
            }

            var parentFile = allFilesList.SingleOrDefault(f => f.FileId == file.ParentId);
            filePath = parentFile == null ? filePath : "/" + file.FileName + filePath;
            return RecursionJoint(parentFile, filePath, allFilesList);
        }
    }
}

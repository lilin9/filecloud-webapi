using Domain.Entities;
using Domain.Repository;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.RepositoryImpl {
    public class FilesRepository(SqlServerDbContext dbContext, IConfiguration configuration): IFilesRepository {
        private readonly string _basicFilePath = configuration["CustomStrings:BasicFilePath"]!;
        private readonly string _onlineFilePath = configuration["CustomStrings:OnlineFilePath"]!;
        private readonly List<Files> _allFilesList = dbContext.Files.ToList();

        public async Task<Files?> FindOne(Guid fileId) {
            var file = await Task.FromResult(dbContext.Files.SingleOrDefault(f => f.FileId == fileId));

            //补全 StaticDownloadUrl 和 DynamicDownloadUrl
            if (file != null) {
                CompleteFileUrl(file);
            }
            return file;
        }

        public async Task SaveOneAsync(Files file) {
            await dbContext.Files.AddAsync(file);
        }

        public Task<Files> GetOne(Guid fileId) {
            var result = FindOne(fileId).Result;

            //补全 StaticDownloadUrl 和 DynamicDownloadUrl
            if (result != null) {
                CompleteFileUrl(result);
            }
            return result == null ? throw new FileNotFoundException($"FileId={fileId}的文件不存在") : Task.FromResult(result);
        }

        public (Task<string>, Task<string>) FindFileUrl(Guid? parentFileId, string currentFileName) {
            var filePath = "/" + currentFileName;
            if (parentFileId == Guid.Empty) {
                return (Task.FromResult(_basicFilePath + filePath), Task.FromResult(_onlineFilePath + filePath));
            }

            var relativeUri = RecursionJoint(parentFileId, filePath);
            return (Task.FromResult(_basicFilePath + relativeUri), Task.FromResult(_onlineFilePath + relativeUri));
        }

        public Task<List<Files>> FindList(int pageIndex, int pageSize) {
            var list = dbContext.Files.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            //补全 StaticDownloadUrl 和 DynamicDownloadUrl
            CompleteFileUrlByList(list);
            return Task.FromResult(list);
        }

        public Task<List<Files>> FindList(int pageIndex, int pageSize, Guid? parentId) {
            var list = dbContext.Files.Where(f => f.ParentId == parentId)
                .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            //补全 StaticDownloadUrl 和 DynamicDownloadUrl
            CompleteFileUrlByList(list);
            return Task.FromResult(list);
        }

        public async Task DeleteOne(Files file) {
            if (file.IsFolder) {
                //查询出所有子文件
                var allChildren = await FindChildren(file.FileId);
                //删除所有子文件
                dbContext.Files.RemoveRange(allChildren);
            }
            //删除父文件
            dbContext.Files.Remove(file);
        }

        public Task<List<Files>> FindChildren(Guid fileId) {
            var fileChildren = FindAllChildren(fileId, _allFilesList).ToList();
            return Task.FromResult(fileChildren);
        }

        public void CompleteFileUrl(Files? file) {
            if (file == null) { return; }

            var relativeUri = RecursionJoint(file, "");

            file.StaticDownloadUrl = _basicFilePath + relativeUri;
            file.DynamicDownloadUrl = _onlineFilePath + relativeUri;
        }

        /// <summary>
        /// 传入文件Id，查询出所有的子文件
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private IEnumerable<Files> FindAllChildren(Guid parentId, List<Files> list) {
            var children = list.Where(f => f.ParentId == parentId);
            foreach (var child in children) {
                yield return child;
                foreach (var grandChild in FindAllChildren(child.FileId, list)) {
                    yield return grandChild;
                }
            }
        } 

        /// <summary>
        /// 补全文件列表的静态保存路径
        /// </summary>
        /// <param name="list"></param>
        private void CompleteFileUrlByList(List<Files> list) {
            list.ForEach(CompleteFileUrl);
        }

        /// <summary>
        /// 通过递归拼接出文件相对路径地址
        /// </summary>
        /// <param name="parentId">父ID</param>
        /// <param name="filePath">拼接好的文件路径</param>
        /// <returns></returns>
        private string RecursionJoint(Guid? parentId, string filePath) {
            if (parentId == null || parentId == Guid.Empty) {
                return filePath;
            }

            var filesModel = _allFilesList.SingleOrDefault(f => f.FileId == parentId);
            filePath = filesModel == null ? filePath : "/" + filesModel.FileName + filePath;
            return RecursionJoint(filesModel?.ParentId, filePath);
        }

        /// <summary>
        /// 通过递归拼接出文件相对路径地址
        /// </summary>
        /// <param name="file">当前文件</param>
        /// <param name="filePath">当前文件拼接好的路径</param>
        /// <returns></returns>
        private string RecursionJoint(Files? file, string filePath) {
            if (file == null || file.ParentId == Guid.Empty) {
                return file == null ? filePath : "/" + file.FileName + filePath;
            }

            var parentFile = _allFilesList.SingleOrDefault(f => f.FileId == file.ParentId);
            filePath = parentFile == null ? filePath : "/" + file.FileName + filePath;
            return RecursionJoint(parentFile, filePath);
        }
    }
}

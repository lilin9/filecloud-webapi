using Domain.Entities;

namespace Domain.Repository
{
    public interface IFilesRepository
    {
        /// <summary>
        /// 查找单个文件
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public Task<Files?> FindOne(Guid fileId);

        /// <summary>
        /// 保存单个文件
        /// </summary>
        /// <param name="file"></param>
        public Task SaveOneAsync(Files file);

        /// <summary>
        /// 获取单个文件
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public Task<Files> GetOne(Guid fileId);

        /// <summary>
        /// 获取文件的相对静态下载路径
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public Task<Files?> FindStaticDownloadUrl(Files? file);
    }
}

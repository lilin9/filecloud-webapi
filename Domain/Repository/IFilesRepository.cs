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
        /// 补全文件的静态下载路径
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public void CompleteFileUrl(Files? file);


        /// <summary>
        /// 获取文件的静态以及动态下载路径，路径是完整的
        /// </summary>
        /// <param name="parentFileId">传入父id</param>
        /// <param name="currentFileName">当前文件名</param>
        /// <returns>返回静态下载路径，返回动态访问路径</returns>
        public (Task<string>, Task<string>) FindFileUrl(Guid? parentFileId, string currentFileName);

        /// <summary>
        /// 查询文件列表
        /// </summary>
        /// <param name="pageIndex">页索引，从1开始</param>
        /// <param name="pageSize">当页大小</param>
        /// <returns></returns>
        public Task<List<Files>> FindList(int pageIndex, int pageSize);

        /// <summary>
        /// 查询文件列表
        /// </summary>
        /// <param name="pageIndex">页索引，从1开始</param>
        /// <param name="pageSize">当页大小</param>
        /// <param name="parentId">父文件id</param>
        /// <returns></returns>
        public Task<List<Files>> FindList(int pageIndex, int pageSize, Guid? parentId);

        /// <summary>
        /// 删除单个文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public Task DeleteOne(Files file);

        /// <summary>
        /// 根据传入的文件Id，查询子文件
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public Task<List<Files>> FindChildren(Guid fileId);
    }
}

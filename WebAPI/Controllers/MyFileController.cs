using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Middleware;
using WebAPI.Services;

namespace WebAPI.Controllers {
    [ApiController]
    [Route("/myFile")]
    public class MyFileController(FileService fileService): ControllerBase {
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [HttpPost("/upload/{parentId}")]
        [UnityOfWork(typeof(SqlServerDbContext))]
        public ActionResult UploadFile(IFormFile file, string? parentId) {
            if (file.Length == 0) {
                return BadRequest("需要提供上传文件数据");
            }

            var resultFile = fileService.UploadFile(file, ConvertToGuid(parentId));
            return Ok(resultFile);
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        [HttpPost("/createFolder/{parentId}/{folderName}")]
        [UnityOfWork(typeof(SqlServerDbContext))]
        public ActionResult CreateFolder(string? parentId, string folderName) {
            if (string.IsNullOrEmpty(folderName)) {
                return BadRequest("需要提供文件名字");
            }

            var resultFile = fileService.CreateFolder(ConvertToGuid(parentId), folderName);
            return Ok(resultFile);
        }

        /// <summary>
        /// 修改文件名
        /// </summary>
        /// <param name="fileId">文件标识Id</param>
        /// <param name="newFileName">新文件名</param>
        /// <returns></returns>
        [HttpPost("/renameFile/{fileId}/{newFileName}")]
        [UnityOfWork(typeof(SqlServerDbContext))]
        public ActionResult RenameFile(string fileId, string newFileName) {
            if (string.IsNullOrEmpty(fileId) || string.IsNullOrEmpty(newFileName)) {
                return BadRequest("需要提供文件ID和新文件名");
            }

            var result = fileService.RenameFile((Guid)ConvertToGuid(fileId)!, newFileName);
            return Ok(result);
        }

        /// <summary>
        /// 把字符串类型ID转换成Guid类型ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private Guid? ConvertToGuid(string? id) {
            return string.IsNullOrEmpty(id) ? Guid.Empty : Guid.Parse(id);
        }
    }
}

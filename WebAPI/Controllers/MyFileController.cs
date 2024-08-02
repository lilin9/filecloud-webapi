using System.Text;
using System.Web;
using Domain.Vo;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Attributes;
using WebAPI.Services;

namespace WebAPI.Controllers {
    [ApiController]
    [Route("myFile")]
    public class MyFileController(FileService fileService, IWebHostEnvironment webHostEnvironment): ControllerBase {
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [HttpPost("upload/{parentId}")]
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
        [HttpPost("createFolder/{parentId}/{folderName}")]
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
        [HttpPost("renameFile/{fileId}/{newFileName}")]
        [UnityOfWork(typeof(SqlServerDbContext))]
        public ActionResult RenameFile(string fileId, string newFileName) {
            if (string.IsNullOrEmpty(fileId) || string.IsNullOrEmpty(newFileName)) {
                return BadRequest("需要提供文件ID和新文件名");
            }

            var result = fileService.RenameFile((Guid)ConvertToGuid(fileId)!, newFileName);
            return Ok(result);
        }

        /// <summary>
        /// 查询文件列表
        /// </summary>
        /// <param name="listVo"></param>
        /// <returns></returns>
        [HttpPost("searchList")]
        public ActionResult SearchList([FromBody] FileListVo listVo) {
            if (listVo.PageIndex <= 0) {
                listVo.PageIndex = 1;
            }

            if (listVo.PageSize < 10) {
                listVo.PageSize = 10;
            }

            listVo.GuidParentId = ConvertToGuid(listVo.ParentId);
            return Ok(fileService.GetList(listVo).Result);
        }

        /// <summary>
        /// 传入文件的物理路径，返回文件的二进制流
        /// </summary>
        /// <param name="absolutePath">文件物理绝对路径</param>
        /// <returns></returns>
        /// <exception cref="IOException"></exception>
        [HttpPost("download/${absolutePath}")]
        public async Task<IActionResult> DownloadFile(string absolutePath) {
            try {
                if (string.IsNullOrEmpty(absolutePath)) {
                    throw new ArgumentException("需要提供文件路径");
                }

                //对传入的文件路径进行解码
                var filePath = HttpUtility.UrlDecode(absolutePath);

                var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
                var fileName = filePath[(filePath.LastIndexOf('/') + 1)..];

                return File(bytes, "application/octet-stream", fileName);
            } catch {
                throw new IOException("文件下载失败，请重试");
            }
        }


        /// <summary>
        /// 把字符串类型ID转换成Guid类型ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private Guid? ConvertToGuid(string? id) {
            return string.IsNullOrEmpty(id?.Trim()) ? Guid.Empty : Guid.Parse(id);
        }
    }
}

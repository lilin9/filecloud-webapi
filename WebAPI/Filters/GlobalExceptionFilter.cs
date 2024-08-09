using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace WebAPI.Filters
{
    /// <summary>
    /// 本地异常统一处理中心
    /// </summary>
    public class GlobalExceptionFilter : IAsyncExceptionFilter
    {

        public Task OnExceptionAsync(ExceptionContext context)
        {
            ResponseResult exceptionResult;
            var exception = context.Exception.InnerException;
            if (exception is CustomReplyException)
            {
                exceptionResult = new ResponseResult
                {
                    Success = false,
                    Code = 400,
                    Message = context.Exception.InnerException?.Message
                };
            }
            else
            {
                exceptionResult = new ResponseResult
                {
                    Success = false,
                    Code = context.Exception.GetHashCode(),
                    Message = context.Exception.ToString()
                };
            }
            context.Result = new ContentResult
            {
                StatusCode = StatusCodes.Status200OK,
                ContentType = "application/json;charset=utf-8",
                Content = JsonConvert.SerializeObject(exceptionResult)
            };
            return Task.CompletedTask;
        }
    }
}

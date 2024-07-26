using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace WebAPI.Middleware {
    /// <summary>
    /// 本地异常统一处理中心
    /// </summary>
    public class GlobalExceptionFilter: IAsyncExceptionFilter {
        public Task OnExceptionAsync(ExceptionContext context) {
            if (!context.ExceptionHandled) {
                var result = new ResponseResult {
                    Success = false,
                    Code = context.HttpContext.Response.StatusCode,
                    Message = context.Exception.Message
                };
                context.Result = new ContentResult {
                    StatusCode = StatusCodes.Status200OK,
                    ContentType = "application/json;charset=utf-8",
                    Content = JsonConvert.SerializeObject(result)
                };
            }

            context.ExceptionHandled = true;
            return Task.CompletedTask;
        }
    }
}

using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebAPI.Filters
{
    /// <summary>
    /// 响应拦截器
    /// </summary>
    public class ResponseActionFilter : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ObjectResult objectResult)
            {
                var isSuccess = objectResult.StatusCode == 200;
                if (isSuccess)
                {
                    context.Result = new ObjectResult(new ResponseResult
                    {
                        Data = objectResult.Value,
                        Code = objectResult.StatusCode,
                        Success = isSuccess,
                    });
                }
                else
                {
                    context.Result = new ObjectResult(new ResponseResult
                    {
                        Message = objectResult.Value?.ToString(),
                        Code = objectResult.StatusCode,
                        Success = isSuccess,
                    });
                }
            }
            else
            {
                var code = context.HttpContext.Response.StatusCode;
                context.Result = new ObjectResult(new ResponseResult
                {
                    Message = context.HttpContext.Response.ContentType,
                    Code = code,
                    Success = code == 200
                });
            }
        }
    }
}

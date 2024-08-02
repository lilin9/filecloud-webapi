using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using WebAPI.Attributes;

namespace WebAPI.Middleware
{
    /// <summary>
    /// 工作单元过滤器
    /// </summary>
    public class UnityOfWorkFilter: IAsyncActionFilter {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) {
            var result = await next();
            //Action执行失败的话退出逻辑
            if (result.Exception != null) { return; }

            //获取所有 Controller 里面的 Attribute
            if (context.ActionDescriptor is not ControllerActionDescriptor actionDes) { return; }

            //在 Attribute 里面获取所有的 UnityOfWorkAttribute
            var unityOfWorkAttr = actionDes.MethodInfo.GetCustomAttribute<UnityOfWorkAttribute>();
            if (unityOfWorkAttr ==  null) { return; }

            foreach (var dbCtxType in unityOfWorkAttr.DbContextTypes)
            {
                //获取DbContext，执行 Save 操作
                var service = context.HttpContext.RequestServices.GetService(dbCtxType);
                if (service is DbContext dbCtx) {
                   await dbCtx.SaveChangesAsync();
                }
            }
        }
    }
}

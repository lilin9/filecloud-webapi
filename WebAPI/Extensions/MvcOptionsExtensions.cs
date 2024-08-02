using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using WebAPI.Filters;

namespace WebAPI.Extensions {
    /// <summary>
    /// 添加路由扩展方法
    /// </summary>
    public static class MvcOptionsExtensions {
        /// <summary>
        /// 统一添加路由前缀
        /// </summary>
        /// <param name="opts"></param>
        /// <param name="prefix"></param>
        public static void UseCentralRoutePrefix(this MvcOptions opts, string prefix) {
            opts.UseGeneralRoutePrefix(new RouteAttribute(prefix));
        }

        public static void UseGeneralRoutePrefix(this MvcOptions opts, IRouteTemplateProvider routes) { 
            opts.Conventions.Add(new RouteConvention(routes));
        }
    }
}

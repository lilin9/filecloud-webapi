using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Infrastructure;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using WebAPI.Middleware;
using WebAPI.Extensions;
using WebAPI.Filters;

var builder = WebApplication.CreateBuilder(args);

var sqlServerStr = builder.Configuration.GetConnectionString("SqlServerStr")!;
var routePrefixStr = builder.Configuration.GetSection("CustomStrings:RoutePrefixStr").Value;
var maxUploadFileSizeStr = builder.Configuration.GetSection("CustomStrings:MaxUploadFileSize").Value;

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//配置数据库连接
builder.Services.AddDbContext<SqlServerDbContext>(opt => {
    opt.UseSqlServer(sqlServerStr);
});

builder.Services.AddControllers(opt => {
    //注册全局响应拦截器
    opt.Filters.Add<ResponseActionFilter>();
    //注入全局异常处理器
    opt.Filters.Add(new GlobalExceptionFilter());
}).AddJsonOptions(opt => {
    //添加枚举型值对象的序列化转换器
    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

//全局统一添加路由前缀
builder.Services.AddControllers(opt => {
    opt.UseCentralRoutePrefix(routePrefixStr!);
});

//添加工作单元过滤器
builder.Services.Configure<MvcOptions>(opts => {
    opts.Filters.Add<UnityOfWorkFilter>();
});

//注册所有IOC接口
builder.Services.AddAllScope();

//设置请求头最大上传文件大小
var maxUploadFileSize = string.IsNullOrEmpty(maxUploadFileSizeStr) ? 0 : int.Parse(maxUploadFileSizeStr);
builder.Services.Configure<IISServerOptions>(opt => {
    opt.MaxRequestBodySize = maxUploadFileSize;
});
builder.Services.Configure<KestrelServerOptions>(opt => {
    opt.Limits.MaxRequestBodySize = maxUploadFileSize;
});

//配置 IFormFile 文件最大上传大小
builder.Services.Configure<FormOptions>(opt => {
    opt.MultipartBodyLengthLimit = maxUploadFileSize;
});

//注册缓存服务
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

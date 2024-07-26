using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using WebAPI.Middleware;
using WebAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//配置数据库连接
builder.Services.AddDbContext<SqlServerDbContext>(opt => {
    opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerStr"));
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
    opt.UseCentralRoutePrefix(builder.Configuration.GetSection("CustomStrings:RoutePrefixStr").Value!);
});

//添加工作单元过滤器
builder.Services.Configure<MvcOptions>(opts => {
    opts.Filters.Add<UnityOfWorkFilter>();
});

//注册所有IOC接口
builder.Services.AddAllScope();

//设置请求头最大上传文件大小
var maxUploadFileSizeStr = builder.Configuration.GetSection("CustomStrings:MaxUploadFileSize").Value;
var maxUploadFileSize = string.IsNullOrEmpty(maxUploadFileSizeStr) ? 0 : int.Parse(maxUploadFileSizeStr);
builder.Services.Configure<IISServerOptions>(opt => {
    opt.MaxRequestBodySize = maxUploadFileSize;
});
builder.Services.Configure<KestrelServerOptions>(opt => {
    opt.Limits.MaxRequestBodySize = maxUploadFileSize;
});

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

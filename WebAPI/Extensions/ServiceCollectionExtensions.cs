using Domain.Repository;
using Infrastructure.RepositoryImpl;
using WebAPI.Services;

namespace WebAPI.Extensions {
    public static class ServiceCollectionExtensions {
        /// <summary>
        /// 在这里添加所有映射
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection AddAllScope(this IServiceCollection services) {
            services.AddScoped<IFilesRepository, FilesRepository>();
            services.AddScoped<IEmailRepository, EmailRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddTransient<FileService>();
            services.AddTransient<UserService>();
            services.AddTransient<VerifyCodeService>();

            return services;
        }
    }
}

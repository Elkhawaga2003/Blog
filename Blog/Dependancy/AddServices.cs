using Blog.Helpers;
using Blog.Repository;
using Blog.Services.Implemetation;
using Blog.Services.Interfaces;
using Blog.UnitOfWorks;
using Microsoft.AspNetCore.SignalR;

namespace Blog.Dependancy
{
    public static class AddServices
    {
        public static IServiceCollection AddAServices(this IServiceCollection services)
        {
            services.AddSingleton<VerificationService>();
            services.AddSingleton<EmailSender>();
            services.AddTransient<IToxicDetector,ToxicDetector>();
            services.AddScoped<IFileServices, FileServices>();
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IJwtServices, JwtServices>();

            return services;
        }
    }
}

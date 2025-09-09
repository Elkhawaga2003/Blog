using Blog.Data;
using Blog.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Blog.Dependancy
{
    public static class general
    {
        public static IServiceCollection AddGServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularClient", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });
            services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
            services.AddDbContext<ApplicationDbContext>(Options =>

                Options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), options =>
                {
                    options.CommandTimeout(120);
                })
            );
            return services;
        }
    }
}

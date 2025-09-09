
using Blog.Dependancy;
using Blog.Helpers;

namespace Blog
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAServices()
                .AddGServices(builder.Configuration)
                .AddIServices(builder.Configuration);
            builder.Services.AddSignalR();
            
            var app = builder.Build();
            app.UseCors("AllowAngularClient");
            
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
       

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<NotificationHub>("/notificationHub");
            });
            app.MapControllers();

            app.Run();

        }
    }
}

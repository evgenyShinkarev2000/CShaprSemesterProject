using Microsoft.Extensions.FileProviders;
using ServerAPI.Services.ArchiveManager;
using ServerAPI.Services.FileManager;

namespace ServerAPI
{
    public static class CustomInit
    {
        public static void InitService(IServiceCollection services)
        {
            services.AddCors(options => options.AddPolicy("AllowAnyOriginsPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            services.AddSingleton<IFileManager, MockFileManager>();
            services.AddSingleton<IArchiveManager, MockArchiveManager>();
        }

        public static void InitConveyor(WebApplication app)
        {
            app.Services.GetService<IFileManager>(); // В случае чего упадет при запуске, а не через 2 часа
            app.Services.GetService<IArchiveManager>();
            app.UseCors("AllowAnyOriginsPolicy");

        }
    }
}

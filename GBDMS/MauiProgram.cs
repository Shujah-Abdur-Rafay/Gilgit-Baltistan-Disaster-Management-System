using Microsoft.Extensions.Logging;
using GBDMS.Data;
using GBDMS.Repository;
using GBDMS.Repository.IRepository;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using GBDMS.Services.ModelService;

namespace GBDMS
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

            // Register database and repository services
            builder.Services.AddSingleton<LocalDbService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            
            // Register model execution service
            builder.Services.AddScoped<PythonModelExecutor>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}

﻿using Microsoft.Extensions.Logging;
using GBDMS.Data;
using GBDMS.Repository;
using GBDMS.Repository.IRepository;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using GBDMS.Services.ModelService;
using GBDMS.Services;

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
            builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
            builder.Services.AddScoped<IIncidentRepository, IncidentRepository>();
            builder.Services.AddScoped<IDamageRepository, DamageRepository>();
            builder.Services.AddScoped<INgoRepository, NgoRepository>();
            builder.Services.AddScoped<IAlertSubscriptionRepository, AlertSubscriptionRepository>();
            builder.Services.AddScoped<IAlertRepository, AlertRepository>();
            builder.Services.AddScoped<ISurvivalGuidelineRepository, SurvivalGuidelineRepository>();
            builder.Services.AddScoped<IContactMessageRepository, ContactMessageRepository>();

            // Register model execution service
            builder.Services.AddScoped<PythonModelExecutor>();

            // Register alert service
            builder.Services.AddScoped<IAlertService, AlertService>();

            // Register email service
            // Gmail App Password configured for Mail app: rguafenybilqozpu

            // OPTION 1: Use Mock Email Service (for testing - logs to console)
            // builder.Services.AddScoped<IEmailService, MockEmailService>();

            // OPTION 2: Use Real Gmail SMTP (new App Password configured)
            builder.Services.AddScoped<IEmailService, EmailService>();

            // Register email test service for verification
            builder.Services.AddScoped<EmailTestService>();

            // Register authentication service
            builder.Services.AddScoped<GBDMS.Services.AuthenticationService>();

            // Register toast service
            builder.Services.AddScoped<ToastService>();

#if DEBUG
            // Only add developer tools if explicitly requested
            var enableDevTools = Environment.GetEnvironmentVariable("ENABLE_DEV_TOOLS");
            if (enableDevTools == "true")
            {
                builder.Services.AddBlazorWebViewDeveloperTools();
                builder.Logging.AddDebug();
            }
            else
            {
                // Use minimal logging to reduce debugger conflicts
                builder.Logging.SetMinimumLevel(LogLevel.Warning);
            }
#endif

            return builder.Build();
        }
    }
}

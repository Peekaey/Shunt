using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shunt.Main.Interfaces;
using Shunt.Main.Services;
using System;
using Shunt.Main.Services;

namespace Shunt.Main;

class Program
{
    private static IHost? Host { get; set; }

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        Host = CreateHostBuilder(args).Build();
        Host.Start();

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

        Host.StopAsync().GetAwaiter().GetResult();
        Host.Dispose();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
        => Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddSingleton<IGameService, GameService>();
                services.AddHostedService<BackgroundWorkerService>();
            });

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace();
}
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Shunt.Main.Interfaces;
using Shunt.Main.Models;
using Shunt.Main.Services;
using Shunt.Main.ViewModels;
using Shunt.Main.Views;

namespace Shunt.Main;

public partial class App : Application
{
    private SettingsWindow? _settingsWindow;
    private IServiceProvider _serviceProvider;
    public static IServiceProvider ServiceProvider { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var serviceProvider = ConfigureServices();
        ServiceProvider = serviceProvider;
        ReadSavedSettingsToMemory();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            // Controls whenever main window appears upon app startup
            // desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
    
    private void ExitMenuItem_OnClick(object? sender, EventArgs e)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }

    private void OpenSettingsMenuItem_OnClick(object? sender, EventArgs e)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime)
        {
            // Reuse one settings window instance so repeated menu clicks just refocus it.
            if (_settingsWindow is null || !_settingsWindow.IsVisible)
            {
                _settingsWindow = new SettingsWindow();
                _settingsWindow.Closed += (_, _) => _settingsWindow = null;
                _settingsWindow.Show();
            }
            else
            {
                _settingsWindow.WindowState = WindowState.Normal;
                _settingsWindow.Activate();
            }
        }
    }

    private void ModelQuickUnloadMenuItem_OnClick(object? sender, EventArgs e)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var apiManager = _serviceProvider.GetService<IApiManager>();
            apiManager.UnloadModel();
        }
    }

    private void ModelQuickLoadMenuItem_OnClick(object? sender, EventArgs e)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var apiManager = _serviceProvider.GetService<IApiManager>();
            apiManager.LoadModel();

        }
    }

    private void ReadSavedSettingsToMemory()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var services = scope.ServiceProvider;
            var appSettingsService = services.GetRequiredService<IAppSettingsService>();
            var cachedAppSettings = services.GetRequiredService<CachedAppSettings>();
            var settingsResult = appSettingsService.GetStoredAppSettings().GetAwaiter().GetResult();

            if (settingsResult.IsSuccess)
            {
                var loaded = settingsResult.AppSettings;
                cachedAppSettings.ServerIp = loaded.ServerIp;
                cachedAppSettings.ServerPort = loaded.ServerPort;
                cachedAppSettings.DefaultModel = loaded.DefaultModel;
                cachedAppSettings.ContextTokenLength = loaded.ContextTokenLength;
                cachedAppSettings.EnableAutoLoadUnload = loaded.EnableAutoLoadUnload;
                return;
            }
            else
            {
                var result = appSettingsService.ClearAndSaveDefaultAppSettings(new AppSettings()).GetAwaiter().GetResult();
                if (!result.IsSuccess)
                {
                    throw new InvalidOperationException($"Failed to save default app settings: {result.ErrorMessage}");
                }
                
                var loaded = result.AppSettings;
                cachedAppSettings.ServerIp = loaded.ServerIp;
                cachedAppSettings.ServerPort = loaded.ServerPort;
                cachedAppSettings.DefaultModel = loaded.DefaultModel;
                cachedAppSettings.ContextTokenLength = loaded.ContextTokenLength;
                cachedAppSettings.EnableAutoLoadUnload = loaded.EnableAutoLoadUnload;
            }

        }
    }

    public IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        services.AddHttpClient();
        services.AddSingleton<IAppSettingsService, AppSettingsService>();
        services.AddSingleton<IApiManager,ApiManager>();
        services.AddSingleton<IGameService, GameService>();
        services.AddSingleton<ISecretStoreService, SecretStoreService>();
        services.AddSingleton<CachedAppSettings>();
        // services.AddHostedService<BackgroundWorkerService>();
        services.AddTransient<SettingsWindowViewModel>();
        
        _serviceProvider = services.BuildServiceProvider();
        return _serviceProvider;
    }


}
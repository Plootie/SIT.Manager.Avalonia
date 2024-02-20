using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using SIT.Manager.Avalonia.Classes;
using SIT.Manager.Avalonia.Interfaces;
using SIT.Manager.Avalonia.ManagedProcess;
using SIT.Manager.Avalonia.Services;
using SIT.Manager.Avalonia.ViewModels;
using SIT.Manager.Avalonia.Views;
using System;
using System.Net.Http;

namespace SIT.Manager.Avalonia;

public sealed partial class App : Application
{
    /// <summary>
    /// Gets the current <see cref="App"/> instance in use
    /// </summary>
    public new static App Current => Application.Current as App ?? throw new ArgumentNullException(nameof(Application.Current));

    /// <summary>
    /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
    /// </summary>
    public IServiceProvider Services { get; }

    public App() {
        Services = ConfigureServices();
    }

    /// <summary>
    /// Configures the services for the application.
    /// </summary>
    private static IServiceProvider ConfigureServices() {
        var services = new ServiceCollection();

        // Services
        services.AddSingleton<IActionNotificationService, ActionNotificationService>();
        services.AddSingleton<IAkiServerService, AkiServerService>();
        services.AddSingleton<ITarkovClientService, TarkovClientService>();
        services.AddSingleton<IBarNotificationService, BarNotificationService>();
        services.AddTransient<IFileService, FileService>();
        services.AddTransient<IFolderPickerService>(x => {
            if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop || desktop.MainWindow?.StorageProvider is not { } provider) {
                return new FolderPickerService(new MainWindow());
            }
            return new FolderPickerService(desktop.MainWindow);
        });
        services.AddSingleton<IManagerConfigService, ManagerConfigService>();
        services.AddTransient<IModService, ModService>();
        services.AddSingleton<IVersionService, VersionService>();
        services.AddSingleton(new HttpClientHandler
        {
            SslProtocols = System.Security.Authentication.SslProtocols.Tls12,
            ServerCertificateCustomValidationCallback = delegate { return true; }
        });
        services.AddSingleton(provider => new HttpClient(provider.GetService<HttpClientHandler>() ?? throw new ArgumentNullException()));
        services.AddSingleton<IZlibCompressionService, ZlibCompressionService>();

        // Viewmodels
        services.AddTransient<MainViewModel>();
        services.AddTransient<ModsPageViewModel>();
        services.AddTransient<SettingsPageViewModel>();
        services.AddTransient<ServerPageViewModel>();
        services.AddTransient<PlayPageViewModel>();

        return services.BuildServiceProvider();
    }

    public override void Initialize() {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted() {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            desktop.MainWindow = new MainWindow {
                DataContext = Current.Services.GetService<MainViewModel>()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform) {
            singleViewPlatform.MainView = new MainView {
                DataContext = Current.Services.GetService<MainViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}

using System;
using System.Windows;
using MediaOverlayWPF.Classes;
using MediaOverlayWPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace MediaOverlayWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public App()
        {
            ServiceCollection Services = new();
            Services.AddSingleton<IDispatcherService, WpfDispatcherService>();
            Services.AddSingleton<MediaManagerViewModel>();
            Services.AddSingleton<AudioManagerViewModel>();
            Services.AddTransient<MainWindow>();

            ServiceProvider = Services.BuildServiceProvider();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            MediaManagerViewModel MediaManagerVW = ServiceProvider.GetRequiredService<MediaManagerViewModel>();
            MediaManagerVW.Dispose();

            AudioManagerViewModel AudioManagerVM = ServiceProvider.GetRequiredService<AudioManagerViewModel>();
            AudioManagerVM.Dispose();
        }
    }

}

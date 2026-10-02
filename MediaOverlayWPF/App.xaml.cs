using System;
using System.Windows;
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
            Services.AddSingleton<MediaManagerViewModel>();
            Services.AddTransient<MainWindow>();

            ServiceProvider = Services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //var MainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            //MainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            MediaManagerViewModel MediaManagerVW = ServiceProvider.GetRequiredService<MediaManagerViewModel>();
            MediaManagerVW.Dispose();
        }
    }

}

using System;
using System.Collections.ObjectModel;
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
            Services.AddSingleton<AudioManagerViewModel>();
            Services.AddTransient<MainWindow>();

            ServiceProvider = Services.BuildServiceProvider();
        }

        public static void ChangeStyle(string Style)
        {
            ResourceDictionary NewStyle = new();
            NewStyle.Source = new Uri($"Styles/{Style}.xaml", UriKind.Relative);

            Collection<ResourceDictionary> MergedDictionaries = App.Current.Resources.MergedDictionaries;
            MergedDictionaries[1] = NewStyle;
        }

        public static void ChangeTheme(string Theme)
        {
            ResourceDictionary NewTheme = new();
            NewTheme.Source = new Uri($"Styles/{Theme}.xaml", UriKind.Relative);

            Collection<ResourceDictionary> MergedDictionaries = App.Current.Resources.MergedDictionaries;
            MergedDictionaries[0] = NewTheme;
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

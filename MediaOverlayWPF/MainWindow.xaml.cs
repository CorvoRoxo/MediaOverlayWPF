using MediaOverlayWPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace MediaOverlayWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ViewModelLocator PageViewModel = new();

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = PageViewModel;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            double ScreenWidth = SystemParameters.PrimaryScreenWidth;
            this.Left = (ScreenWidth - this.Width) / 2;

            this.Top = 0;
        }

        private void ToggleMainContainerVisibility_ButtonClick(object sender, RoutedEventArgs e)
        {
            MainContainer.Visibility = MainContainer.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }
    }

    public class ViewModelLocator
    {
        public MediaManagerViewModel MediaManagerVM => App.ServiceProvider.GetRequiredService<MediaManagerViewModel>();
        public AudioManagerViewModel AudioManagerVM => App.ServiceProvider.GetRequiredService<AudioManagerViewModel>();
    }
}
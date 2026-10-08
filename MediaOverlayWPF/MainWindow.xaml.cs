using MediaOverlayWPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace MediaOverlayWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ViewModelLocator PageViewModel = new();
        string AppPositionSelectedOption = "top";

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

        private async void ToggleMainContainerVisibility_ButtonClick(object sender, RoutedEventArgs e)
        {
            MainContainer.Visibility = MainContainer.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

            await Task.Delay(10);
            AdjustPosition();
        }

        private async void ToggleOptionsContainerVisibility_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OptionsContainer.Visibility = OptionsContainer.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

            await Task.Delay(10);
            AdjustPosition();
        }

        private void OptionsMenu_ButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is not Button ClickedButton || ClickedButton.ContextMenu is null)
                return;

            ClickedButton.ContextMenu.IsOpen = true;
        }

        private void ChangeAppPosition_MenuItemClick(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem ClickedOption)
                return;

            AppPositionSelectedOption = ClickedOption.Header.ToString()!.ToLower().Split(' ')[1];
            AdjustPosition();

            Debug.WriteLine($"{ClickedOption.Header}");
        }

        private void AdjustPosition()
        {
            switch (AppPositionSelectedOption.ToLower())
            {
                case "top":
                    MainContainer.SetValue(Grid.RowProperty, 2);
                    OptionsContainer.SetValue(Grid.RowProperty, 1);
                    ContainersVisibilityTogglerButton.SetValue(Grid.RowProperty, 0);

                    this.Top = 0;
                    Debug.WriteLine($"Adjusted position to tiop");

                    break;

                case "bottom":
                    MainContainer.SetValue(Grid.RowProperty, 0);
                    OptionsContainer.SetValue(Grid.RowProperty, 1);
                    ContainersVisibilityTogglerButton.SetValue(Grid.RowProperty, 2);

                    double ScreenHeight = SystemParameters.WorkArea.Height;
                    double AppHeight = this.ActualHeight;

                    this.Top = ScreenHeight - AppHeight;
                    Debug.WriteLine($"Adjusted position to bomtom");

                    break;
            }
        }

        private void ForceUIUpdateMenuItem_Click(object sender, RoutedEventArgs e)
        {
            PageViewModel.MediaManagerVM.UpdateMediaProperties();
            Debug.WriteLine($"UI updated");
        }

        private void ChangeAppStyle_MenuItemClick(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem MenuItem)
                return;

            string StyleSelected = MenuItem.Header.ToString() == "compacted" ? "AppCompactedMode" : "AppDetailedMode";
            App.ChangeStyle(StyleSelected);
        }

        private void ChangeAppTheme_MenuItemClick(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem MenuItem)
                return;

            string ThemeSelected = MenuItem.Header.ToString() == "light" ? "WindowLightTheme" : "WindowDarkTheme";
            App.ChangeTheme(ThemeSelected);
        }
    }

    public class ViewModelLocator
    {
        public MediaManagerViewModel MediaManagerVM => App.ServiceProvider.GetRequiredService<MediaManagerViewModel>();
        public AudioManagerViewModel AudioManagerVM => App.ServiceProvider.GetRequiredService<AudioManagerViewModel>();
    }
}
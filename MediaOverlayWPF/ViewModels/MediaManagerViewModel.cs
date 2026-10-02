using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Media.Imaging;
using Windows.ApplicationModel.Calls.Background;
using Windows.Media.Control;

namespace MediaOverlayWPF.ViewModels
{
    public partial class MediaManagerViewModel : ObservableObject
    {
        public GlobalSystemMediaTransportControlsSessionManager? MediaManager { get; set; }

        [ObservableProperty]
        public partial GlobalSystemMediaTransportControlsSessionMediaProperties? CurrentSessionProperties { get; private set; }

        [ObservableProperty]
        public partial BitmapImage Thumbnail { get; private set; } = new();

        public MediaManagerViewModel()
        {
            InitializeMediaManager();
        }

        public void Dispose()
        {
            if (MediaManager is null)
                return;

            MediaManager.CurrentSessionChanged -= MediaManager_CurrentSessionChanged;
            MediaManager.SessionsChanged -= MediaManager_SessionsChanged;
        }

        [RelayCommand]
        public async Task TryTogglePlayPause()
        {
            if (MediaManager is null)
                return;

            GlobalSystemMediaTransportControlsSession CurrentSession = MediaManager.GetCurrentSession();
            if (CurrentSession is null)
                return;

            await CurrentSession.TryTogglePlayPauseAsync();
        }

        [RelayCommand]
        public async Task TryPlayNext()
        {
            if (MediaManager is null)
                return;

            GlobalSystemMediaTransportControlsSession CurrentSession = MediaManager.GetCurrentSession();
            if (CurrentSession is null)
                return;

            await CurrentSession.TrySkipNextAsync();
        }

        [RelayCommand]
        public async Task TryPlayPrevious()
        {
            if (MediaManager is null)
                return;

            GlobalSystemMediaTransportControlsSession CurrentSession = MediaManager.GetCurrentSession();
            if (CurrentSession is null)
                return;

            await CurrentSession.TrySkipPreviousAsync();
        }

        private async void InitializeMediaManager()
        {
            MediaManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
            MediaManager.CurrentSessionChanged += MediaManager_CurrentSessionChanged;
            MediaManager.SessionsChanged += MediaManager_SessionsChanged;

            UpdateCurrentSessionProperties();
        }

        private void MediaManager_SessionsChanged(GlobalSystemMediaTransportControlsSessionManager sender, SessionsChangedEventArgs args)
            => UpdateCurrentSessionProperties();
        private void MediaManager_CurrentSessionChanged(GlobalSystemMediaTransportControlsSessionManager sender, CurrentSessionChangedEventArgs args)
            => UpdateCurrentSessionProperties();

        private async void UpdateCurrentSessionProperties()
        {
            if (MediaManager is null)
                return;

            GlobalSystemMediaTransportControlsSession CurrentSession = MediaManager.GetCurrentSession();
            if (CurrentSession is null)
                return;

            GlobalSystemMediaTransportControlsSessionMediaProperties MediaProperties = await CurrentSession.TryGetMediaPropertiesAsync();
            CurrentSessionProperties = MediaProperties;

            await Task.Delay(100);
            using var RandomAccessStreamImage = await MediaProperties.Thumbnail.OpenReadAsync();
            using var ImageStream = RandomAccessStreamImage.AsStream();

            using MemoryStream ThumbnailMemoryStream = new();
            ThumbnailMemoryStream.Position = 0;
            await ImageStream.CopyToAsync(ThumbnailMemoryStream);

            BitmapImage NewBitMap = new();
            NewBitMap.BeginInit();
            NewBitMap.CacheOption = BitmapCacheOption.OnLoad;
            NewBitMap.StreamSource = ThumbnailMemoryStream;
            NewBitMap.EndInit();
            NewBitMap.Freeze();

            Thumbnail = NewBitMap;

            Debug.WriteLine($"Media Changed to: {MediaProperties.Title}");
        }
    }
}

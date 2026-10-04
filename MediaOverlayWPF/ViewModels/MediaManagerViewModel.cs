using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using Windows.Media.Control;

namespace MediaOverlayWPF.ViewModels
{
    public partial class MediaManagerViewModel : ObservableObject, IDisposable
    {
        private readonly IDispatcherService _DispatcherService;

        public GlobalSystemMediaTransportControlsSessionManager? MediaManager { get; set; }

        [ObservableProperty]
        public partial MediaProperties? CurrentSessionProperties { get; private set; }

        [ObservableProperty]
        public partial BitmapImage Thumbnail { get; private set; } = new();

        public MediaManagerViewModel(IDispatcherService dispatcherService)
        {
            _DispatcherService = dispatcherService;
            InitializeMediaManager();
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

        public void Dispose()
        {
            if (MediaManager is null)
                return;

            MediaManager.CurrentSessionChanged -= MediaManager_CurrentSessionChanged;
            MediaManager.SessionsChanged -= MediaManager_SessionsChanged;
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

            await Task.Delay(200);
            try
            {
                Task<GlobalSystemMediaTransportControlsSessionMediaProperties> MediaProperties = await _DispatcherService.InvokeAsync(async () =>
                        await CurrentSession.TryGetMediaPropertiesAsync().AsTask());
                
                CurrentSessionProperties = new(await MediaProperties);
            }
            catch (Exception Ex)
            {
                Debug.WriteLine(Ex);
            }
        }
    }
}
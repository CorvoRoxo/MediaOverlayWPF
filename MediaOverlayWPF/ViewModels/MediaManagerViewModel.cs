using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Windows.Media.Control;

namespace MediaOverlayWPF.ViewModels
{
    public partial class MediaManagerViewModel : ObservableObject, IDisposable
    {
        private GlobalSystemMediaTransportControlsSessionManager? SessionManager;
        private GlobalSystemMediaTransportControlsSession? CurrentSession;
        private CancellationTokenSource? DebounceTokenSource;

        [ObservableProperty]
        public partial MediaProperties? CurrentMediaProperties { get; private set; }

        [ObservableProperty]
        public partial GlobalSystemMediaTransportControlsSessionPlaybackInfo? CurrentPlaybackInfo { get; private set; }

        [ObservableProperty]
        public partial MediaTimeline CurrentTimelineProperties { get; private set; } = new();

        public MediaManagerViewModel()
        {
            InitializeMediaManager();
        }

        [RelayCommand]
        public async Task TryTogglePlayPause()
        {
            if (CurrentSession is null)
                return;

            await CurrentSession.TryTogglePlayPauseAsync();
        }

        [RelayCommand]
        public async Task TryPlayNext()
        {
            if (CurrentSession is null)
                return;

            await CurrentSession.TrySkipNextAsync();
        }

        [RelayCommand]
        public async Task TryPlayPrevious()
        {
            if (CurrentSession is null)
                return;

            await CurrentSession.TrySkipPreviousAsync();
        }

        private async void InitializeMediaManager()
        {
            SessionManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
            SessionManager.CurrentSessionChanged += MediaManager_CurrentSessionChanged;
            SessionManager.SessionsChanged += SessionManager_SessionsChanged;

            UpdateCurrentSession();
        }

        private void SessionManager_SessionsChanged(GlobalSystemMediaTransportControlsSessionManager sender, SessionsChangedEventArgs args)
            => UpdateCurrentSession();

        public void Dispose()
        {
            if (SessionManager is null)
                return;

            SessionManager.CurrentSessionChanged -= MediaManager_CurrentSessionChanged;
            SessionManager.SessionsChanged -= SessionManager_SessionsChanged;
        }

        private void MediaManager_CurrentSessionChanged(GlobalSystemMediaTransportControlsSessionManager sender, CurrentSessionChangedEventArgs args)
            => UpdateCurrentSession();

        private void CurrentSession_MediaPropertiesChanged(GlobalSystemMediaTransportControlsSession sender, MediaPropertiesChangedEventArgs args)
            => UpdateMediaProperties();

        private void CurrentSession_PlaybackInfoChanged(GlobalSystemMediaTransportControlsSession sender, PlaybackInfoChangedEventArgs args)
            => UpdateMediaProperties();

        public async void UpdateCurrentSession()
        {
            if (SessionManager is null)
                return;

            if (CurrentSession is not null)
            { 
                CurrentSession.MediaPropertiesChanged -= CurrentSession_MediaPropertiesChanged;
                CurrentSession.PlaybackInfoChanged -= CurrentSession_PlaybackInfoChanged;
                CurrentSession.TimelinePropertiesChanged -= CurrentSession_TimelinePropertiesChanged;
            }

            CurrentSession = SessionManager.GetCurrentSession();

            if (CurrentSession is null)
                return;

            CurrentSession.MediaPropertiesChanged += CurrentSession_MediaPropertiesChanged;
            CurrentSession.PlaybackInfoChanged += CurrentSession_PlaybackInfoChanged;
            CurrentSession.TimelinePropertiesChanged += CurrentSession_TimelinePropertiesChanged;

            UpdateMediaProperties();
            UpdateTimelineProperties();
        }

        private void CurrentSession_TimelinePropertiesChanged(GlobalSystemMediaTransportControlsSession sender, TimelinePropertiesChangedEventArgs args)
            => UpdateTimelineProperties();
        public async void UpdateTimelineProperties()
        {
            if (CurrentSession is null || CurrentPlaybackInfo is null)
                return;

            try
            {
                await Task.Delay(50);
                GlobalSystemMediaTransportControlsSessionTimelineProperties TimeLine = CurrentSession.GetTimelineProperties();
                CurrentTimelineProperties.UpdateTimeline(TimeLine, CurrentPlaybackInfo.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing);

                Debug.WriteLine($"PlaybackStatus inside UpdateTimelineProperties(): {CurrentPlaybackInfo.PlaybackStatus}");
            }
            catch (Exception) { }
        }

        public async void UpdateMediaProperties()
        {
            DebounceTokenSource?.Cancel();
            DebounceTokenSource = new();

            CancellationToken Token = DebounceTokenSource.Token;

            await Task.Run(async () =>
            {
                try
                {
                    if (CurrentSession is null)
                    {
                        CurrentMediaProperties = null;
                        return;
                    }

                    GlobalSystemMediaTransportControlsSessionPlaybackInfo? PlaybackInfo = CurrentSession.GetPlaybackInfo();
                    CurrentPlaybackInfo = PlaybackInfo;

                    await Task.Delay(200, Token);
                    if (Token.IsCancellationRequested)
                        return;          

                    GlobalSystemMediaTransportControlsSessionMediaProperties? MediaProperties = await CurrentSession.TryGetMediaPropertiesAsync();

                    if (MediaProperties is null || PlaybackInfo is null)
                        return;

                    CurrentMediaProperties = new(MediaProperties);
                }
                catch (TaskCanceledException) { }
                catch (Exception) { }
            });
        }
    }
}
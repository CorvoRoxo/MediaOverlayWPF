using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;
using System.Windows.Threading;
using Windows.Media.Control;

namespace MediaOverlayWPF.ViewModels
{
    public partial class MediaTimeline : ObservableObject
    {
        private DispatcherTimer DispatcherTimer;
        private GlobalSystemMediaTransportControlsSessionTimelineProperties? TimelineInfo;
        private bool IsPlaying;

        [ObservableProperty]
        public partial TimeSpan CurrentPosition { get; private set; }

        [ObservableProperty]
        public partial TimeSpan EndTime { get; private set; }

        [ObservableProperty]
        public partial double TotalSeconds { get; private set; }

        [ObservableProperty]
        public partial double CurrentSeconds { get; private set; }

        public MediaTimeline()
        {
            DispatcherTimer = new(DispatcherPriority.Background);
            DispatcherTimer.Interval = TimeSpan.FromMilliseconds(200);
            DispatcherTimer.Tick += DispatcherTimer_Tick;
        }
        
        public void UpdateTimeline(GlobalSystemMediaTransportControlsSessionTimelineProperties NewTimelineInfo, bool IsPlaying)
        {
            TimelineInfo = NewTimelineInfo;
            EndTime = TimelineInfo.EndTime;
            TotalSeconds = EndTime.TotalSeconds;
            this.IsPlaying = IsPlaying;

            if (TimelineInfo is null)
                return;

            if (IsPlaying)
                DispatcherTimer.Start();
            else
                DispatcherTimer.Stop();

            UpdateCurrentTime();
        }

        private void DispatcherTimer_Tick(object? sender, EventArgs e)
            => UpdateCurrentTime();

        private void UpdateCurrentTime()
        {
            if (TimelineInfo is null)
                return;

            if (!IsPlaying)
            {
                CurrentPosition = TimelineInfo.Position;
                CurrentSeconds = CurrentPosition.TotalSeconds;
                return;
            }

            TimeSpan TimeSinceLastUpdate = DateTime.UtcNow - TimelineInfo.LastUpdatedTime.UtcDateTime;
            TimeSpan CalculatedPosition = TimelineInfo.Position + TimeSinceLastUpdate;

            if (CalculatedPosition > TimelineInfo.EndTime)
                CalculatedPosition = TimelineInfo.EndTime;

            CurrentPosition = CalculatedPosition;
            CurrentSeconds = CurrentPosition.TotalSeconds;

            //Debug.WriteLine($"Current Time: {CalculatedPosition} / {EndTime}");
        }
    }
}

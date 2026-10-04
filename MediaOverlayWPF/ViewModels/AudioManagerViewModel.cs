using CommunityToolkit.Mvvm.ComponentModel;
using NAudio.CoreAudioApi;
using System.Windows;

namespace MediaOverlayWPF.ViewModels
{
    public partial class AudioManagerViewModel : ObservableObject, IDisposable
    {
        public MMDevice DefaultMMDevice { get; private set; }

        [ObservableProperty]
        public partial double Volume { get; set; }

        private bool IsUpdatingFromSystem = false;

        public AudioManagerViewModel()
        {
            MMDeviceEnumerator MMDeviceEnumerator = new();

            DefaultMMDevice = MMDeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            Volume = DefaultMMDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100;

            DefaultMMDevice.AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;
        }

        private void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
            {
                IsUpdatingFromSystem = true;
                Volume = data.MasterVolume * 100;

                IsUpdatingFromSystem = false;
            }));
        }

        partial void OnVolumeChanged(double value)
        {
            if (IsUpdatingFromSystem)
                return;

            float ScalarVolume = (float)(value / 100.0);
            DefaultMMDevice.AudioEndpointVolume.MasterVolumeLevelScalar = ScalarVolume;
        }

        public void Dispose()
        {
            if (DefaultMMDevice is null)
                return;

            DefaultMMDevice.AudioEndpointVolume.OnVolumeNotification -= AudioEndpointVolume_OnVolumeNotification;
            DefaultMMDevice.Dispose();
        }
    }
}

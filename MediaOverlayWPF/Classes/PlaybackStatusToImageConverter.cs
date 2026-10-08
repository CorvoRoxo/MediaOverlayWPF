using System.Windows.Data;
using System.Globalization;
using Windows.Media.Control;
using System.Windows.Media.Imaging;

namespace MediaOverlayWPF.Classes
{
    public class PlaybackStatusToImageConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            GlobalSystemMediaTransportControlsSessionPlaybackStatus? PlaybackStatus = Value as GlobalSystemMediaTransportControlsSessionPlaybackStatus?;

            string ImagePath = PlaybackStatus switch
            {
                GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing => "pack://application:,,,/Images/pause_small.png",
                GlobalSystemMediaTransportControlsSessionPlaybackStatus.Paused => "pack://application:,,,/Images/play_small.png",
                null => "pack://application:,,,/Images/play_small.png",
                _ => "pack://application:,,,/Images/play_small.png"
            };

            return new BitmapImage(new Uri(ImagePath));
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            throw new NotImplementedException();
        }
    }
}

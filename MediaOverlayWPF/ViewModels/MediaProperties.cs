using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;
using System.Windows.Media.Imaging;
using Windows.Media;
using Windows.Media.Control;
using Windows.Storage.Streams;

namespace MediaOverlayWPF.ViewModels
{
    public partial class MediaProperties : ObservableObject
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string AlbumArtist { get; set; }
        public string AlbumTitle { get; set; }
        public string Subtitle { get; set; }
        public List<string> Genres { get; set; }
        public int AlbumTrackCount { get; set; }
        public MediaPlaybackType? PlaybackType { get; set; }

        [ObservableProperty]
        public partial BitmapImage? Thumbnail { get; set; }

        public GlobalSystemMediaTransportControlsSessionMediaProperties GSMTTCSMediaProperties { get; }

        public MediaProperties(GlobalSystemMediaTransportControlsSessionMediaProperties GSMTTCSMediaProperties)
        {
            this.GSMTTCSMediaProperties = GSMTTCSMediaProperties;

            Title = GSMTTCSMediaProperties.Title;
            Artist = GSMTTCSMediaProperties.Artist;
            AlbumArtist = GSMTTCSMediaProperties.AlbumArtist;
            AlbumTitle = GSMTTCSMediaProperties.AlbumTitle;
            Subtitle = GSMTTCSMediaProperties.Subtitle;
            Genres = GSMTTCSMediaProperties.Genres.ToList();
            AlbumTrackCount = GSMTTCSMediaProperties.AlbumTrackCount;
            PlaybackType = GSMTTCSMediaProperties.PlaybackType;

            _ = LoadThumbnailAsync();
        }

        public async Task LoadThumbnailAsync()
        {
            if (GSMTTCSMediaProperties.Thumbnail is null)
            {
                Thumbnail = null;
                return;
            }

            using IRandomAccessStreamWithContentType RandomAccessStreamImage = await GSMTTCSMediaProperties.Thumbnail.OpenReadAsync();
            using Stream ImageStream = RandomAccessStreamImage.AsStream();

            BitmapImage BitmapImage = new();
            BitmapImage.BeginInit();
            BitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            BitmapImage.StreamSource = ImageStream;
            BitmapImage.EndInit();
            BitmapImage.Freeze();

            Thumbnail = BitmapImage;
        }
    }
}

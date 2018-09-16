using SpotifyAPI.Local;

namespace Audio_Visualizer.SpotifyAPI
{
    public class Spotify
    {
        private static SpotifyLocalAPI _spotify;

        public Spotify()
        {
            _spotify = new SpotifyLocalAPI();
            if (!SpotifyLocalAPI.IsSpotifyRunning()) { return; }
            if (!SpotifyLocalAPI.IsSpotifyWebHelperRunning()) { return; }

            try
            {
                if(!_spotify.Connect()) { return; }
                _spotify.ListenForEvents = true;
                _spotify.OnTrackChange += OnTrackChange;
                _spotify.OnTrackTimeChange += OnTrackTimeChange;
            }
            catch { }
        }

        private static void OnTrackChange(object sender, TrackChangeEventArgs e)
        {
            //
        }

        private static void OnTrackTimeChange(object sender, TrackTimeChangeEventArgs e)
        {

        }
    }
}



/*
 * 
 * private static SpotifyLocalAPI _spotify;
        public static bool trackHasChanged;
        public static bool trackTimeHasChanged;
        public static double trackTime;

        public static void SetUp()
        {
            _spotify = new SpotifyLocalAPI();
            if (!SpotifyLocalAPI.IsSpotifyRunning()) { return; }
            if (!SpotifyLocalAPI.IsSpotifyWebHelperRunning()) { return; }
            try
            {
                if (!_spotify.Connect()) { return; }
                _spotify.ListenForEvents = true;
                _spotify.OnTrackChange += OnTrackChange;
                _spotify.OnTrackTimeChange += OnTrackTimeChange;
            }
            catch { }
        }

        public static void GetSong(out string name, out string artist, out string album, out float trackLength)
        {
            name = "";
            artist = "";
            album = "";
            trackLength = 0f;

            StatusResponse status = _spotify.GetStatus();

            try
            {
                name = status.Track.TrackResource.Name;
                artist = status.Track.ArtistResource.Name;
                album = status.Track.AlbumResource.Name;
                trackLength = status.Track.Length;
            }
            catch
            {
                name = "";
                artist = "";
                album = "";
                trackLength = 0;
            }
        }

        public static Bitmap GetAlbumArt()
        {
            StatusResponse status = _spotify.GetStatus();
            try { return status.Track.GetAlbumArt(AlbumArtSize.Size160); }
            catch { return null; }
        }

        private static void OnTrackChange(object sender, TrackChangeEventArgs e)
        {
            trackHasChanged = true;
        }

        private static void OnTrackTimeChange(object sender, TrackTimeChangeEventArgs e)
        {
            trackTimeHasChanged = true;
            trackTime = e.TrackTime;
        }

        public static void Pause() { _spotify.Pause(); }
        public static void Play() { _spotify.Play(); }
        public static void Prev() { _spotify.Previous(); }
        public static void Skip() { _spotify.Skip(); }
        public static bool IsPlaying()
        {
            try { return _spotify.GetStatus().Playing; }
            catch { return false; }
        }

        public static bool IsConnected()
        {
            return _spotify.GetStatus() != null;
        }


*/
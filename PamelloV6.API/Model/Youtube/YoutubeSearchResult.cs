namespace PamelloV6.API.Model.Youtube
{
    public class YoutubeSearchResult
    {
        public int ResultsCount;
        public List<PamelloSong> PamelloSongs;
        public List<YoutubeSearchVideoInfo> YoutubeVideos;

        public YoutubeSearchResult() {
            ResultsCount = 0;
            PamelloSongs = new List<PamelloSong>();
            YoutubeVideos = new List<YoutubeSearchVideoInfo>();
        }
    }
}

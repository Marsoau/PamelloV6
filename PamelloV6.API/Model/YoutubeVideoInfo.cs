namespace PamelloV6.API.Model
{
	public class YoutubeVideoInfo
	{
		public string Name { get; set; }
		public string Author { get; set; }
		public List<YoutubeEpisodeInfo> Episodes { get; set; }
	}
}

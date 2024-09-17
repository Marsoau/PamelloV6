using AngleSharp.Dom;
using AngleSharp;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Web;
using PamelloV6.API.Exceptions;
using PamelloV6.API.Model.Youtube;
using PamelloV6.API.Repositories;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Google.Apis.Services;
using PamelloV6.API.Model;

namespace PamelloV6.API.Services
{
    public class PamelloYoutubeService
	{
		public readonly IHttpClientFactory _httpClientFactory;
		public readonly IServiceProvider _services;

		public PamelloYoutubeService(IHttpClientFactory httpClientFactory, IServiceProvider services) {
			_httpClientFactory = httpClientFactory;
			_services = services;
		}

		public string GetVideoIdFromUrl(string url) {
			var uri = new Uri(url);
			var query = HttpUtility.ParseQueryString(uri.Query);

			var youtubeId = query["v"];
			if (youtubeId is null || youtubeId.Length != 11) {
				throw new PamelloException($"cant find youtube id in url \"{url}\"");
			}

            return youtubeId;
        }

		public async Task<YoutubeVideoInfo> GetVideoInfo(string youtubeId) {
			var client = _httpClientFactory.CreateClient();

			var responce = await client.GetAsync($"https://www.youtube.com/watch?v={youtubeId}");

			var config = Configuration.Default.WithDefaultLoader();
			var context = BrowsingContext.New(config);
			var html = await context.OpenAsync(response => response.Content(responce.Content.ReadAsStream()));

			var youtubeVideoInfo = new YoutubeVideoInfo();

			IHtmlCollection<IElement> metaElements = html.QuerySelectorAll("meta");
			foreach (var metaElement in metaElements) {
				if (metaElement.GetAttribute("name") == "title") {
					youtubeVideoInfo.Name = metaElement.GetAttribute("content") ?? "";
					break;
				}
			}
			var span = html.QuerySelectorAll("span").First(
				s => s.GetAttribute("itemprop") == "author"
			);
			var link = span.QuerySelectorAll("link").First(l => l.GetAttribute("itemprop") == "name");

			youtubeVideoInfo.Author = link.GetAttribute("content") ?? "";

			var json = GetVideoJson(html);

			youtubeVideoInfo.Episodes = await GetVideoEpisodes(json);

			return youtubeVideoInfo;
		}

		private JsonDocument GetVideoJson(IDocument videoHtml) {
			string? jsonStr = null;
			IHtmlCollection<IElement> scriptElements = videoHtml.QuerySelectorAll("script");
			foreach (IElement scriptElement in scriptElements) {
				if (scriptElement.InnerHtml.StartsWith("var ytInitialData")) {
					jsonStr = scriptElement.InnerHtml.Substring(20, scriptElement.InnerHtml.Length - 21);
					break;
				}
			}

			if (jsonStr is null) {
				throw new PamelloException("Couldnt find requires json object in html");
			}

			File.WriteAllText(@"D:\json\v.json", jsonStr);

			return JsonDocument.Parse(jsonStr ?? "{}");
		}

		private async Task<List<YoutubeEpisodeInfo>> GetVideoEpisodes(JsonDocument videoJson) {
			var episodes = new List<YoutubeEpisodeInfo>();

			JsonElement chapterElements;
			try {
				chapterElements = videoJson.RootElement.GetProperty("playerOverlays")
					.GetProperty("playerOverlayRenderer")
					.GetProperty("decoratedPlayerBarRenderer")
					.GetProperty("decoratedPlayerBarRenderer")
					.GetProperty("playerBar")
					.GetProperty("multiMarkersPlayerBarRenderer")
					.GetProperty("markersMap")
					[0]
					.GetProperty("value")
					.GetProperty("chapters");
			}
			catch {
				return episodes;
			}

			for (int i = 0; i < chapterElements.GetArrayLength(); i++) {
				episodes.Add(new YoutubeEpisodeInfo() {
					Name = chapterElements[i]
						.GetProperty("chapterRenderer")
						.GetProperty("title")
						.GetProperty("simpleText").ToString(),

					Start = int.Parse(
						chapterElements[i]
							.GetProperty("chapterRenderer")
							.GetProperty("timeRangeStartMillis").ToString()
					) / 1000
				});
			}

			return episodes;
		}

        public async Task<YoutubeSearchResult> Search(int pageSize, string? query) {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer() {
                ApiKey = PamelloConfig.YoutubeToken,
            });

            var searchRequest = youtubeService.Search.List("snippet");
            searchRequest.Q = query;
            searchRequest.MaxResults = pageSize;
            searchRequest.SafeSearch = SearchResource.ListRequest.SafeSearchEnum.None;

            var searchResponse = await searchRequest.ExecuteAsync();
			var searchResult = new YoutubeSearchResult();

			var songs = _services.GetRequiredService<PamelloSongRepository>();
			PamelloSong? song;

            foreach (SearchResult result in searchResponse.Items) {
                if (result?.Id?.VideoId is not null) {
					song = songs.GetByYoutubeId(result.Id.VideoId);

					if (song is not null) {
						searchResult.PamelloSongs.Add(song);
					}
					else {
						searchResult.YoutubeVideos.Add(new YoutubeSearchVideoInfo() {
							Id = result.Id.VideoId,
							Name = result.Snippet.Title,
							Author = result.Snippet.ChannelTitle,
							ThumbnailUrl = result.Snippet.Thumbnails.Default__?.Url ?? "",
                        });
					}
				}
			}

			return searchResult;
        }
    }
}

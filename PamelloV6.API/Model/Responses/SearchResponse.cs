namespace PamelloV6.API.Model.Responses
{
    public class SearchResponse<T>
    {
        public int PagesCount { get; set; }
        public List<T> Results { get; set; }
    }
}

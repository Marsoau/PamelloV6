using AngleSharp.Html;
using PamelloV6.API.Model;
using PamelloV6.API.Model.Responses;
using PamelloV6.API.Services;
using PamelloV6.DAL;

namespace PamelloV6.API.Repositories
{
	public abstract class PamelloRepository<T> where T : PamelloEntity
	{
		protected readonly IServiceProvider _services;

        protected readonly DatabaseContext _database;
        protected readonly PamelloEventsService _events;

        protected readonly List<T> _list;

		public int Size {
			get => _list.Count;
		}

		public PamelloRepository(IServiceProvider services) {
			_services = services;

            _database = services.GetRequiredService<DatabaseContext>();
            _events = services.GetRequiredService<PamelloEventsService>();

            _list = new List<T>();
		}

		public SearchResponse<T> Search(int page, int count, string? query) {
			List<T> list;

			if (query is null) list = _list;
			else list = _list.Where(item => item.Name.ToLower().Contains(query.ToLower())).ToList();

            var start = page * count;
			if (start >= list.Count) {
				return new SearchResponse<T>() {
                    PagesCount = 0,
                    Results = []
                };
            }
			if (start + count > list.Count) {
				count = list.Count - start;
            }

			return new SearchResponse<T>() {
				PagesCount = list.Count / count + (list.Count % count != 0 ? 1 : 0),
				Results = list.GetRange(start, count)
            };
		}
		public T GetRequired(int id)
			=> Get(id) ?? throw new Exception($"Cant find required {typeof(T)} with id {id}");
		public abstract T? Get(int id);
		public abstract void Delete(int id);
	}
}

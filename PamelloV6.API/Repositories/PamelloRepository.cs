﻿using AngleSharp.Html;
using PamelloV6.API.Model;
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

		public PamelloRepository(IServiceProvider services) {
			_services = services;

            _database = services.GetRequiredService<DatabaseContext>();
            _events = services.GetRequiredService<PamelloEventsService>();

            _list = new List<T>();
		}

		public List<T> GetAll(int page, int count) {
			if (page * count > _list.Count) {
				return [];
			}
			if (page * count + count > _list.Count) {
				count = _list.Count - page * count;
			}
			return _list.GetRange(page * count, count);
		}
		public T GetRequired(int id)
			=> Get(id) ?? throw new Exception($"Cant find required {typeof(T)} with id {id}");
		public abstract T? Get(int id);
		public abstract void Delete(int id);
	}
}

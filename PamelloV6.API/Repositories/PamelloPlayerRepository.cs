using PamelloV6.API.Model.Audio;
using PamelloV6.API.Model.Events;

namespace PamelloV6.API.Repositories
{
    public class PamelloPlayerRepository : PamelloRepository<PamelloPlayer>
	{
		public PamelloPlayerRepository(IServiceProvider services) : base(services) {

		}

		public PamelloPlayer Create(string name = "player") {
			string oldName = name;
			for (int i = 1; _list.FirstOrDefault(player => player.Name == name) is not null; i++) {
				name = $"{oldName}-{i}";
			}

			var player = new PamelloPlayer(name, _services);
			_list.Add(player);

			_events.SendToAll(PamelloEvent.PlayerCreated(player.Id));

			return player;
		}
		public override PamelloPlayer? Get(int id) {
			return _list.FirstOrDefault(player => player.Id == id);
		}
		public PamelloPlayer? GetByName(string name) {
			return _list.FirstOrDefault(player => player.Name == name);
		}
		public override void Delete(int id) {
            var player = GetRequired(id);

			player.Delete();
            _list.Remove(player);

			_events.SendToAll(PamelloEvent.PlayerDeleted(player.Id));
        }
	}
}

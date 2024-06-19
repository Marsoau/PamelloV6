using PamelloV6.API.Model.Audio;

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

			return player;
		}
		public override PamelloPlayer? Get(int id) {
			return _list.FirstOrDefault(player => player.Id == id);
		}
		public PamelloPlayer? GetByName(string name) {
			return _list.FirstOrDefault(player => player.Name == name);
		}
		public override void Delete(int id) => throw new NotImplementedException();
	}
}

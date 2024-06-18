using PamelloV6.API.Model.Audio;

namespace PamelloV6.API.Repositories
{
    public class PamelloPlayerRepository : PamelloRepository<PamelloPlayer>
	{
		public PamelloPlayerRepository(IServiceProvider services) : base(services) {

		}

		public override void Delete(int id) => throw new NotImplementedException();
		public override PamelloPlayer? Get(int id) => throw new NotImplementedException();
	}
}

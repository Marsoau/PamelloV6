namespace PamelloV6.API.Model.Audio
{
    public class PamelloPlayer : PamelloEntity
    {
        private static int nextId = 1;
        public override int Id { get; }

        public PamelloPlayer(IServiceProvider services) : base(services)
        {
            Id = nextId++;
        }

        public override object GetDTO() => throw new NotImplementedException();
    }
}

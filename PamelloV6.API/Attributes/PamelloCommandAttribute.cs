namespace PamelloV6.API.Attributes
{
    public class PamelloCommandAttribute : Attribute
    {
        public string? Name { get; }

        public PamelloCommandAttribute() {
            Name = null;
        }
        public PamelloCommandAttribute(string name) {
            Name = name;
        }
    }
}

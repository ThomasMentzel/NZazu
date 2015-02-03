using NZazu.Contracts;

namespace Sample
{
    public interface INZazuSample
    {
        string Name { get; }
        string Description { get; }
        FormDefinition FormDefinition { get; }
    }
}
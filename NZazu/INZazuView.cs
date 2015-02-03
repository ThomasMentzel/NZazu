using NZazu.Contracts;

namespace NZazu
{
    public interface INZazuView
    {
        FormDefinition FormDefinition { get; set; }

        INZazuFieldFactory FieldFactory { get; set; }
        INZazuLayoutStrategy LayoutStrategy { get; set; }

        INZazuField GetField(string fieldKey);
    }
}
using NZazu.Contracts;

namespace Sample
{
    public class NZazuSample : INZazuSample
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public FormDefinition FormDefinition { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
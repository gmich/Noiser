namespace Noiser.Sources
{
    internal class SequentialSourceSelector : ISourceSelector
    {
        private readonly INoiseSource[] sources;
        private int lastItem;

        public SequentialSourceSelector(INoiseSource[] sources)
        {
            Argument.Require.That(() => sources.Length > 0);
            this.sources = sources;
            lastItem = 0;
        }

        private int GetNextItemIndex => (lastItem == sources.Length - 1) ? 0 : lastItem + 1;

        public INoiseSource Next
        {
            get
            {
                var source =  sources[lastItem];
                lastItem = GetNextItemIndex;
                return source;
            }
        }
    }
}

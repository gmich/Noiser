using NLog;
using System;

namespace Noiser.Sources
{
    internal class RandomSourceSelector : ISourceSelector
    {
        private readonly INoiseSource[] sources;
        private readonly Random rand = new Random();

        public RandomSourceSelector(INoiseSource[] sources)
        {
            Argument.Require.That(() => sources.Length > 0);
            this.sources = sources;
        }

        public INoiseSource Next
        {
            get
            {
                return sources[rand.Next(sources.Length-1)];
            }
        }

    }
}

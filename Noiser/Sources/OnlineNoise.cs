using System;
using System.Diagnostics;

namespace Noiser.Sources
{
    internal class OnlineNoise : INoiseSource
    {
        private readonly Uri uri;

        public OnlineNoise(Uri uri)
        {
            DebugArgument.Require.NotNull(() => uri);
            this.uri = uri;
        }
    

    }
}


namespace Noiser.Sources
{
    internal class OnlineNoise : INoiseSource
    {
        private readonly string url;

        public OnlineNoise(string url)
        {
            this.url = url;
        }
    

    }
}

namespace Noiser.Sources
{
    internal interface ISourceFactory
    {
        Result<INoiseSource> GetSource(string id, string source);
    }
}
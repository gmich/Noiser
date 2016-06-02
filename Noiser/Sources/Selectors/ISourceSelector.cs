namespace Noiser.Sources
{
    internal interface ISourceSelector
    {
        INoiseSource Next { get; }
    }
}

namespace Noiser.Configuration
{
    internal interface IConfigurationLoader
    {
        Result<NoiserConfig> Load();
    }
}

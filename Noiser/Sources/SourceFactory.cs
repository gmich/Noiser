using NLog;
using System;
using System.IO;

namespace Noiser.Sources
{
    internal class SourceFactory : ISourceFactory
    {
        private static Logger logger = LogManager.GetLogger("SourceFactory");

        public Result<INoiseSource> GetSource(string source)
        {
            INoiseSource noiseSource = null;
            TryGetUri(source)
            .OnSuccess(uri =>
            {
                logger.Info($"Configured new online noise: {uri}");
                noiseSource = new OnlineNoise(uri);
            })
            .OnFailure(res =>
                TryGetPath(source)
                .OnSuccess(path =>
                {
                    logger.Info($"Configured new local noise: {path}");
                    noiseSource = new LocalNoise(path);
                })
            )
            .OnFailure(res =>
                logger.Info($"Unable to configure noise source: {source}. {res.ErrorMessage}"));

            return Result.FailIfNull(noiseSource);

        }
        private Result<Uri> TryGetUri(string uri)
        {
            Uri uriResult;
            return (Uri.TryCreate(uri, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)) ?
                Result.Ok(uriResult) : Result.FailWith<Uri>(State.Error, "Invalid URI");
        }

        private Result<string> TryGetPath(string path)
        {
            try
            {
                return Result.Ok(Path.GetFullPath(path));
            }
            catch (Exception ex)
            {
                return Result.FailWith<string>(State.Error, $"{path} is an invalid path. {ex.Message}");
            }
        }
    }
}

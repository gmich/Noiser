using NLog;
using System;
using System.IO;

namespace Noiser.Sources
{
    internal class SourceFactory : ISourceFactory
    {
        private static Logger logger = LogManager.GetLogger("SourceFactory");

        public Result<INoiseSource> GetSource(string id, string source)
        {
            INoiseSource noiseSource = null;

            TryGetUri(source)
            .OnSuccess(uri =>
            {
                logger.Info($"Configured new online noise: {id} , {uri}");
                noiseSource = new OnlineNoise(id, uri);
            })
            .OnFailure(res =>
                TryGetPath(source)
                .OnSuccess(path =>
                {
                    logger.Info($"Configured new local noise: {id} , {path}");
                    noiseSource = new LocalNoise(id, path);
                })
            );

            if (noiseSource == null)
            {
                var errorMessage = $"Unable to configure noise source: {id} ,{source}";
                logger.Error(errorMessage);
                return Result.FailWith<INoiseSource>(State.Error, errorMessage);
            }

            return Result.Ok(noiseSource);

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

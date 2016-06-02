using NLog;
using System;
using System.Diagnostics;

namespace Noiser.Sources
{
    internal class OnlineNoise : INoiseSource
    {
        private readonly Uri uri;
        private Process process;
        private static Logger logger = LogManager.GetLogger("OnlineNoise");
        private readonly string id;

        public OnlineNoise(string id, Uri uri)
        {
            DebugArgument.Require.NotNull(() => uri);
            this.uri = uri;
            this.id = id;
        }

        public override string ToString()
        {
            return $"{id} at {uri.AbsoluteUri}";
        }

        public Result<IDisposable> Create()
        {
            try
            {
                process = Process.Start(uri.AbsoluteUri);
                return Result.Ok(new DelegateDisposable(() =>
                {
                    try
                    {
                        process.Kill();
                        process.Dispose();
                    }
                    catch (Exception ex)
                    {
                        logger.Warn($"Unable to dispose {ToString()}. {ex.Message}");
                    }
                    process = null;
                }) as IDisposable);
            }
            catch (Exception ex)
            {
                return Result.FailWith<IDisposable>(State.Error, ex.Message).Log(logger.Error);
            }
        }

        public Result Validate()
        {
            try
            {
                Create()
                .OnSuccess(res => res.Dispose())
                .OnSuccess(res => logger.Info($"Successfuly validated: {ToString()}"));
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.FailWith(State.Error, $"{ToString()} doesn't validate. {ex.Message}").Log(logger.Error);
            }
        }

    }
}

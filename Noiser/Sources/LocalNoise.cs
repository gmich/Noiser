using NLog;
using System;
using System.Diagnostics;

namespace Noiser.Sources
{
    internal class LocalNoise : INoiseSource
    {
        private readonly string path;
        private readonly string id;
        private Process process;
        private static Logger logger = LogManager.GetLogger("LocalNoise");

        public LocalNoise(string id, string path)
        {
            DebugArgument.Require.NotNullOrEmpty(() => path);
            this.path = path;
            this.id = id;
        }

        public override string ToString()
        {
            return $"{id} at {path}" ;
        }

        public Result<IDisposable> Create()
        {
            try
            {
                process = Process.Start("wmplayer.exe", path);
                return Result.Ok(new DelegateDisposable(() =>
                {
                    try
                    {
                        process.Kill();
                        process.Close();
                        process.Dispose();
                    }
                    catch (Exception ex)
                    {
                        logger.Warn($"Unable to dispose: {ToString()}. {ex.Message}");
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

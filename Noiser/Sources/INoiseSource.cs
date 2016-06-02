using System;

namespace Noiser.Sources
{
    internal interface INoiseSource
    {
        Result<IDisposable> Create();
        Result Validate();
    }
}

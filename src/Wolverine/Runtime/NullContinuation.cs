using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Wolverine.Runtime;

internal class NullContinuation : IContinuation
{
    public static readonly NullContinuation Instance = new();

    public ValueTask ExecuteAsync(IEnvelopeLifecycle lifecycle, IWolverineRuntime runtime, DateTimeOffset now,
        Activity? activity)
    {
        return ValueTask.CompletedTask;
    }
}
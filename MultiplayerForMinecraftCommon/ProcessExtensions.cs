using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace MultiplayerForMinecraftCommon
{
    public static class ProcessExtensions
    {
        public static Task<int> WaitForExitAsync(this Process process, CancellationToken cancellationToken = default)
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();

            process.EnableRaisingEvents = true;
            cancellationToken.Register(() => tcs.SetCanceled());
            process.Exited += (s, e) => tcs.SetResult(process.ExitCode);

            return process.HasExited ? Task.FromResult(process.ExitCode) : tcs.Task;
        }
    }
}

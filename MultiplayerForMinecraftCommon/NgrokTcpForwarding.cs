using System;
using System.Diagnostics;

namespace MultiplayerForMinecraftCommon
{
    public class NgrokTcpForwarding : IDisposable
    {
        public Process Process { get; }
        public DateTime StartTime { get; } = DateTime.Now;
        public bool IsOpened => !Process.HasExited;


        public event Action<NgrokTcpForwarding, TimeSpan> FastClosed;
        public event Action<NgrokTcpForwarding> Closed;


        public NgrokTcpForwarding(Process process)
        {
            Process = process;
        }


        public void StartListening()
        {
            Process.Exited += OnProcessExited;

            if (Process.HasExited)
                OnProcessExited(Process, EventArgs.Empty);
        }

        public void Dispose()
        {
            Process.Kill();
            Process.Dispose();
        }


        private void OnProcessExited(object sender, EventArgs e)
        {
            var runnedTime = DateTime.Now - StartTime;
            Closed?.Invoke(this);

            if (runnedTime < MinRunningTime)
                FastClosed?.Invoke(this, runnedTime);
        }


        private readonly TimeSpan MinRunningTime = TimeSpan.FromSeconds(15);
    }
}

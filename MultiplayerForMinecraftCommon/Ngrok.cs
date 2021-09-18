using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace MultiplayerForMinecraftCommon
{
    public class Ngrok
    {
        public const string NgrokName = @"ngrok.exe";


        public Ngrok()
        {
            File.WriteAllBytes(@"ngrok.exe", GetNgrok());
        }


        private static byte[] GetNgrok()
        {
            using (var mem = new MemoryStream(Resources.ngrokC))
            using (var memOut = new MemoryStream())
            using (var gzip = new GZipStream(mem, CompressionMode.Decompress))
            {
                gzip.CopyTo(memOut);
                return memOut.ToArray();
            }
        }

        public static bool IsOpened()
        {
            return Process.GetProcessesByName("ngrok").Length != 0;
        }

        public async Task Authentificate(string token, CancellationToken cancellationToken = default)
        {
            Process process = Process.Start(new ProcessStartInfo
            {
                FileName = "ngrok.exe",
                Arguments = $"authtoken {token}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            });

            await process.WaitForExitAsync(cancellationToken);
            var output = await process.StandardOutput.ReadToEndAsync();

            if (!output.StartsWith("Authtoken saved to configuration file: "))
                throw new NgrokException(output);
        }

        public Task<NgrokTcpForwarding> ForwardTcp(ushort port, NgrokRegion region = null)
        {
            try
            {
                return Task.FromResult(ForwardTcpSync(port, region));
            }
            catch (Exception ex)
            {
                return Task.FromException<NgrokTcpForwarding>(ex);
            }
        }

        private NgrokTcpForwarding ForwardTcpSync(ushort port, NgrokRegion region = null)
        {
            string arguments = $"tcp {port}";
            if (region != null)
                arguments += $" --region={region.TwoCharName}";

            Process process = Process.Start(new ProcessStartInfo
            {
                FileName = "ngrok.exe",
                Arguments = arguments
            });
            process.EnableRaisingEvents = true;

            return new NgrokTcpForwarding(process);
        }
    }
}

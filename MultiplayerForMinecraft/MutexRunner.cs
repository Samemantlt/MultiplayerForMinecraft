using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using MultiplayerForMinecraftCommon;

namespace MultiplayerForMinecraft
{
    public static class MutexRunner
    {
        private const string AppMutexName = @"MultiplayerForMinecraft";

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        public static bool IsRunning()
        {
            return false;

            // Mutex mutex = new Mutex(false, AppMutexName, out bool createdNew);
            // return createdNew;
        }

        public static void CloseIfRunning()
        {
            if (IsRunning())
            {
                if (MessageBox.Show("Программа уже запущена. Вы хотите продолжить?", "MultiplayerForMinecraft",
                    MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    Process current = Process.GetCurrentProcess();

                    foreach (Process process in Process.GetProcessesByName(current.ProcessName))
                    {
                        if (process.Id != current.Id)
                        {
                            SetForegroundWindow(process.MainWindowHandle);
                            break;
                        }
                    }
                    Environment.Exit(0);
                }
            }

            if (Ngrok.IsOpened())
                MessageBox.Show("У вас уже запущен ngrok. Рекомендуется его закрыть", "MultiplayerForMinecraft",  MessageBoxButton.OK);
        }
    }
}

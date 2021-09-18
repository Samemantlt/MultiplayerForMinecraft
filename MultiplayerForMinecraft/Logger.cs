using System;
using System.IO;
using System.Windows;
using System.Runtime.CompilerServices;

namespace MultiplayerForMinecraft
{
    public class Logger
    {
        private const string LogFolder = "Logs\\";


        public Logger()
        {
            Directory.CreateDirectory(LogFolder);
        }


        public void LogException(string message, Exception ex, [CallerLineNumber] int callerLineNumber = 0, [CallerFilePath] string callerFile = null)
        {
            MessageBox.Show($"Ошибка: {message} | {ex.Message}");
            var content = $"Строка: {callerLineNumber}\r\nФайл: {callerFile}\r\nСообщение: {message}\r\nИсключение: {ex}";

            File.WriteAllText(System.IO.Path.Combine(LogFolder, $"{Guid.NewGuid()}.log"), content);
        }
    }
}

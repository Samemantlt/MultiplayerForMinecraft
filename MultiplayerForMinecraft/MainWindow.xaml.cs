using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MultiplayerForMinecraft.Properties;
using MultiplayerForMinecraftCommon;

namespace MultiplayerForMinecraft
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DataContext LocalContext { get; } = new DataContext();
        public Settings Settings => LocalContext.Settings;
        public Ngrok Ngrok { get; set; }


        public MainWindow()
        {
            MutexRunner.CloseIfRunning();

            DataContext = LocalContext;
            InitializeComponent();
            Settings.PropertyChanged += OnSettingsChanged;
            try
            {
                Settings.Port = MinecraftListenerDetector.GetMinecraftPort();
            }
            catch { }
            Ngrok = new Ngrok();
            OnSettingsChanged(this, new PropertyChangedEventArgs(nameof(Settings.AuthToken)));
        }


        private async void OnStartButtonClick(object sender, RoutedEventArgs e)
        {
            await Ngrok.Authentificate(Settings.AuthToken);
            _ngrokForwarding = await Ngrok.ForwardTcp(Settings.Port, NgrokRegion.Europe);
            _ngrokForwarding.FastClosed += OnNgrokFastClosed;
            _ngrokForwarding.Closed += OnNgrokClosed;
            _ngrokForwarding.StartListening();
            tbNgrokOutput.Text = $"Успешно запущено!";
        }

        private void OnNgrokClosed(NgrokTcpForwarding obj)
        {
            Dispatcher.Invoke(() =>
            {
                tbNgrokOutput.Text = $"Ngrok остановлен";
            });
        }

        private void OnNgrokFastClosed(NgrokTcpForwarding obj, TimeSpan runnedTime)
        {
            Dispatcher.Invoke(() =>
            {
                tbNgrokOutput.Text = $"Ngrok работал лишь {runnedTime.TotalSeconds} секунд, возможно произошла ошибка. Проверьте правильность введённого токена и порта, а также подключение к интернету";
            });
        }

        private void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Settings.AuthToken) && lblTokenOutput != null)
            {
                if (Settings.AuthToken.Length != 43)
                    lblTokenOutput.Content = $"Обычно длина токена равна 43 символам";
                else
                    lblTokenOutput.Content = "";
            }
        }

        private void OnClosing(object sender1, CancelEventArgs e1)
        {
            Application.Current.DispatcherUnhandledException += (object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) =>
            {
                e.Handled = true;
            };
            _ngrokForwarding?.Dispose();
        }

        private NgrokTcpForwarding _ngrokForwarding;
    }
}

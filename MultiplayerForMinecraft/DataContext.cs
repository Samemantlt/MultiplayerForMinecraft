using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MultiplayerForMinecraft
{
    public class DataContext
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Settings Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                OnPropertyChanged();
            }
        }


        public DataContext()
        {
            Settings = _settingsSerializer.Load();
            Settings.PropertyChanged += OnSettingsPropertyChanged;
        }


        private void OnSettingsPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                _settingsSerializer.Save(Settings);
            }
            catch (Exception ex)
            {
                _logger.LogException("Ошибка сохранения", ex);
            }
        }

        private void OnPropertyChanged([CallerMemberName] string callermember = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callermember));
        }


        private Logger _logger = new Logger();
        private SettingsSerializer _settingsSerializer = new SettingsSerializer();
        private Settings _settings;
    }
}

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace MultiplayerForMinecraft
{
    public class Settings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string AuthToken
        {
            get => _authToken;
            set
            {
                _authToken = value;
                OnPropertyChanged();
            }
        }
        public ushort Port
        {
            get => _port;
            set
            {
                _port = value;
                OnPropertyChanged();
            }
        }


        private void OnPropertyChanged([CallerMemberName] string caller = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        }


        private ushort _port;
        private string _authToken = "";
    }
}

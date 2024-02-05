using Avalonia.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace SIT.Manager.Avalonia.Views
{
    public partial class ServerPage : UserControl
    {
        private readonly ConsoleContent dc = new();
        public ServerPage()
        {
            InitializeComponent();
            DataContext = dc;
        }
    }

    public class ConsoleContent : INotifyPropertyChanged
    {
        ObservableCollection<string> consoleOutput = [];
        public ObservableCollection<string> ConsoleOutput
        {
            get
            {
                return consoleOutput;
            }
            set
            {
                consoleOutput = value;
                OnPropertyChanged(nameof(ConsoleOutput));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
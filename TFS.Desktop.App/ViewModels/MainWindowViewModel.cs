namespace TFS.Desktop.App.ViewModels {
    using System.Threading.Tasks;
    using Utils;

    public class MainWindowViewModel : NotifyPropertyChanged {
        private string greeting;

        public string Greeting {
            get { return $"Hello {greeting}!"; }
            set {
                if (greeting != value) {
                    greeting = value;
                    OnPropertyChanged("Greeting");
                }
            }
        }

        public static async Task<MainWindowViewModel> GetViewModel() {
            return new MainWindowViewModel();
        }
    }
}
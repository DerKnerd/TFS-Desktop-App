namespace TFS.Client.ViewModels {

    using MyToolkit.Model;
    using Windows.Storage;

    public class StartupViewModel : ObservableObject {
        private string collection;
        private string password;
        private string url;
        private string username;

        public StartupViewModel() {
            this.Collection = ApplicationData.Current.LocalSettings.Values["SelectedCollection"]?.ToString();
            if (this.Collection == null) {
                this.Collection = "DefaultCollection";
            }
            this.Url = ApplicationData.Current.LocalSettings.Values["TFSUrl"]?.ToString();
        }

        public string Collection {
            get { return collection; }
            set {
                if (collection != value) {
                    collection = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Password {
            get { return password; }
            set {
                if (password != value) {
                    password = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Url {
            get { return url; }
            set {
                if (url != value) {
                    url = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Username {
            get { return username; }
            set {
                if (username != value) {
                    username = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
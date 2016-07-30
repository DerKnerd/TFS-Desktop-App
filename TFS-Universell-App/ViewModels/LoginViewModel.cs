namespace TFS.Client.ViewModels {
    using Utils;

    public class LoginViewModel : NotifyPropertyChanged {
        private string username;

        public string Username {
            get { return username; }
            set {
                if (username != value) {
                    username = value;
                    OnPropertyChanged();
                }
            }
        }

        private string password;

        public string Password {
            get { return password; }
            set {
                if (password != value) {
                    password = value;
                    OnPropertyChanged();
                }
            }
        }

        private string url;

        public string Url {
            get { return url; }
            set {
                if (url != value) {
                    url = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
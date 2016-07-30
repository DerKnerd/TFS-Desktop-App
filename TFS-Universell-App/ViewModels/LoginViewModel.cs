namespace TFS.Client.ViewModels {

    using MyToolkit.Model;

    public class LoginViewModel : ObservableObject {
        private string username;

        public string Username {
            get { return username; }
            set {
                if (username != value) {
                    username = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string password;

        public string Password {
            get { return password; }
            set {
                if (password != value) {
                    password = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string url;

        public string Url {
            get { return url; }
            set {
                if (url != value) {
                    url = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
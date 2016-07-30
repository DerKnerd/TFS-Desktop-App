namespace TFS.Client.ViewModels {

    using MyToolkit.Model;
    using Windows.Storage;

    public class SelectTeamProjectCollectionViewModel : ObservableObject {

        public SelectTeamProjectCollectionViewModel() {
            this.Collection = ApplicationData.Current.LocalSettings.Values["SelectedCollection"]?.ToString();
        }

        private string collection;

        public string Collection {
            get { return collection; }
            set {
                if (collection != value) {
                    collection = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
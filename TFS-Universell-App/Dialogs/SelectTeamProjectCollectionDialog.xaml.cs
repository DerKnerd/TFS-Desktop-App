// Die Elementvorlage "Inhaltsdialog" ist unter http://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace TFS.Client.Dialogs {
    using System;
    using ViewModels;
    using Windows.Storage;
    using Windows.UI.Popups;
    using Windows.UI.Xaml.Controls;

    public sealed partial class SelectTeamProjectCollectionDialog {

        public SelectTeamProjectCollectionDialog() {
            this.InitializeComponent();
            this.ViewModel = new SelectTeamProjectCollectionViewModel();
        }

        public SelectTeamProjectCollectionViewModel ViewModel { get; set; }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
            try {
                var dfd = args.GetDeferral();
                ApplicationData.Current.LocalSettings.Values["SelectedCollection"] = ViewModel.Collection.Name;
                dfd.Complete();
            } catch (Exception ex) {
                var diag = new MessageDialog($"Cannot select collection{Environment.NewLine}{ex.Message}");
                await diag.ShowAsync();
                await this.ShowAsync();
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
            this.Hide();
        }
    }
}
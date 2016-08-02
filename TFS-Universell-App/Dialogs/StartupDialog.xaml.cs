// Die Elementvorlage "Inhaltsdialog" ist unter http://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace TFS.Client.Dialogs {

    using System;
    using ViewModels;
    using Windows.Security.Credentials;
    using Windows.Storage;
    using Windows.UI.Popups;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public sealed partial class StartupDialog {

        public StartupDialog() {
            this.InitializeComponent();
            this.ViewModel = new StartupViewModel();
        }

        public StartupViewModel ViewModel { get; set; }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
            try {
                var dfd = args.GetDeferral();
                ApplicationData.Current.LocalSettings.Values["TFSUrl"] = ViewModel.Url;
                ApplicationData.Current.LocalSettings.Values["SelectedCollection"] = ViewModel.Collection;

                var vault = new PasswordVault();
                try {
                    var credentialList = vault.FindAllByResource("TFS.Client");
                    if (credentialList.Count != 0) {
                        foreach (var item in credentialList) {
                            vault.Remove(item);
                        }
                    }
                } catch {
                }
                vault.Add(new PasswordCredential("TFS.Client", ViewModel.Username, ViewModel.Password));

                dfd.Complete();
            } catch (Exception ex) {
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
            Application.Current.Exit();
        }
    }
}
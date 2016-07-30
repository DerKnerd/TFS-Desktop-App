// Die Elementvorlage "Inhaltsdialog" ist unter http://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace TFS.Client.Dialogs {

    using System;
    using ViewModels;
    using Windows.Security.Credentials;
    using Windows.Storage;
    using Windows.UI.Popups;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public sealed partial class LoginDialog : ContentDialog {

        public LoginDialog() {
            this.InitializeComponent();
            this.ViewModel = new LoginViewModel();
        }

        public LoginViewModel ViewModel { get; set; }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
            try {
                var dfd = args.GetDeferral();
                var passwordVault = new PasswordVault();
                var password = new PasswordCredential("TFS.Client", ViewModel.Username, ViewModel.Password);
                ApplicationData.Current.LocalSettings.Values["TFSUrl"] = ViewModel.Url;
                passwordVault.Add(password);
                dfd.Complete();
            } catch (Exception ex) {
                var diag = new MessageDialog($"Cannot login{Environment.NewLine}{ex.Message}");
                await diag.ShowAsync();
                await this.ShowAsync();
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
            Application.Current.Exit();
        }
    }
}
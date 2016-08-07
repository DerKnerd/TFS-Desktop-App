// Die Elementvorlage "Inhaltsdialog" ist unter http://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace TFS.Client.Dialogs {

    using API.Models;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public sealed partial class WorkItemDetailsDialog {

        // Using a DependencyProperty as the backing store for Model.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModelProperty = DependencyProperty.Register("Model", typeof(WorkItem), typeof(WorkItemDetailsDialog), new PropertyMetadata(null));

        public WorkItemDetailsDialog(WorkItem item) {
            this.InitializeComponent();
            this.Model = item;
        }
        
        public WorkItem Model {
            get { return (WorkItem)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
            await Model.SaveChanges(App.TfsClient);

        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
        }
    }
}
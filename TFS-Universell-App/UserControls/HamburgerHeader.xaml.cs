using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace TFS.Client.UserControls {

    public sealed partial class HamburgerHeader : UserControl {

        public HamburgerHeader(string title = "") {
            this.InitializeComponent();
            if (!string.IsNullOrEmpty(title))
                this.HamburgerHeaderTitle.Text = $"{this.HamburgerHeaderTitle.Text} - {title}";
        }
    }
}
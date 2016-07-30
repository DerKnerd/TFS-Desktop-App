namespace TFS.Client.UserControls {

    using MyToolkit.Controls;
    using Windows.UI.Xaml.Shapes;
    public class PlaceholderHamburgerItem : HamburgerItem {
        public PlaceholderHamburgerItem() {
            this.CanBeSelected = false;
            this.IsEnabled = false;
        }
    }
}
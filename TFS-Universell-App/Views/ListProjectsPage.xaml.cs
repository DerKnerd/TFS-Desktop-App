// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.
namespace TFS.Client.Views {

    using API.Models;
    using MyToolkit.Paging;
    using System.Linq;
    using Windows.UI.Xaml;

    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class ListProjectsPage {

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(TeamProjectCollection), typeof(ListProjectsPage), new PropertyMetadata(default(TeamProjectCollection)));

        public ListProjectsPage() {
            this.InitializeComponent();
        }

        public TeamProjectCollection ViewModel {
            get { return (TeamProjectCollection)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        protected async override void OnNavigatedTo(MtNavigationEventArgs args) {
            base.OnNavigatedTo(args);
            var items = await App.TfsClient?.GetProjects(0, 255);
            Grid.Items.Clear();
            if (args.Parameter == null) {
                foreach (var item in items.Value.OrderBy(o => o.Name)) {
                    Grid.Items.Add(item);
                }
            } else {
                (Application.Current as App).UpdateHamburgerTitle(args.Parameter.ToString());
                foreach (var item in items.Value.Where(w => {
                    var res = w.Name.ToLower().Contains(args.Parameter.ToString());
                    if (w.Description != null)
                        res |= w.Description.ToLower().Contains(args.Parameter.ToString());
                    return res;
                }).OrderBy(o => o.Name)) {
                    Grid.Items.Add(item);
                }
            }
        }

        private void Grid_ItemClick(object sender, Windows.UI.Xaml.Controls.ItemClickEventArgs e) {
            var project = (e.ClickedItem as TeamProject);
            App.GetFrame().NavigateAsync(typeof(ProjectOverviewPage), project.ID);
        }
    }
}
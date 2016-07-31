// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace TFS.Client.Views {

    using MyToolkit.Paging;
    using System;
    using Windows.UI.Xaml;

    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class ProjectOverviewPage : MtPage {

        public ProjectOverviewPage() {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(MtNavigationEventArgs args) {
            base.OnNavigatedTo(args);
            (Application.Current as App).SetTopItemsForProject((Guid)args.Parameter);
            var project = await App.TfsClient.GetProject((Guid)args.Parameter);
            (Application.Current as App).UpdateHamburgerTitle(project.Name);
            await App.GetFrame().NavigateAsync(typeof(MyTasksPage));
        }
    }
}
// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace TFS.Client.Views {

    using MyToolkit.Paging;
    using System;
    using Windows.Storage;
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
            var projectId = default(Guid);
            if (args.Parameter is Guid) {
                projectId = (Guid)args.Parameter;
            } else {
                projectId = App.SelectedProject;
            } 
            (Application.Current as App).SetTopItemsForProject(projectId);
            var project = await App.TfsClient.GetProject(projectId);
            (Application.Current as App).UpdateHamburgerTitle(project.Name);
            ApplicationData.Current.LocalSettings.Values["SelectedProject"] = projectId;
            await App.GetFrame().NavigateAsync(typeof(MyTasksPage));
        }
    }
}
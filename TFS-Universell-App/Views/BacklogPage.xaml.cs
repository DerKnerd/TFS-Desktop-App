// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace TFS.Client.Views {

    using System;
    using API.Models;
    using MyToolkit.Paging;
    using ViewModels;
    using Windows.UI.Xaml;
    using Dialogs;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class BacklogPage {

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(TasksViewModel), typeof(BacklogPage), new PropertyMetadata(null));

        public BacklogPage() {
            this.InitializeComponent();
        }

        public TasksViewModel ViewModel {
            get { return (TasksViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        protected async override void OnNavigatedTo(MtNavigationEventArgs args) {
            base.OnNavigatedTo(args);
            this.ViewModel = await TasksViewModel.GetBacklogTasksViewModel();
        }

        private async void DataGrid_SelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e) {
            foreach (var item in e.AddedItems) {
                await (item as WorkItem).GetChildren(App.TfsClient);
            }
        }

        private async void GridView_ItemClick(object sender, Windows.UI.Xaml.Controls.ItemClickEventArgs e) {
            var diag = new WorkItemDetailsDialog(e.ClickedItem as WorkItem);
            diag.Closed += async (s, a) => {
                if (a.Result == ContentDialogResult.Primary)
                    this.ViewModel = await TasksViewModel.GetBacklogTasksViewModel();
            };
            diag.MaxWidth = this.ActualWidth;
            diag.MinWidth = this.ActualWidth;
            diag.MaxHeight = this.ActualHeight;
            diag.MinHeight = this.ActualHeight;
            await diag.ShowAsync();
        }
    }
}
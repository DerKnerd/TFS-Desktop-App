// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace TFS.Client.Views {

    using Dialogs;
    using MyToolkit.Paging;
    using System;
    using API.Models;
    using ViewModels;
    using Windows.ApplicationModel.DataTransfer;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;

    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class CurrentSprint {

        public CurrentSprint() {
            this.InitializeComponent();
        }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(SprintViewModel), typeof(CurrentSprint), new PropertyMetadata(null));

        public SprintViewModel ViewModel {
            get { return (SprintViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        protected async override void OnNavigatedTo(MtNavigationEventArgs args) {
            base.OnNavigatedTo(args);
            this.ViewModel = await SprintViewModel.GetCurrentSprint();
        }

        private async void DataGrid_SelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e) {
            foreach (var item in e.AddedItems) {
                await (DataGrid.SelectedItem as SprintItemViewModel).LoadCurrentSprint();
            }
        }

        private void GridView_DragOver(object sender, DragEventArgs e) {
            e.AcceptedOperation = DataPackageOperation.Move;
        }

        private void GridView_DragItemsStarting(object sender, Windows.UI.Xaml.Controls.DragItemsStartingEventArgs e) {
            e.Data.SetText((e.Items[0] as WorkItem).ID.ToString());
            e.Data.RequestedOperation = DataPackageOperation.Move;
        }

        private void GridView_DragEnter(object sender, DragEventArgs e) {
            (sender as GridView).Background = Application.Current.Resources["SystemControlHighlightListAccentLowBrush"] as SolidColorBrush;
        }

        private void GridView_DragLeave(object sender, DragEventArgs e) {
            (sender as GridView).Background = null;
        }

        private async void GridView_Drop(object sender, DragEventArgs e) {
            (sender as GridView).Background = null;
            await (DataGrid.SelectedItem as SprintItemViewModel).LoadCurrentSprint();
        }

        private async void GridView_ItemClick(object sender, Windows.UI.Xaml.Controls.ItemClickEventArgs e) {
            var diag = new WorkItemDetailsDialog(e.ClickedItem as WorkItem);
            diag.Closed += async (s, a) => {
                if (a.Result == ContentDialogResult.Primary)
                    await (DataGrid.SelectedItem as SprintItemViewModel).LoadCurrentSprint();
            };
            diag.MaxWidth = this.ActualWidth;
            diag.MinWidth = this.ActualWidth;
            diag.MaxHeight = this.ActualHeight;
            diag.MinHeight = this.ActualHeight;
            await diag.ShowAsync();
        }
    }
}
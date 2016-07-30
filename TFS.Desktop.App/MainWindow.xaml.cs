using Microsoft.TeamFoundation.Client;
using System.Windows;
using TFS.Desktop.App.ViewModels;

namespace TFS.Desktop.App {

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow {

        public MainWindow() {
            InitializeComponent();
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e) {
            this.DataContext = await MainWindowViewModel.GetViewModel();
        }
    }
}
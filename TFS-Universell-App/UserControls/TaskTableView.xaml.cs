using TFS.Client.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace TFS.Client.UserControls {

    public sealed partial class TaskTableView : UserControl {

        public TaskTableView() {
            this.InitializeComponent();
        }

        public TasksViewModel ViewModel {
            get { return (TasksViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(TasksViewModel), typeof(TaskTableView), new PropertyMetadata(null));
    }
}
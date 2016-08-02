namespace TFS.Client.ViewModels {

    using API.Models;
    using MyToolkit.Model;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class TasksViewModel : ObservableObject {

        public async static Task<TasksViewModel> GetMyTasksViewModel() {
            var tasks = await App.TfsClient.GetMyWorkItems(App.SelectedProject);
            return new TasksViewModel { Items = tasks.Value.ToArray() };
        }

        public async static Task<TasksViewModel> GetBacklogTasksViewModel() {
            var tasks = await App.TfsClient.GetBacklogWorkItems(App.SelectedProject);
            return new TasksViewModel { Items = tasks.Value.ToArray() };
        }

        public async static Task<TasksViewModel> ExecuteStoredQuery(Guid query) {
            var tasks = await App.TfsClient.ExecuteStoredQuery(App.SelectedProject, query);
            return new TasksViewModel { Items = tasks.Value.ToArray() };
        }

        private WorkItem[] items;

        public WorkItem[] Items {
            get { return items; }
            set {
                if (items != value) {
                    items = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
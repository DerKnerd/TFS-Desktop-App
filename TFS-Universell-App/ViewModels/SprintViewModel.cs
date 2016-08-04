namespace TFS.Client.ViewModels {

    using API.Models;
    using MyToolkit.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    public class SprintItemViewModel : WorkItem {

        public static SprintItemViewModel FromWorkItem(WorkItem item) {
            var result = new SprintItemViewModel();
            var typeofSprintItem = typeof(SprintItemViewModel);
            var typeofWorkItem = typeof(WorkItem);
            foreach (var prop in typeofWorkItem.GetProperties()) {
                typeofSprintItem.GetProperty(prop.Name).SetValue(result, prop.GetValue(item));
            }
            return result;
        }

        private WorkItem[] active;

        private WorkItem[] done;

        private WorkItem[] planning;

        public WorkItem[] Active {
            get { return active; }
            set {
                if (active != value) {
                    active = value;
                    RaisePropertyChanged();
                }
            }
        }

        public WorkItem[] Done {
            get { return done; }
            set {
                if (done != value) {
                    done = value;
                    RaisePropertyChanged();
                }
            }
        }

        public WorkItem[] Planning {
            get { return planning; }
            set {
                if (planning != value) {
                    planning = value;
                    RaisePropertyChanged();
                }
            }
        }

        public async Task LoadCurrentSprint() {
            this.Active = (await App.TfsClient.GetCurrentSprintActive(App.SelectedProject, this.ID)).Value.ToArray();
            this.Done = (await App.TfsClient.GetCurrentSprintDone(App.SelectedProject, this.ID)).Value.ToArray();
            this.Planning = (await App.TfsClient.GetCurrentSprintPlanning(App.SelectedProject, this.ID)).Value.ToArray();
        }
    }

    public class SprintViewModel : ObservableObject {

        public static async Task<SprintViewModel> GetCurrentSprint() {
            var tasks = await App.TfsClient.GetCurrentSprint(App.SelectedProject);
            var items = new List<SprintItemViewModel>();
            foreach (var item in tasks) {
                items.Add(SprintItemViewModel.FromWorkItem(item));
            }
            return new SprintViewModel { Items = items.ToArray() };
        }

        private SprintItemViewModel[] items;

        public SprintItemViewModel[] Items {
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
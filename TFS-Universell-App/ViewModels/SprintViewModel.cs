namespace TFS.Client.ViewModels {

    using API.Models;
    using MyToolkit.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class SprintItemViewModel : WorkItem {

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
            this.Active = (await App.TfsClient.GetCurrentSprintActive(App.SelectedProject)).Value.ToArray();
            this.Done = (await App.TfsClient.GetCurrentSprintDone(App.SelectedProject)).Value.ToArray();
            this.Planning = (await App.TfsClient.GetCurrentSprintPlanning(App.SelectedProject)).Value.ToArray();
        }
    }

    public class SprintViewModel : ObservableObject {

        public static async Task<SprintViewModel> GetCurrentSprint() {
            var tasks = await App.TfsClient.GetCurrentSprint(App.SelectedProject);
            var items = new List<SprintItemViewModel>();
            foreach (var item in tasks) {
                items.Add(Convert.ChangeType(item, typeof(SprintItemViewModel)) as SprintItemViewModel);
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
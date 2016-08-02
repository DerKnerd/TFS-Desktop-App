namespace TFS.API.Models {

    using MyToolkit.Model;
    using Newtonsoft.Json;

    public class WorkItem : ObservableObject {
        private WorkItemFields fields;
        private int id;

        private int revision;

        private string url;

        [JsonProperty("fields")]
        public WorkItemFields Fields {
            get { return fields; }
            set {
                if (fields != value) {
                    fields = value;
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("id")]
        public int ID {
            get { return id; }
            set {
                if (id != value) {
                    id = value;
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("rev")]
        public int Revision {
            get { return revision; }
            set {
                if (revision != value) {
                    revision = value;
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("url")]
        public string Url {
            get { return url; }
            set {
                if (url != value) {
                    url = value;
                    RaisePropertyChanged();
                }
            }
        }
    }

    public class WorkItemCollection : BaseCollection<WorkItem> { }

    [JsonObject]
    public class WorkItemFields : ObservableObject {
        private string description;
        private float effort;
        private string iterationPath;

        private string state;

        private int stateCode;
        private string title;

        private string workItemType;

        [JsonProperty("System.Description")]
        public string Description {
            get { return description; }
            set {
                if (description != value) {
                    description = value;
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("Microsoft.VSTS.Scheduling.Effort")]
        public float Effort {
            get { return effort; }
            set {
                if (effort != value) {
                    effort = value;
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("System.IterationPath")]
        public string IterationPath {
            get { return iterationPath; }
            set {
                if (iterationPath != value) {
                    iterationPath = value;
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("System.State")]
        public string State {
            get { return state; }
            set {
                if (state != value) {
                    state = value;
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("Microsoft.VSTS.Common.StateCode")]
        public int StateCode {
            get { return stateCode; }
            set {
                if (stateCode != value) {
                    stateCode = value;
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("System.Title")]
        public string Title {
            get { return title; }
            set {
                if (title != value) {
                    title = value;
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("System.WorkItemType")]
        public string WorkItemType {
            get { return workItemType; }
            set {
                if (workItemType != value) {
                    workItemType = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
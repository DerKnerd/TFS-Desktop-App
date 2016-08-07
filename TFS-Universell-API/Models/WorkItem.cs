namespace TFS.API.Models {

    using MyToolkit.Model;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Threading.Tasks;

    public class WorkItem : ObservableObject {
        private WorkItemCollection children;

        private WorkItemFields fields;

        private int id;

        private int revision;

        private string url;

        public WorkItem() {
            Children = new WorkItemCollection();
        }

        [JsonIgnore]
        public WorkItemCollection Children {
            get { return children; }
            set {
                if (children != value) {
                    children = value;
                    RaisePropertyChanged();
                }
            }
        }

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

        public async Task GetChildren(TfsClient client) {
            Children = await client.GetChildren(this.ID);
        }

        public async Task SaveChanges(TfsClient client) {
            await client.UpdateWorkItem(this);
        }
    }

    public class WorkItemCollection : BaseCollection<WorkItem> { }

    [JsonObject]
    public class WorkItemFields : ObservableObject {

        private string activity;

        private string areaPath;

        private string assignedTo;

        private string blocked;

        private string description;

        private float effort;

        private string iterationPath;

        private int priority;

        private string reason;

        private double remainingWork;

        private string state;

        private int stateCode;

        private string title;

        private string workItemType;

        public WorkItemFields() {
            OriginalValue = new Dictionary<string, object>();
        }

        [JsonProperty("Microsoft.VSTS.Common.Activity")]
        public string Activity {
            get { return activity; }
            set {
                if (activity != value) {
                    activity = value;
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("System.AreaPath")]
        public string AreaPath {
            get { return areaPath; }
            set {
                if (areaPath != value) {
                    areaPath = value;
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("System.AssignedTo")]
        public string AssignedTo {
            get { return assignedTo; }
            set {
                if (assignedTo != value) {
                    assignedTo = value;
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("Microsoft.VSTS.CMMI.Blocked")]
        public string Blocked {
            get { return blocked; }
            set {
                if (blocked != value) {
                    blocked = value;
                    RaisePropertyChanged();
                }
            }
        }

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

        [JsonIgnore]
        public Dictionary<string, object> OriginalValue { get; set; }

        [JsonProperty("Microsoft.VSTS.Common.Priority")]
        public int Priority {
            get { return priority; }
            set {
                if (priority != value) {
                    priority = value;
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("System.Reason")]
        public string Reason {
            get { return reason; }
            set {
                if (reason != value) {
                    reason = value;
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("Microsoft.VSTS.Scheduling.RemainingWork")]
        public double RemainingWork {
            get { return remainingWork; }
            set {
                if (remainingWork != value) {
                    remainingWork = value;
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

        protected override void RaisePropertyChanged(PropertyChangedEventArgs args) {
            base.RaisePropertyChanged(args);
            if (!OriginalValue.ContainsKey(args.PropertyName)) {
                OriginalValue.Add(args.PropertyName, this.GetType().GetProperty(args.PropertyName).GetValue(this));
            }
        }
    }
}
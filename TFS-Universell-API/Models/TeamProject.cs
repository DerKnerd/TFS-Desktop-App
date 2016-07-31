namespace TFS.API.Models {

    using MyToolkit.Model;
    using Newtonsoft.Json;
    using System;

    public class TeamProject : ObservableObject {
        private string description;
        private Guid id;

        private string name;

        private int revision;
        private ProjectState state;

        private string url;

        [JsonProperty("description")]
        public string Description {
            get { return description; }
            set {
                if (description != value) {
                    description = value;
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("id")]
        public Guid ID {
            get { return id; }
            set {
                if (id != value) {
                    id = value;
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("name")]
        public string Name {
            get { return name; }
            set {
                if (name != value) {
                    name = value;
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("revision")]
        public int Revision {
            get { return revision; }
            set {
                if (revision != value) {
                    revision = value;
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("state")]
        public ProjectState State {
            get { return state; }
            set {
                if (state != value) {
                    state = value;
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

    public class TeamProjectCollection : BaseCollection<TeamProject> {

    }
}
using MyToolkit.Model;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;

namespace TFS.API.Models {

    public class ModifiedBy : ObservableObject {
        private Guid id;

        private string name;

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
    }

    public class Query : ObservableObject {

        private ObservableCollection<Query> children;

        private ModifiedBy createdBy;

        private DateTime createdDate;

        private bool hasChildren;

        private Guid id;

        private bool isFolder;

        private bool isPublic;

        private ModifiedBy lastModifiedBy;

        private DateTime lastModifiedDate;

        private string name;

        private string path;

        [JsonProperty("children")]
        public ObservableCollection<Query> Children {
            get { return children; }
            set {
                if (children != value) {
                    children = value;
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("createdBy")]
        public ModifiedBy CreatedBy {
            get { return createdBy; }
            set {
                if (createdBy != value) {
                    createdBy = value;
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("createdDate")]
        public DateTime CreatedDate {
            get { return createdDate; }
            set {
                if (createdDate != value) {
                    createdDate = value;
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("hasChildren")]
        public bool HasChildren {
            get { return hasChildren; }
            set {
                if (hasChildren != value) {
                    hasChildren = value;
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

        [JsonProperty("isFolder")]
        public bool IsFolder {
            get { return isFolder; }
            set {
                if (isFolder != value) {
                    isFolder = value;
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("isPublic")]
        public bool IsPublic {
            get { return isPublic; }
            set {
                if (isPublic != value) {
                    isPublic = value;
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("lastModifiedBy")]
        public ModifiedBy LastModifiedBy {
            get { return lastModifiedBy; }
            set {
                if (lastModifiedBy != value) {
                    lastModifiedBy = value;
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("lastModifiedDate")]
        public DateTime LastModifiedDate {
            get { return lastModifiedDate; }
            set {
                if (lastModifiedDate != value) {
                    lastModifiedDate = value;
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

        [JsonProperty("path")]
        public string Path {
            get { return path; }
            set {
                if (path != value) {
                    path = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<Query> GetQueries() {
            return flatten(this);
        }

        private ObservableCollection<Query> flatten(Query query) {
            var result = new ObservableCollection<Query>();
            if (query.HasChildren) {
                foreach (var item in query.Children) {
                    if (item.HasChildren) {
                        foreach (var elem in flatten(item)) {
                            result.Add(elem);
                        }
                    } else {
                        if (!item.IsFolder)
                            result.Add(item);
                    }
                }
            } else {
                if (!query.IsFolder)
                    result.Add(this);
            }
            return result;
        }
    }

    public class QueryCollection : BaseCollection<Query> { }
}
namespace TFS.API.Models {

    using MyToolkit.Model;
    using System;

    public class Queue : ObservableObject {
        private Guid groupScopeId;
        private int id;
        private string name;
        private QueuePool pool;

        private Guid projectId;

        public Guid GroupScopeID {
            get { return groupScopeId; }
            set {
                if (groupScopeId != value) {
                    groupScopeId = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int ID {
            get { return id; }
            set {
                if (id != value) {
                    id = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Name {
            get { return name; }
            set {
                if (name != value) {
                    name = value;
                    RaisePropertyChanged();
                }
            }
        }

        public QueuePool Pool {
            get { return pool; }
            set {
                if (pool != value) {
                    pool = value;
                    RaisePropertyChanged();
                }
            }
        }

        public Guid ProjectID {
            get { return projectId; }
            set {
                if (projectId != value) {
                    projectId = value;
                    RaisePropertyChanged();
                }
            }
        }
    }

    public class QueuePool : ObservableObject {
        private int id;

        private string name;

        private Guid scope;

        public int ID {
            get { return id; }
            set {
                if (id != value) {
                    id = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Name {
            get { return name; }
            set {
                if (name != value) {
                    name = value;
                    RaisePropertyChanged();
                }
            }
        }

        public Guid Scope {
            get { return scope; }
            set {
                if (scope != value) {
                    scope = value;
                    RaisePropertyChanged();
                }
            }
        }
    }

    public class QueueCollection : BaseCollection<Queue> { }
}
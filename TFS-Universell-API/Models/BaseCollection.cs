namespace TFS.API.Models {

    using MyToolkit.Model;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    [JsonObject]
    public class BaseCollection<TModel> : ObservableObject, IEnumerable<TModel> {
        public BaseCollection() {
            Value = new ObservableCollection<TModel>();
        }

        private int count;
        private ObservableCollection<TModel> value;

        [JsonProperty("count")]
        public int Count {
            get { return count; }
            set {
                if (count != value) {
                    count = value;
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("value")]
        public ObservableCollection<TModel> Value {
            get { return value; }
            set {
                if (this.value != value) {
                    this.value = value;
                    RaisePropertyChanged();
                }
            }
        }
        public IEnumerator<TModel> GetEnumerator() {
            return Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return Value.GetEnumerator();
        }
    }
}
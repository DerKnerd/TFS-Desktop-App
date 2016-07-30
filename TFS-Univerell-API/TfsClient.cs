using Windows.Storage;
using Windows.Web.Http;

namespace TFS.API {

    public class TfsClient {

        public TfsClient(string collection) {
            Collection = collection;
            baseUri = $"{ApplicationData.Current.LocalSettings.Values["TFSUrl"]}/{Collection}/_apis/";
        }

        public string Collection { get; set; }
        private string baseUri;
    }
}
namespace TFS.API {

    using Models;
    using Newtonsoft.Json;
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Windows.Storage;
    using Windows.Web.Http;
    using Windows.Web.Http.Headers;

    public class TfsClient {

        public TfsClient(string username, string password) {
            ReInitialize(username, password);
        }

        public void ReInitialize(string username, string password) {
            Collection = ApplicationData.Current.LocalSettings.Values["SelectedCollection"].ToString();
            baseUri = $"{ApplicationData.Current.LocalSettings.Values["TFSUrl"]}/{Collection}/";
            client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new HttpCredentialsHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}")));
        }

        private string getUrl(string path, string apiversion) {
            var url = $"{baseUri}_apis/{path}";
            if (url.IndexOf("?") > -1) {
                url = $"{url}&api-version={apiversion}";
            } else {
                url = $"{url}?api-version={apiversion}";
            }
            return url;
        }

        private string getUrl(string path, string apiversion, Guid project) {
            var url = $"{baseUri}{project}/_apis/{path}";
            if (url.IndexOf("?") > -1) {
                url = $"{url}&api-version={apiversion}";
            } else {
                url = $"{url}?api-version={apiversion}";
            }
            return url;
        }

        public string Collection { get; set; }

        private string baseUri;
        private HttpClient client;

        public async Task<TeamProjectCollection> GetProjects(int skip = 0, int top = 100) {
            var data = await client.GetStringAsync(new Uri($"{getUrl("projects", "1.0")}&$skip={skip}&top={top}", UriKind.RelativeOrAbsolute));
            var result = JsonConvert.DeserializeObject<TeamProjectCollection>(data);
            return result;
        }
        
        public async Task<TeamProject> GetProject(Guid project) {
            var data = await client.GetStringAsync(new Uri($"{getUrl($"projects/{project}", "1.0")}", UriKind.RelativeOrAbsolute));
            var result = JsonConvert.DeserializeObject<TeamProject>(data);
            return result;
        }

        public async Task<QueueCollection> GetQueues(Guid project) {
            var data = await client.GetStringAsync(new Uri($"{getUrl($"distributedtask/queues", "3.0-preview.1", project)}", UriKind.RelativeOrAbsolute));
            var result = JsonConvert.DeserializeObject<QueueCollection>(data);
            return result;
        }

        public async Task<QueryCollection> GetQueries(Guid project) {
            var data = await client.GetStringAsync(new Uri($"{getUrl($"wit/queries?$depth=2", "2.2", project)}", UriKind.RelativeOrAbsolute));
            var result = JsonConvert.DeserializeObject<QueryCollection>(data);
            return result;
        }
    }
}
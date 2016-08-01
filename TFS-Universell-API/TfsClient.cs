namespace TFS.API {

    using Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Windows.Storage;
    using Windows.Web.Http;
    using Windows.Web.Http.Headers;

    public class TfsClient {

        public TfsClient() {
            ReInitialize();
        }

        public void ReInitialize() {
            Collection = ApplicationData.Current.LocalSettings.Values["SelectedCollection"]?.ToString();
            baseUri = $"{ApplicationData.Current.LocalSettings.Values["TFSUrl"]}/{Collection}/";
            client = new HttpClient();
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
            var data = await client.GetStringAsync(new Uri($"{getUrl("projects", "1.0")}&$skip={skip}&$top={top}", UriKind.RelativeOrAbsolute));
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
            var data = await client.GetStringAsync(new Uri($"{getUrl($"wit/queries?$depth=2", "2.0", project)}", UriKind.RelativeOrAbsolute));
            var result = JsonConvert.DeserializeObject<QueryCollection>(data);
            return result;
        }

        public async Task<WorkItemCollection> GetWorkItemsByIds(List<int> ids) {
            var data = await client.GetStringAsync(new Uri($"{getUrl($"wit/WorkItems?ids={string.Join(",", ids)}&fields=System.Description,Microsoft.VSTS.Scheduling.Effort,System.IterationPath,System.State,System.Title,System.WorkItemType,Microsoft.VSTS.Common.StateCode", "1.0")}", UriKind.RelativeOrAbsolute));
            var result = JsonConvert.DeserializeObject<WorkItemCollection>(data);
            return result;
        }

        public async Task<WorkItemCollection> GetWorkItemsByQuery(Guid project, string wiql) {
            var postresult = await client.PostAsync(new Uri($"{getUrl("wit/wiql", "1.0", project)}", UriKind.RelativeOrAbsolute), new HttpStringContent(JsonConvert.SerializeObject(new { query = wiql }), Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json"));
            var data = JsonConvert.DeserializeObject<JObject>(await postresult.Content.ReadAsStringAsync());
            var relations = data["workItemRelations"];
            var ids = new List<int>();
            foreach (var item in relations) {
                ids.Add(item["target"]["id"].Value<int>());
            }
            return await GetWorkItemsByIds(ids);
        }

        public async Task<WorkItemCollection> GetMyWorkItems(Guid project) {
            return await GetWorkItemsByQuery(project, $"SELECT [System.Id] FROM WorkItemLinks WHERE Source.[System.AssignedTo] = @me AND Source.[System.TeamProject] = @project AND Source.[System.State] <> 'Fertig' AND Source.[System.State] <> 'Geschlossen'");
        }
    }
}
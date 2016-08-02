namespace TFS.API {

    using Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using Windows.Storage;

    public class TfsClient {

        private string baseUri;

        private string password;

        private string username;

        public TfsClient(string username, string password) {
            ReInitialize(username, password);
        }

        public string Collection { get; set; }

        internal async Task<WorkItemCollection> GetChildren(int id) {
            var workitem = await GetAsync<JObject>(new Uri($"{getUrl($"wit/WorkItems/{id}?$expand=relations", "1.0")}", UriKind.RelativeOrAbsolute));
            var relations = workitem["relations"];
            var res = new WorkItemCollection();
            foreach (var item in relations) {
                if (item["rel"]?.Value<string>() == "System.LinkTypes.Hierarchy-Forward") {
                    res.Value.Add(await getWorkItem(item["url"].Value<string>()));
                }
            }
            res.Count = res.Value.Count;
            return res;
        }

        private async Task<WorkItem> getWorkItem(string path) {
            return await GetAsync<WorkItem>(new Uri($"{path}?fields=System.Description,Microsoft.VSTS.Scheduling.Effort,System.IterationPath,System.State,System.Title,System.WorkItemType,Microsoft.VSTS.Common.StateCode", UriKind.RelativeOrAbsolute));
        }

        public async Task<WorkItem> GetWorkItem(int id) {
            return await GetAsync<WorkItem>(new Uri($"{getUrl($"wit/WorkItems/{id}&fields=System.Description,Microsoft.VSTS.Scheduling.Effort,System.IterationPath,System.State,System.Title,System.WorkItemType,Microsoft.VSTS.Common.StateCode", "1.0")}", UriKind.RelativeOrAbsolute));
        }

        public async Task<WorkItemCollection> GetBacklogWorkItems(Guid project) {
            return await GetWorkItemsByQuery(project, $"SELECT [System.Id] FROM WorkItemLinks WHERE Source.[System.TeamProject] = @project AND Source.[System.State] <> 'Fertig' AND Source.[System.State] <> 'Geschlossen' AND Source.[System.State] <> 'Entfernt' AND Source.[System.WorkItemType] = 'Product Backlog Item'");
        }

        public async Task<WorkItemCollection> GetMyWorkItems(Guid project) {
            return await GetWorkItemsByQuery(project, $"SELECT [System.Id] FROM WorkItemLinks WHERE Source.[System.AssignedTo] = @me AND Source.[System.TeamProject] = @project AND Source.[System.State] <> 'Fertig' AND Source.[System.State] <> 'Entfernt' AND Source.[System.State] <> 'Geschlossen'");
        }

        public async Task<TeamProject> GetProject(Guid project) {
            return await GetAsync<TeamProject>(new Uri($"{getUrl($"projects/{project}", "1.0")}", UriKind.RelativeOrAbsolute));
        }

        public async Task<TeamProjectCollection> GetProjects(int skip = 0, int top = 100) {
            return await GetAsync<TeamProjectCollection>(new Uri($"{getUrl("projects", "1.0")}&$skip={skip}&$top={top}", UriKind.RelativeOrAbsolute));
        }

        public async Task<QueryCollection> GetQueries(Guid project) {
            return await GetAsync<QueryCollection>(new Uri($"{getUrl($"wit/queries?$depth=2", "2.0", project)}", UriKind.RelativeOrAbsolute));
        }

        public async Task<WorkItemCollection> ExecuteStoredQuery(Guid project, Guid query) {
            var data = await GetAsync<JObject>(new Uri($"{getUrl($"wit/wiql/{query}", "1.0", project)}", UriKind.RelativeOrAbsolute));
            var workItems = data["workItems"];
            var ids = new List<int>();
            foreach (var item in workItems) {
                ids.Add(item["id"].Value<int>());
            }
            return await GetWorkItemsByIds(ids);
        }

        public async Task<WorkItemCollection> GetWorkItemsByIds(List<int> ids) {
            return await GetAsync<WorkItemCollection>(new Uri($"{getUrl($"wit/WorkItems?ids={string.Join(",", ids)}&fields=System.Description,Microsoft.VSTS.Scheduling.Effort,System.IterationPath,System.State,System.Title,System.WorkItemType,Microsoft.VSTS.Common.StateCode", "1.0")}", UriKind.RelativeOrAbsolute));
        }

        public async Task<WorkItemCollection> GetWorkItemsByQuery(Guid project, string wiql) {
            var data = await PostAsync<JObject>(new Uri($"{getUrl("wit/wiql", "1.0", project)}", UriKind.RelativeOrAbsolute), new { query = wiql });
            var relations = data["workItemRelations"];
            var ids = new List<int>();
            foreach (var item in relations) {
                if (item["source"] == null) {
                    ids.Add(item["target"]["id"].Value<int>());
                }
            }
            return await GetWorkItemsByIds(ids);
        }

        public void ReInitialize(string username, string password) {
            Collection = ApplicationData.Current.LocalSettings.Values["SelectedCollection"]?.ToString();
            baseUri = $"{ApplicationData.Current.LocalSettings.Values["TFSUrl"]}/{Collection}/";
            this.username = username;
            this.password = password;
        }
        internal async Task<TModel> GetAsync<TModel>(Uri uri) {
            var req = WebRequest.CreateHttp(uri);
            req.Method = "GET";
            req.Credentials = new NetworkCredential(username, password);
            var res = await req.GetResponseAsync();
            using (var sr = new StreamReader(res.GetResponseStream())) {
                return JsonConvert.DeserializeObject<TModel>(await sr.ReadToEndAsync());
            }
        }

        internal async Task<TModel> PostAsync<TModel>(Uri uri, object body) {
            var req = WebRequest.CreateHttp(uri);
            req.Method = "POST";
            req.Credentials = new NetworkCredential(username, password);

            if (body != null) {
                req.ContentType = "application/json";
                using (var sw = new StreamWriter(await req.GetRequestStreamAsync())) {
                    await sw.WriteAsync(JsonConvert.SerializeObject(body));
                }
            }

            var res = await req.GetResponseAsync();
            using (var sr = new StreamReader(res.GetResponseStream())) {
                return JsonConvert.DeserializeObject<TModel>(await sr.ReadToEndAsync());
            }
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
    }
}
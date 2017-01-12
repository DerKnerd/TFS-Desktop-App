namespace TFS.API {

    using Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Threading.Tasks;
    using Windows.Storage;

    public class TfsClient {
        private string baseUri;

        private string password;

        private string username;

        private readonly string fields = "System.Description,Microsoft.VSTS.Scheduling.Effort,System.IterationPath,System.State,System.Title,System.WorkItemType,Microsoft.VSTS.Common.StateCode,System.AssignedTo,System.AreaPath,System.Reason,Microsoft.VSTS.Common.Priority,Microsoft.VSTS.Scheduling.RemainingWork,Microsoft.VSTS.Common.Activity,Microsoft.VSTS.CMMI.Blocked";

        public TfsClient(string username, string password) {
            ReInitialize(username, password);
        }

        public string Collection { get; set; }

        public async Task<WorkItemCollection> ExecuteStoredQuery(Guid project, Guid query) {
            var data = await GetAsync<JObject>(new Uri($"{getUrl($"wit/wiql/{query}", "1.0", project)}", UriKind.RelativeOrAbsolute));
            var workItems = data["workItems"];
            var ids = new List<int>();
            foreach (var item in workItems) {
                ids.Add(item["id"].Value<int>());
            }
            if (ids.Any())
                return await GetWorkItemsByIds(ids);
            else
                return new WorkItemCollection();
        }

        public async Task UpdateWorkItem(WorkItem workitem) {
            var items = new List<dynamic>();
            foreach (var item in workitem.Fields.GetType().GetProperties().Where(p => p.GetCustomAttribute<JsonIgnoreAttribute>() == null)) {
                var action = "replace";
                if (!workitem.Fields.OriginalValue.ContainsKey(item.Name) && item.GetValue(workitem.Fields) != null) {
                    action = "add";
                } else if (!workitem.Fields.OriginalValue.ContainsKey(item.Name) && item.GetValue(workitem.Fields) == null) {
                    continue;
                } else if (!workitem.Fields.OriginalValue.ContainsKey(item.Name) && item.GetValue(workitem.Fields) == workitem.Fields.OriginalValue[item.Name]) {
                    continue;
                }
                items.Add(new {
                    op = action,
                    value = item.GetValue(workitem.Fields),
                    path = item.GetCustomAttribute<JsonPropertyAttribute>().PropertyName
                });
            }
            await PatchAsync<dynamic>(new Uri($"{getUrl($"wit/workitems/{workitem.ID}", "1.0")}", UriKind.RelativeOrAbsolute), items);
        }

        public async Task<WorkItemCollection> GetBacklogWorkItems(Guid project) {
            return await GetWorkItemsByQuery(project, $"SELECT [System.Id] FROM WorkItemLinks WHERE Source.[System.TeamProject] = @project AND Source.[System.State] <> 'Fertig' AND Source.[System.State] <> 'Geschlossen' AND Source.[System.State] <> 'Entfernt' AND (Source.[System.WorkItemType] = 'Product Backlog Item' OR Source.[System.WorkItemType] = 'User Story')");
        }

        public async Task<WorkItemCollection> GetCurrentSprint(Guid project) {
            return await GetWorkItemsByQuery(project, $"SELECT [System.Id] FROM WorkItemLinks WHERE Source.[System.TeamProject] = @project AND Source.[System.State] <> 'Entfernt' AND Source.[System.IterationPath] = @CurrentIteration AND (Source.[System.WorkItemType] = 'Product Backlog Item' OR Source.[System.WorkItemType] = 'User Story')");
        }

        public async Task<WorkItemCollection> GetCurrentSprintActive(Guid project, int workitemId) {
            return await GetSprintWorkItemsByQuery(project, workitemId, "In Bearbeitung");
        }

        public async Task<WorkItemCollection> GetCurrentSprintDone(Guid project, int workitemId) {
            return await GetSprintWorkItemsByQuery(project, workitemId, "Fertig");
        }

        public async Task<WorkItemCollection> GetCurrentSprintPlanning(Guid project, int workitemId) {
            return await GetSprintWorkItemsByQuery(project, workitemId, "Aufgabenplanung");
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

        public async Task<WorkItem> GetWorkItem(int id) {
            return await getWorkItem($"{getUrl($"wit/WorkItems/{id}", "1.0")}");
        }

        public async Task<WorkItemCollection> GetWorkItemsByIds(List<int> ids) {
            return await GetAsync<WorkItemCollection>(new Uri($"{getUrl($"wit/WorkItems?ids={string.Join(",", ids)}&fields={fields}", "1.0")}", UriKind.RelativeOrAbsolute));
        }

        public async Task<WorkItemCollection> GetSprintWorkItemsByQuery(Guid project, int workitemId, string state) {
            var data = await GetAsync<JObject>(new Uri($"{getUrl($"wit/WorkItems/{workitemId}?$expand=relations", "1.0")}"));
            var relations = data["relations"];
            var result = new WorkItemCollection();
            foreach (var item in relations) {
                if (item["rel"]?.Value<string>() == "System.LinkTypes.Hierarchy-Forward") {
                    var workitem = await getWorkItem($"{item["url"].Value<string>()}");
                    if (workitem.Fields.State == state) {
                        result.Value.Add(workitem);
                    }
                }
            }
            return result;
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
            if (ids.Any())
                return await GetWorkItemsByIds(ids);
            else
                return new WorkItemCollection();
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

        internal async Task<TModel> PatchAsync<TModel>(Uri uri, object body) {
            var req = WebRequest.CreateHttp(uri);
            req.Method = "PATCH";
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

        private async Task<string> getCurrentSprintName(Guid project) {
            var data = await GetAsync<JObject>(new Uri($"{getUrl($"work/teamsettings/iterations?$timeframe=current", "2.0-preview", project)}", UriKind.RelativeOrAbsolute));
            var sprints = data["value"];
            foreach (var sprint in sprints) {
                return sprint["path"].Value<string>();
            }
            return string.Empty;
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

        private async Task<WorkItem> getWorkItem(string path) {
            if (path.Contains("?"))
                return await GetAsync<WorkItem>(new Uri($"{path}&fields={fields}", UriKind.RelativeOrAbsolute));
            else
                return await GetAsync<WorkItem>(new Uri($"{path}?fields={fields}", UriKind.RelativeOrAbsolute));
        }
    }
}
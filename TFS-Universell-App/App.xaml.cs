namespace TFS.Client {

    using API;
    using Dialogs;
    using Microsoft.TFS.ProjectCollection.WorkItemTracking;
    using MyToolkit.Controls;
    using MyToolkit.Paging;
    using MyToolkit.Paging.Animations;
    using MyToolkit.UI;
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using UserControls;
    using Views;
    using Windows.ApplicationModel.Activation;
    using Windows.ApplicationModel.Resources;
    using Windows.Security.Credentials;
    using Windows.Storage;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Stellt das anwendungsspezifische Verhalten bereit, um die Standardanwendungsklasse zu ergänzen.
    /// </summary>
    sealed partial class App : MtApplication {
        public static ClientService3SoapClient WorkItemTrackingClient;

        private HamburgerFrameBuilder _hamburgerFrameBuilder;

        /// <summary>
        /// Initialisiert das Singletonanwendungsobjekt.  Dies ist die erste Zeile von erstelltem Code
        /// und daher das logische Äquivalent von main() bzw. WinMain().
        /// </summary>
        public App() {
            this.InitializeComponent();
            TfsClient = new TfsClient(string.Empty, string.Empty);
        }

        public static Guid SelectedProject { get { return (Guid)ApplicationData.Current.LocalSettings.Values["SelectedProject"]; } }

        public static TfsClient TfsClient { get; internal set; }

        public override Type StartPageType {
            get {
                var containsSelectedProject = ApplicationData.Current.LocalSettings.Values.ContainsKey("SelectedProject");
                if (containsSelectedProject) {
                    return typeof(ProjectOverviewPage);
                }
                var containsSelectedCollection = ApplicationData.Current.LocalSettings.Values.ContainsKey("SelectedCollection");
                if (containsSelectedCollection) {
                    return typeof(ListProjectsPage);
                }
                return typeof(SettingsPage);
            }
        }

        public static PasswordCredential GetCredentialFromLocker() {
            var credential = default(PasswordCredential);
            var vault = new PasswordVault();
            var credentialList = vault.FindAllByResource("TFS.Client");
            if (credentialList.Count > 0) {
                if (credentialList.Count == 1) {
                    credential = credentialList[0];
                }
            }
            return credential;
        }

        public static MtFrame GetFrame() {
            return (Current as App).GetFrame(null);
        }

        public static string GetString(string key) {
            var resourceLoader = ResourceLoader.GetForCurrentView();
            return resourceLoader.GetString(key);
        }

        public void AddHamburgerBottomItem(HamburgerItem item) {
            _hamburgerFrameBuilder.Hamburger.BottomItems.Add(item);
        }

        public void AddHamburgerTopItem(HamburgerItem item) {
            _hamburgerFrameBuilder.Hamburger.TopItems.Add(item);
        }

        public void ClearHamburgerBottomItems() {
            _hamburgerFrameBuilder.Hamburger.BottomItems.Clear();
        }

        public void ClearHamburgerBottomItems(SearchHamburgerItem search) {
            ClearHamburgerBottomItems();
            AddHamburgerBottomItem(search);
        }

        public void ClearHamburgerTopItems() {
            _hamburgerFrameBuilder.Hamburger.TopItems.Clear();
        }

        public void ClearHamburgerTopItems(SearchHamburgerItem search) {
            ClearHamburgerTopItems();
            AddHamburgerTopItem(search);
        }

        public override FrameworkElement CreateWindowContentElement() {
            _hamburgerFrameBuilder = new HamburgerFrameBuilder();

            _hamburgerFrameBuilder.Hamburger.Header = new HamburgerHeader();
            _hamburgerFrameBuilder.Hamburger.TopItems = new ObservableCollection<HamburgerItem>();
            _hamburgerFrameBuilder.Hamburger.BottomItems = new ObservableCollection<HamburgerItem>();
            _hamburgerFrameBuilder.Frame.PageAnimation = new ScalePageTransition();

            SetHamburgerTopMenu();

            AddHamburgerBottomItem(getListProjectsHamburgerItem());
            AddHamburgerBottomItem(getSettingsHamburgerItem());

            return _hamburgerFrameBuilder.Hamburger;
        }

        public override MtFrame GetFrame(UIElement windowContentElement) {
            return _hamburgerFrameBuilder.Frame;
        }

        public override async Task OnInitializedAsync(MtFrame frame, ApplicationExecutionState args) {
            //await HideStatusBarAsync();
        }

        public void RemoveHamburgerBottomItem(Func<HamburgerItem, bool> predicate) {
            var item = _hamburgerFrameBuilder.Hamburger.BottomItems.SingleOrDefault(predicate);
            _hamburgerFrameBuilder.Hamburger.BottomItems.Remove(item);
        }

        public void RemoveHamburgerTopItem(Func<HamburgerItem, bool> predicate) {
            var item = _hamburgerFrameBuilder.Hamburger.TopItems.SingleOrDefault(predicate);
            _hamburgerFrameBuilder.Hamburger.TopItems.Remove(item);
        }

        public void SetHamburgerTopMenu() {
            AddHamburgerTopItem(getSearchHamburgerItem());
        }

        public async void SetTopItemsForProject(Guid project) {
            ClearHamburgerTopItems(getSearchTasksHamburgerItem());
            AddHamburgerTopItem(new PageHamburgerItem {
                Content = "Meine Tasks",
                ContentIcon = new FontAwesome.UWP.FontAwesome { Icon = FontAwesome.UWP.FontAwesomeIcon.List },
                Icon = new FontAwesome.UWP.FontAwesome { Icon = FontAwesome.UWP.FontAwesomeIcon.List },
                PageType = typeof(MyTasksPage)
            });
            AddHamburgerTopItem(new PageHamburgerItem {
                Content = "Backlog",
                ContentIcon = new FontAwesome.UWP.FontAwesome { Icon = FontAwesome.UWP.FontAwesomeIcon.StackOverflow },
                Icon = new FontAwesome.UWP.FontAwesome { Icon = FontAwesome.UWP.FontAwesomeIcon.StackOverflow },
                PageType = typeof(BacklogPage)
            });
            AddHamburgerTopItem(new PlaceholderHamburgerItem());
            AddHamburgerTopItem(new PageHamburgerItem {
                Content = "Aktueller Sprint",
                ContentIcon = new BitmapIcon { Height = 24, Width = 24, UriSource = new Uri("ms-appx:///Assets/run.png", UriKind.RelativeOrAbsolute) },
                Icon = new BitmapIcon { Height = 24, Width = 24, UriSource = new Uri("ms-appx:///Assets/run.png", UriKind.RelativeOrAbsolute) }
            });
            fillQueryHamburgerItems();
            AddHamburgerTopItem(new PlaceholderHamburgerItem());
            foreach (var item in await TfsClient.GetQueries(project)) {
                foreach (var query in item.GetQueries()) {
                    AddHamburgerTopItem(new PageHamburgerItem {
                        Content = query.Name,
                        ContentIcon = new FontAwesome.UWP.FontAwesome { Icon = FontAwesome.UWP.FontAwesomeIcon.PlayCircle },
                        Icon = new FontAwesome.UWP.FontAwesome { Icon = FontAwesome.UWP.FontAwesomeIcon.PlayCircle },
                        PageType = typeof(StoredQueryPage),
                        PageParameter = query.ID
                    });
                }
            }
        }

        public void UpdateHamburgerTitle(string title) {
            _hamburgerFrameBuilder.Hamburger.Header = new HamburgerHeader(title);
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args) {
            base.OnLaunched(args);
            ApplicationViewUtilities.ConnectRootElementSizeToVisibleBounds();
        }

        protected override async void OnWindowCreated(WindowCreatedEventArgs args) {
            base.OnWindowCreated(args);
            var loginCredential = default(PasswordCredential);
            var username = string.Empty;
            var password = string.Empty;
            var startupDialog = default(StartupDialog);
            try {
                loginCredential = GetCredentialFromLocker();

                if (!(loginCredential != null && ApplicationData.Current.LocalSettings.Values.ContainsKey("TFSUrl") && ApplicationData.Current.LocalSettings.Values.ContainsKey("SelectedCollection"))) {
                    startupDialog = new StartupDialog();
                    await startupDialog.ShowAsync();
                }
            } catch {
                startupDialog = new StartupDialog();
                await startupDialog.ShowAsync();
            }
            loginCredential = GetCredentialFromLocker();
            loginCredential.RetrievePassword();
            username = loginCredential.UserName;
            password = loginCredential.Password;

            TfsClient.ReInitialize(username, password);
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey("SelectedProject")) {
                var projects = await TfsClient?.GetProjects(0, 255);
                if (projects != null) {
                    foreach (var item in projects.OrderBy(o => o.Name)) {
                        AddHamburgerTopItem(new PageHamburgerItem {
                            PageParameter = item.ID,
                            PageType = typeof(ProjectOverviewPage),
                            Icon = new FontAwesome.UWP.FontAwesome { Icon = FontAwesome.UWP.FontAwesomeIcon.FolderOutlinepenOutline },
                            ContentIcon = new FontAwesome.UWP.FontAwesome { Icon = FontAwesome.UWP.FontAwesomeIcon.FolderOutlinepenOutline },
                            Content = item.Name
                        });
                    }
                }
                GetFrame(null).Navigate(typeof(ListProjectsPage));
            }
        }
        private void fillQueryHamburgerItems() {
            var project = ApplicationData.Current.LocalSettings.Values["SelectedProject"]?.ToString();
        }

        private PageHamburgerItem getListProjectsHamburgerItem() {
            var item = new PageHamburgerItem {
                Content = GetString("ListProjectsPageTitle/Text"),
                ContentIcon = new FontAwesome.UWP.FontAwesome { Icon = FontAwesome.UWP.FontAwesomeIcon.List },
                Icon = new FontAwesome.UWP.FontAwesome { Icon = FontAwesome.UWP.FontAwesomeIcon.List },
                PageType = typeof(ListProjectsPage)
            };
            return item;
        }

        private SearchHamburgerItem getSearchHamburgerItem() {
            var searchItem = new SearchHamburgerItem {
                PlaceholderText = GetString("Search")
            };
            searchItem.QuerySubmitted += async (sender, args) => {
                await _hamburgerFrameBuilder.Frame.NavigateToExistingOrNewPageAsync(typeof(ListProjectsPage), args.QueryText);
            };
            return searchItem;
        }

        private SearchHamburgerItem getSearchTasksHamburgerItem() {
            var searchItem = new SearchHamburgerItem {
                PlaceholderText = GetString("Search")
            };
            searchItem.QuerySubmitted += async (sender, args) => {
                await _hamburgerFrameBuilder.Frame.NavigateToExistingOrNewPageAsync(typeof(SearchTasksPage), args.QueryText);
            };
            return searchItem;
        }

        private PageHamburgerItem getSettingsHamburgerItem() {
            var item = new PageHamburgerItem {
                Content = GetString("SettingsPageTitle/Text"),
                ContentIcon = new FontAwesome.UWP.FontAwesome { Icon = FontAwesome.UWP.FontAwesomeIcon.Cog },
                Icon = new FontAwesome.UWP.FontAwesome { Icon = FontAwesome.UWP.FontAwesomeIcon.Cog },
                PageType = typeof(SettingsPage)
            };
            return item;
        }
    }
}
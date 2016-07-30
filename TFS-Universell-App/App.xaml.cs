namespace TFS.Client {

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

        public static string GetString(string key) {
            var resourceLoader = ResourceLoader.GetForCurrentView();
            return resourceLoader.GetString(key);
        }

        private HamburgerFrameBuilder _hamburgerFrameBuilder;

        public void AddHamburgerTopItem(HamburgerItem item) {
            _hamburgerFrameBuilder.Hamburger.TopItems.Add(item);
        }

        public void RemoveHamburgerTopItem(Func<HamburgerItem, bool> predicate) {
            var item = _hamburgerFrameBuilder.Hamburger.TopItems.SingleOrDefault(predicate);
            _hamburgerFrameBuilder.Hamburger.TopItems.Remove(item);
        }

        public void ClearHamburgerTopItems() {
            _hamburgerFrameBuilder.Hamburger.TopItems.Clear();
        }

        public void ClearHamburgerTopItems(SearchHamburgerItem search) {
            ClearHamburgerTopItems();
            AddHamburgerTopItem(search);
        }

        public void AddHamburgerBottomItem(HamburgerItem item) {
            _hamburgerFrameBuilder.Hamburger.BottomItems.Add(item);
        }

        public void RemoveHamburgerBottomItem(Func<HamburgerItem, bool> predicate) {
            var item = _hamburgerFrameBuilder.Hamburger.BottomItems.SingleOrDefault(predicate);
            _hamburgerFrameBuilder.Hamburger.BottomItems.Remove(item);
        }

        public void ClearHamburgerBottomItems() {
            _hamburgerFrameBuilder.Hamburger.BottomItems.Clear();
        }

        public void ClearHamburgerBottomItems(SearchHamburgerItem search) {
            ClearHamburgerBottomItems();
            AddHamburgerBottomItem(search);
        }

        /// <summary>
        /// Initialisiert das Singletonanwendungsobjekt.  Dies ist die erste Zeile von erstelltem Code
        /// und daher das logische Äquivalent von main() bzw. WinMain().
        /// </summary>
        public App() {
            this.InitializeComponent();
        }

        private SearchHamburgerItem getSearchHamburgerItem() {
            var searchItem = new SearchHamburgerItem {
                PlaceholderText = GetString("Search")
            };
            searchItem.QuerySubmitted += async (sender, args) => {
                await _hamburgerFrameBuilder.Frame.NavigateToExistingOrNewPageAsync(typeof(SearchTasksPage), args.QueryText);
            };
            return searchItem;
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

        private PageHamburgerItem getSettingsHamburgerItem() {
            var item = new PageHamburgerItem {
                Content = GetString("SettingsPageTitle/Text"),
                ContentIcon = new FontAwesome.UWP.FontAwesome { Icon = FontAwesome.UWP.FontAwesomeIcon.Cog },
                Icon = new FontAwesome.UWP.FontAwesome { Icon = FontAwesome.UWP.FontAwesomeIcon.Cog },
                PageType = typeof(SettingsPage)
            };
            return item;
        }

        private void fillQueryHamburgerItems() {
            var project = ApplicationData.Current.LocalSettings.Values["SelectedProject"]?.ToString();
        }

        public void SetHamburgerTopMenu() {
            AddHamburgerTopItem(getSearchHamburgerItem());
            AddHamburgerTopItem(new PageHamburgerItem {
                Content = "Projekt",
                ContentIcon = new FontAwesome.UWP.FontAwesome { Icon = FontAwesome.UWP.FontAwesomeIcon.University },
                Icon = new FontAwesome.UWP.FontAwesome { Icon = FontAwesome.UWP.FontAwesomeIcon.University },
                PageType = typeof(ProjectOverviewPage)
            });
            AddHamburgerTopItem(new PageHamburgerItem {
                Content = "Meine Tasks",
                ContentIcon = new FontAwesome.UWP.FontAwesome { Icon = FontAwesome.UWP.FontAwesomeIcon.List },
                Icon = new FontAwesome.UWP.FontAwesome { Icon = FontAwesome.UWP.FontAwesomeIcon.List }
            });
            AddHamburgerTopItem(new PageHamburgerItem {
                Content = "Backlog",
                ContentIcon = new FontAwesome.UWP.FontAwesome { Icon = FontAwesome.UWP.FontAwesomeIcon.StackOverflow },
                Icon = new FontAwesome.UWP.FontAwesome { Icon = FontAwesome.UWP.FontAwesomeIcon.StackOverflow },
            });
            AddHamburgerTopItem(new PlaceholderHamburgerItem());
            AddHamburgerTopItem(new PageHamburgerItem {
                Content = "Aktueller Sprint",
                ContentIcon = new BitmapIcon { Height = 24, Width = 24, UriSource = new Uri("ms-appx:///Assets/run.png", UriKind.RelativeOrAbsolute) },
                Icon = new BitmapIcon { Height = 24, Width = 24, UriSource = new Uri("ms-appx:///Assets/run.png", UriKind.RelativeOrAbsolute) }
            });
            fillQueryHamburgerItems();
            AddHamburgerTopItem(new PlaceholderHamburgerItem());
            AddHamburgerTopItem(new PageHamburgerItem {
                Content = "Neue Abfrage",
                ContentIcon = new FontAwesome.UWP.FontAwesome { Icon = FontAwesome.UWP.FontAwesomeIcon.Plus },
                Icon = new FontAwesome.UWP.FontAwesome { Icon = FontAwesome.UWP.FontAwesomeIcon.Plus }
            });
        }

        public override Type StartPageType {
            get {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("SelectedProject")) {
                    return typeof(ProjectOverviewPage);
                }
                return typeof(ListProjectsPage);
            }
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

        protected override async void OnWindowCreated(WindowCreatedEventArgs args) {
            base.OnWindowCreated(args);

            try {
                var loginCredential = GetCredentialFromLocker();

                if (loginCredential != null) {
                    loginCredential.RetrievePassword();
                    var usr = loginCredential.UserName;
                    var pwd = loginCredential.Password;
                    var url = ApplicationData.Current.LocalSettings.Values["TFSUrl"].ToString();
                } else {
                    var dialog = new LoginDialog();
                    await dialog.ShowAsync();
                }
            } catch {
                var dialog = new LoginDialog();
                await dialog.ShowAsync();
            }
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey("SelectedCollection")) { 
                var chooseTeamProject = new SelectTeamProjectCollectionDialog();
                await chooseTeamProject.ShowAsync();
            }
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args) {
            base.OnLaunched(args);
            ApplicationViewUtilities.ConnectRootElementSizeToVisibleBounds();
        }

        private static PasswordCredential GetCredentialFromLocker() {
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
    }
}
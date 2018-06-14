using System.Preferences;
using System.Windows;

using Azzembly.Patcher;

using SourceTree.Accounts;
using SourceTree.Analytics;
using SourceTree.Configuration;
using SourceTree.CustomActions;
using SourceTree.Diagnostics;
using SourceTree.Dvcss;
using SourceTree.Instances;
using SourceTree.Notifications;
using SourceTree.Repositories;
using SourceTree.Scheduler;
using SourceTree.Security;
using SourceTree.Utils;
using SourceTree.ViewModel.Repositories;
using SourceTree.Web;

namespace SourceTree.ViewModel
{
    [Patch(typeof(MainWindowViewModel))]
    public class MainWindowViewModelPatch : MainWindowViewModel
    {
        public MainWindowViewModelPatch(IRepositoryTabContainerViewModel repositoryTabContainerViewModel, ICustomActionsManager customActionsManager, IRepositoryManager repositoryManager, IAnalyticsDataManager analyticsDataManager, ITraceManager traceManager, IDispatcher dispatcher, IAccountManager accountManager, IFailureHandler failureHandler, IDvcsManager dvcsManager, IConfigurationManager configurationManager, IInstanceManager instanceManager, ISchedulerManager schedulerManager, IWebManager webManager, IRepositoryMonitorManager repositoryMonitorManager, ISshKeyManager sshKeyManager, INotificationsManager notificationsManager, IPreferencesManager preferencesManager, IRepoProcessFactory repoProcessFactory)
            : base(repositoryTabContainerViewModel, customActionsManager, repositoryManager, analyticsDataManager, traceManager, dispatcher, accountManager, failureHandler, dvcsManager, configurationManager, instanceManager, schedulerManager, webManager, repositoryMonitorManager, sshKeyManager, notificationsManager, preferencesManager, repoProcessFactory)
        {
        }

        private new void About()
        {
            //AboutWindowViewModel aboutWindowViewModel = new AboutWindowViewModel(this.NotificationsManager);
            //AboutWindow aboutWindow = new AboutWindow();
            //aboutWindow.DataContext = (object)aboutWindowViewModel;
            //aboutWindow.Owner = Application.Current.MainWindow;
            //aboutWindow.ShowDialog();

            MessageBox.Show(this.ConfigurationManager.DataFolder);
        }
    }
}

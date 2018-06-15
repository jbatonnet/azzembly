using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
using SourceTree.Model;
using SourceTree.Notifications;
using SourceTree.Properties;
using SourceTree.Repositories;
using SourceTree.Scheduler;
using SourceTree.Security;
using SourceTree.Utils;
using SourceTree.View;
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

        private new void OpenGetGitRight()
        {
            Debugger.Launch();

            string[] args = new[] { "/a:b", "/c", "d" };

            var options = args.Where(a => a.StartsWith("/"))
                          .Select(a => a.TrimStart('/'))
                          .ToDictionary(a => a.Substring(0, 1).ToLower(), a => a.Substring(1), StringComparer.InvariantCultureIgnoreCase);
            var parameters = args.Where(a => !a.StartsWith("/"))
                             .ToList();

            options.TryGetValue("patch", out string patchAssemblyPath);
            options.TryGetValue("source", out string sourceAssemblyPath);
            options.TryGetValue("target", out string targetAssemblyPath);
        }

        private new void RegisterCommands()
        {
            this.RegisterCommand(RoutedCommands.CloneNewRepoCommand, p => true, p => this.CloneNew());
            this.RegisterCommand(RoutedCommands.FileOpenCommand, p => true, p => this.FileOpen());
            this.RegisterCommand(RoutedCommands.RefreshCommand, p => true, p => this.FullManualRefresh());
            this.RegisterCommand(RoutedCommands.NextTabCommand, p => this.CanExecuteRepoCommand(), p => this.ViewNextTab());
            this.RegisterCommand(RoutedCommands.PrevTabCommand, p => this.CanExecuteRepoCommand(), p => this.ViewPrevTab());
            this.RegisterCommand(RoutedCommands.CloseTabCommand, p => this.CanExecuteRepoCommand(), p => this.ViewCloseTab());
            this.RegisterCommand(RoutedCommands.FileStatusViewCommand, p => this.CanExecuteRepoCommand(), p => this.FileStatusView());
            this.RegisterCommand(RoutedCommands.LogViewCommand, p => this.CanExecuteRepoCommand(), p => this.LogView());
            this.RegisterCommand(RoutedCommands.SearchViewCommand, p => this.CanExecuteRepoCommand(), p => this.SearchView());
            this.RegisterCommand(RoutedCommands.RepoSettingsCommand, p => this.CanExecuteRepoCommand(), p => this.RepoSettings());
            this.RegisterCommand(RoutedCommands.RefreshRemoteStatusCommand, p => this.CanExecuteRepoCommand(), p => this.RefreshRemoteStatus());
            this.RegisterCommand(RoutedCommands.CommitAllCommand, p => this.CanExecuteCommitAll(), p => this.CommitAll());
            this.RegisterCommand(RoutedCommands.CommitSelectedCommand, p => this.CanExecuteCommitSelected(), p => this.CommitSelected());
            this.RegisterCommand(RoutedCommands.DiscardCommand, p => this.CanExecuteDiscard(), p => this.Discard());
            this.RegisterCommand(RoutedCommands.DiscardSelectedCommand, p => this.CanExecuteDiscardSelected(), p => this.DiscardSelected());
            this.RegisterCommand(RoutedCommands.ToggleStagingCommand, p => this.CanExecuteToggleStaging(), p => this.ToggleStaging(p as IFileStatusRecord));
            this.RegisterCommand(RoutedCommands.ToggleStagingMultipleCommand, p => this.CanExecuteToggleStaging(), p => this.ToggleStaging(p as IEnumerable<IFileStatusRecord>));
            this.RegisterCommand(RoutedCommands.StashCommand, p => this.CanExecuteStash(), p => this.Stash());
            this.RegisterCommand(RoutedCommands.DraftCommitCommand, p => this.CanExecuteRepoCommand(), p => this.DraftCommit());
            this.RegisterCommand(RoutedCommands.PushCommand, p => this.CanExecuteRepoCommand(), p => this.Push());
            this.RegisterCommand(RoutedCommands.PullCommand, p => this.CanExecuteRepoCommand(), p => this.Pull());
            this.RegisterCommand(RoutedCommands.FetchCommand, p => this.CanExecuteFetch(), p => this.Fetch());
            this.RegisterCommand(RoutedCommands.CheckoutCommand, p => this.CanExecuteRepoCommand(), p => this.Checkout());
            this.RegisterCommand(RoutedCommands.CheckoutSelectedCommand, p => this.CanExecuteCheckoutSelected(), p => this.CheckoutSelected());
            this.RegisterCommand(RoutedCommands.BranchCommand, p => this.CanExecuteRepoCommand(), p => this.Branch());
            this.RegisterCommand(RoutedCommands.BranchSelectedCommand, p => this.CanExecuteBranchSelected(), p => this.BranchSelected());
            this.RegisterCommand(RoutedCommands.MergeCommand, p => this.CanExecuteRepoCommand(), p => this.Merge());
            this.RegisterCommand(RoutedCommands.MergeSelectedCommand, p => this.CanExecuteMergeSelected(), p => this.MergeSelected());
            this.RegisterCommand(RoutedCommands.RebaseSelectedCommand, p => this.CanExecuteRebaseSelected(), p => this.RebaseSelected());
            this.RegisterCommand(RoutedCommands.TagCommand, p => this.CanExecuteRepoCommand(), p => this.Tag());
            this.RegisterCommand(RoutedCommands.TagSelectedCommand, p => this.CanExecuteTagSelected(), p => this.TagSelected());
            this.RegisterCommand(RoutedCommands.ArchiveCommand, p => this.CanExecuteRepoCommand(), p => this.Archive());
            this.RegisterCommand(RoutedCommands.ArchiveSelectedCommand, p => this.CanExecuteArchiveSelected(), p => this.ArchiveSelected());
            this.RegisterCommand(RoutedCommands.InteractiveRebaseCommand, p => this.CanExecuteInteractiveRebaseRepoCommand(), p => this.InteractiveRebase());
            this.RegisterCommand(RoutedCommands.InteractiveRebaseFromCommitCommand, p => this.CanExecuteRebaseSelected(), p => this.InteractiveRebaseFromCommit());
            this.RegisterCommand(RoutedCommands.AddSubmoduleCommand, p => this.CanExecuteAddSubmoduleCommand(), p => this.AddSubmodule());
            this.RegisterCommand(RoutedCommands.AddLinkSubtreeCommand, p => this.CanExecuteRepoCommand(), p => this.AddLinkSubtree());
            this.RegisterCommand(RoutedCommands.FlowInitialiseCommand, p => this.CanExecuteFlowInitialise(), p => this.FlowInitialise());
            this.RegisterCommand(RoutedCommands.FlowNextActionCommand, p => this.CanExecuteRepoCommand(), p => this.FlowNextAction());
            this.RegisterCommand(RoutedCommands.FlowStartFeatureCommand, p => this.CanExecuteFlowStartFinish(), p => this.FlowStartFeature());
            this.RegisterCommand(RoutedCommands.FlowFinishFeatureCommand, p => this.CanExecuteFlowStartFinish(), p => this.FlowFinishFeature());
            this.RegisterCommand(RoutedCommands.FlowStartReleaseCommand, p => this.CanExecuteFlowStartFinish(), p => this.FlowStartRelease());
            this.RegisterCommand(RoutedCommands.FlowFinishReleaseCommand, p => this.CanExecuteFlowStartFinish(), p => this.FlowFinishRelease());
            this.RegisterCommand(RoutedCommands.FlowStartHotfixCommand, p => this.CanExecuteFlowStartFinish(), p => this.FlowStartHotfix());
            this.RegisterCommand(RoutedCommands.FlowFinishHotfixCommand, p => this.CanExecuteFlowStartFinish(), p => this.FlowFinishHotfix());
            this.RegisterCommand(RoutedCommands.GitLfsInitCommand, p => this.CanExecuteGitLfsInit(), p => this.GitLfsInit());
            this.RegisterCommand(RoutedCommands.GitLfsEditTrackingCommand, p => this.CanExecuteGitLfsOperation(), p => this.GitLfsEditTracking());
            this.RegisterCommand(RoutedCommands.GitLfsPullCommand, p => this.CanExecuteGitLfsOperation(), p => this.GitLfsPull());
            this.RegisterCommand(RoutedCommands.GitLfsFetchCommand, p => this.CanExecuteGitLfsOperation(), p => this.GitLfsFetch());
            this.RegisterCommand(RoutedCommands.GitLfsCheckoutCommand, p => this.CanExecuteGitLfsOperation(), p => this.GitLfsCheckout());
            this.RegisterCommand(RoutedCommands.GitLfsPruneCommand, p => this.CanExecuteGitLfsOperation(), p => this.GitLfsPrune());
            this.RegisterCommand(RoutedCommands.OpenCommand, p => this.CanExecuteOpenSelection(), p => this.OpenSelection());
            this.RegisterCommand(RoutedCommands.OpenInExplorerCommand, p => true, p => this.ShowInExplorer());
            this.RegisterCommand(RoutedCommands.ShowInExplorerCommand, p => this.CanExecuteRepoCommand(), p => this.ShowInExplorer());
            this.RegisterCommand(RoutedCommands.ExternalDiffCommand, p => this.CanExecuteExternalDiff(), p => this.ExternalDiff());
            this.RegisterCommand(RoutedCommands.CreatePatchCommand, p => this.CanExecuteRepoCommand(), p => this.CreatePatch());
            this.RegisterCommand(RoutedCommands.ApplyPatchCommand, p => this.CanExecuteRepoCommand(), p => this.ApplyPatch());
            this.RegisterCommand(RoutedCommands.AddCommand, p => this.CanExecuteStage(), p => this.Stage());
            this.RegisterCommand(RoutedCommands.TrackInGitLfsCommand, p => this.CanExecuteTrackInGitLfs(), p => this.TrackInGitLfs());
            this.RegisterCommand(RoutedCommands.RemoveCommand, p => this.CanExecuteRemove(), p => this.Remove());
            this.RegisterCommand(RoutedCommands.AddRemoveCommand, p => this.CanExecuteRepoCommand(), p => this.AddRemove());
            this.RegisterCommand(RoutedCommands.UnstageCommand, p => this.CanExecuteUnstage(), p => this.Unstage());
            this.RegisterCommand(RoutedCommands.StopTrackingCommand, p => this.CanExecuteStopTracking(), p => this.StopTracking());
            this.RegisterCommand(RoutedCommands.IgnoreCommand, p => this.CanExecuteIgnore(), p => this.Ignore());
            this.RegisterCommand(RoutedCommands.ResetToCommitCommand, p => this.CanExecuteResetToCommit(), p => this.ResetToCommit());
            this.RegisterCommand(RoutedCommands.ContinueRebaseCommand, p => this.CanExecuteContinueAbortRebase(), p => this.ContinueRebase());
            this.RegisterCommand(RoutedCommands.AbortRebaseCommand, p => this.CanExecuteContinueAbortRebase(), p => this.AbortRebase());
            this.RegisterCommand(RoutedCommands.ResolveExtMergeCommand, p => this.CanExecuteResolveAction(), p => this.ResolveExtMerge());
            this.RegisterCommand(RoutedCommands.ResolveUsingTheirsCommand, p => this.CanExecuteResolveAction(), p => this.ResolveUsingTheirs());
            this.RegisterCommand(RoutedCommands.ResolveRestartMergeCommand, p => this.CanExecuteResolveRestartUnresolveAction(), p => this.ResolveRestartMerge());
            this.RegisterCommand(RoutedCommands.ResolveUsingMineCommand, p => this.CanExecuteResolveAction(), p => this.ResolveUsingMine());
            this.RegisterCommand(RoutedCommands.ResolveMarkResolvedCommand, p => this.CanExecuteResolveAction(), p => this.ResolveMarkResolved());
            this.RegisterCommand(RoutedCommands.ResolveMarkUnresolvedCommand, p => this.CanExecuteResolveRestartUnresolveAction(), p => this.ResolveMarkUnresolved());
            this.RegisterCommand(RoutedCommands.LogSelectedCommand, p => this.CanExecuteLogSelected(), p => this.LogSelected());
            this.RegisterCommand(RoutedCommands.BlameSelectedCommand, p => this.CanExecuteBlameSelected(), p => this.BlameSelected());
            this.RegisterCommand(RoutedCommands.CopyCommand, p => this.CanExecuteCopy(), p => this.Copy());
            this.RegisterCommand(RoutedCommands.CopyPathCommand, p => this.CanExecuteCopy(), p => this.Copy());
            this.RegisterCommand(RoutedCommands.AboutCommand, p => true, p => this.About());
            this.RegisterCommand(RoutedCommands.OpenGetGitRightSiteCommand, p => true, p => this.OpenGetGitRight());
            this.RegisterCommand(RoutedCommands.OpenSourceTreeWebsiteCommand, p => true, p => this.OpenSourceTreeWebsite());
            this.RegisterCommand(RoutedCommands.SupportCommand, p => true, p => this.Support());
            this.RegisterCommand(RoutedCommands.OpenGetStartedWithSourceTreeSiteCommand, p => true, p => this.OpenGetStartedWithSourceTree());
            this.RegisterCommand(RoutedCommands.PreferencesCommand, p => true, p => this.Preferences());
            this.RegisterCommand(RoutedCommands.ReleaseNotesCommand, p => true, p => WindowsOSHelper.OpenWebBrowser(Settings.Default.SourceTreeHomeWebSite + Settings.Default.ReleaseNotesRelativeUrl, this.NotificationsManager));
            this.RegisterCommand(RoutedCommands.ExitCommand, p => true, p => Application.Current.Shutdown());
            this.RegisterCommand(RoutedCommands.WelcomeWizardCommand, p => true, p => AppRoot.Current.Onboard());
            this.RegisterCommand(RoutedCommands.HyperlinkCommand, p => true, this.OpenLink);
            this.RegisterCommand(RoutedCommands.TerminalCommand, p => true, p => this.Terminal());
            this.RegisterCommand(RoutedCommands.LaunchSshAgentCommand, p => true, p => this.LaunchSshAgent());
            this.RegisterCommand(RoutedCommands.CreateImportSshKeyCommand, p => true, p => this.LaunchPuttyGen());
            this.RegisterCommand(RoutedCommands.AddSshKeyCommand, p => true, p => this.AddExtraOpenSshKey());
            this.RegisterCommand(RoutedCommands.ProcessViewerCommand, p => true, p => this.OpenProcessViewer());
            this.RegisterCommand(RoutedCommands.CherryPickCommand, p => this.CanExecuteCherryPick(), p => this.CherryPick());
            this.RegisterCommand(RoutedCommands.ReverseCommitCommand, p => this.CanExecuteReverseCommit(), p => this.ReverseCommit());
            this.RegisterCommand(RoutedCommands.ContinueGraftCommand, p => this.CanExecuteContinueAbortGraft(), p => this.ContinueGraft());
            this.RegisterCommand(RoutedCommands.AbortGraftCommand, p => this.CanExecuteContinueAbortGraft(), p => this.AbortGraft());
            this.RegisterCommand(RoutedCommands.CreatePullRequestCommand, p => this.CanExecuteCreatePullRequest(), p => this.CreatePullRequest());
            this.RegisterCommand(RoutedCommands.CustomActionCommand, this.CanExecuteCustomAction, this.ExecuteCustomAction);
            this.RegisterCommand(RoutedCommands.NewTabCommand, p => true, p => this.NewTab());
            this.RegisterCommand(RoutedCommands.BenchmarkRepoCommand, p => this.CanExecuteRepoCommand(), p => this.BenchmarkRepo());
        }
    }
}

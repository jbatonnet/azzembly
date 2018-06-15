﻿using System.Collections.Generic;
using System.Windows;

using Azzembly.Patcher;

using SourceTree.Accounts;
using SourceTree.Analytics;
using SourceTree.Configuration;
using SourceTree.CustomActions;
using SourceTree.Diagnostics;
using SourceTree.Dvcss;
using SourceTree.Model;
using SourceTree.Notifications;
using SourceTree.Repositories;
using SourceTree.Scheduler;
using SourceTree.Utils;
using SourceTree.View.ChangeSets;
using SourceTree.View.Container;
using SourceTree.View.Diff;
using SourceTree.View.FileLists;
using SourceTree.View.Process;
using SourceTree.ViewModel.Repositories;
using SourceTree.Web;

namespace SourceTree.ViewModel
{
    [Patch(typeof(RepoTabViewModel))]
    public class RepoTabViewModelPatch : RepoTabViewModel
    {
        public RepoTabViewModelPatch(Repository r, IRepositoryTabContainerViewModel repositoryTabContainerViewModel, ICustomActionsManager customActionsManager, ISchedulerManager schedulerManager, IRepositoryManager repositoryManager, IAnalyticsDataManager analyticsDataManager, ITraceManager traceManager, IDispatcher dispatcher, IAccountManager accountManager, IFailureHandler failureHandler, IDvcsManager dvcsManager, IRepositoryMonitorManager repositoryMonitorManager, IWebManager webManager, IFileListViewManager fileListViewManager, IFileListContainerViewManager fileListContainerViewManager, IDiffViewManager diffViewManager, IChangeSetViewManager changeSetViewManager, IConfigurationManager configurationManager, IProcessDialogViewManager processDialogViewManager, INotificationsManager notificationsManager, IEnumerable<ISideBarRoot> extentionSideBarRoots)
            : base(r, repositoryTabContainerViewModel, customActionsManager, schedulerManager, repositoryManager, analyticsDataManager, traceManager, dispatcher, accountManager, failureHandler, dvcsManager, repositoryMonitorManager, webManager, fileListViewManager, fileListContainerViewManager, diffViewManager, changeSetViewManager, configurationManager, processDialogViewManager, notificationsManager, extentionSideBarRoots)
        {
        }

        public new void ShowInExplorer()
        {
            //List<FileStatusRecord> list = this.SelectedFileRecords(false).ToList<FileStatusRecord>();
            //if (list.Any<FileStatusRecord>())
            //{
            //    foreach (FileStatusRecord fileStatusRecord in list)
            //    {
            //        if (fileStatusRecord.Status != FileStatus.Deleted && fileStatusRecord.Status != FileStatus.Missing)
            //            WindowsOSHelper.ShowPathInExplorer(this._repo.GetAbsoluteFilename(fileStatusRecord.Filepath ?? fileStatusRecord.Path ?? fileStatusRecord.Filename));
            //    }
            //}
            //else
            //    WindowsOSHelper.ShowPathInExplorer(this._repo.Path);

            WindowsOSHelper.ShowPathInExplorer(this.Repo.Path);
        }
    }
}

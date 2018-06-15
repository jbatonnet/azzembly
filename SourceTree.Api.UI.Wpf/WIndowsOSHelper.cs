using System;
using System.Diagnostics;

using Azzembly.Patcher;

using SourceTree.Diagnostics;
using SourceTree.Dvcss;
using SourceTree.Localisation;
using SourceTree.Model;
using SourceTree.Notifications;
using SourceTree.Properties;

namespace SourceTree.Utils
{
    [Patch(typeof(WindowsOSHelper))]
    public class WindowsOSHelperPatch
    {
        public static Process OpenTerminal(string path, RepoType repoType, IDvcsManager dvcsManager, IFailureHandler failureHandler, INotificationsManager notificationsManager)
        {
            Process result;
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    path = WindowsOSHelper.GetHomeDirectory();
                    WindowsOSHelper.Logger.Debug("Path was null/empty so reseting to be users home directory '{0}'", new object[]
                    {
                        path
                    });
                }
                if (string.IsNullOrWhiteSpace(path))
                {
                    WindowsOSHelper.Logger.Error("Path was null/empty AFTER reseting to be users home directory '{0}'", new object[]
                    {
                        path
                    });
                    if (notificationsManager != null)
                    {
                        notificationsManager.ShowErrorDialog(Messages.ErrorTitle, string.Format(Messages.RequestedFilePathDoesNotExist, path));
                    }
                    result = null;
                }
                else if (dvcsManager == null)
                {
                    WindowsOSHelper.Logger.Debug("Path was null/empty so reseting to be users home directory '{0}'", new object[]
                    {
                        path
                    });
                    result = WindowsOSHelper.RunCmdProcess(WindowsOSHelper.GetCmdProcess(path), failureHandler);
                }
                else if (!FileHelper.DirectoryExists(path))
                {
                    if (notificationsManager != null)
                    {
                        notificationsManager.ShowErrorDialog(Messages.ErrorTitle, string.Format(Messages.RequestedFilePathDoesNotExist, path));
                    }
                    result = null;
                }
                else
                {
                    ProcessStartInfo cmdProcess = WindowsOSHelper.GetCmdProcess(path);
                    if (Settings.Default.HgWhichOne == 0)
                    {
                        EnvironmentHelper.AddToPathEnvironment(dvcsManager.GetRepositoryHandler(repoType).BasePath, cmdProcess.EnvironmentVariables, true);
                        cmdProcess.UseShellExecute = false;
                    }
                    result = WindowsOSHelper.RunCmdProcess(cmdProcess, failureHandler);
                }
            }
            catch (Exception exception)
            {
                WindowsOSHelper.Logger.Error(exception, "Unable to open terminal for path '{0}' and repo type '{1}'", new object[]
                {
                    path,
                    repoType
                });
                result = null;
            }
            return result;
        }
    }
}

using System;
using System.Diagnostics;
using System.Linq;

using Azzembly.Patcher;

namespace SourceTree
{
    [Patch(typeof(AppRoot))]
    public class AppRootPatch : AppRoot
    {
        public new bool Onboard()
        {
            // return this.OnboardingManager.Run();

            /*Process[] processes = Process.GetProcessesByName("devenv");
            if (processes.Length > 0)
                Debugger.Launch();

            string[] args = new[] { "/a:b", "/c", "d" };

            var options = args.Where(a => a.StartsWith("/"))
                          .Select(a => a.TrimStart('/'))
                          .ToDictionary(a => a.Substring(0, 1).ToLower(), a => a.Substring(1), StringComparer.InvariantCultureIgnoreCase);
            var parameters = args.Where(a => !a.StartsWith("/"))
                             .ToList();

            options.TryGetValue("patch", out string patchAssemblyPath);
            options.TryGetValue("source", out string sourceAssemblyPath);
            options.TryGetValue("target", out string targetAssemblyPath);*/

            return true;
        }
    }
}

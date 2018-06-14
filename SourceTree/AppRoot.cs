
using System;
using Azzembly.Patcher;

namespace SourceTree
{
    [Patch(typeof(AppRoot))]
    public class AppRootPatch : AppRoot
    {
        public new bool Onboard()
        {
            // return this.OnboardingManager.Run();

            return true;
        }
    }
}

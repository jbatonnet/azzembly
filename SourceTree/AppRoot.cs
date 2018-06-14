
using Azzembly.Patcher;

namespace SourceTree
{
    [Patch(typeof(AppRoot))]
    public class AppRootPatch
    {
        public bool Onboard()
        {
            // return this.OnboardingManager.Run();

            return true;
        }
    }
}

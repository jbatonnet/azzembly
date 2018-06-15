using System.Windows;

using Azzembly.Patcher;

namespace SourceTree.View.Dialogs
{
    [Patch(typeof(SimpleTextDialog))]
    public class SimpleTextDialogPatch : SimpleTextDialog
    {
        private new void TextField_OnLoaded(object sender, RoutedEventArgs e)
        {
            this.TextField.Focus();
            this.TextField.SelectAll();
        }
    }
}

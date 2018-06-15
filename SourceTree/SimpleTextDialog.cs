using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Azzembly.Patcher;

namespace SourceTree.View.Dialogs
{
    [Patch(typeof(SimpleTextDialog))]
    public class SimpleTextDialogPatch : SimpleTextDialog
    {
        private new void TextField_OnLoaded(object sender, RoutedEventArgs e)
        {
            this.TextField.SelectAll();
        }
    }
}

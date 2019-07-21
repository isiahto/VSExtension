using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITExtensionProject.Dialog
{
    public class BaseDialogWindow : DialogWindow
    {
        public BaseDialogWindow()
        {
            HasMaximizeButton = true;
            HasMinimizeButton = true;
        }
    }
}

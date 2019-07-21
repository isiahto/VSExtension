using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;
using Microsoft;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Task = System.Threading.Tasks.Task;

namespace ITExtensionProject.Commands
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class DocumentCodeSpanCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 256;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("2823e425-7a94-45f6-9f44-33d46193d38d");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentCodeSpanCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private DocumentCodeSpanCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static DocumentCodeSpanCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in DocumentCodeSpanCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new DocumentCodeSpanCommand(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            // https://docs.microsoft.com/en-us/visualstudio/extensibility/how-to-provide-an-asynchronous-visual-studio-service?view=vs-2019
            // _ = ProcessSelectionAsync();

            var n = new Dialog.AddDocumentationWindow();
            n.ShowDialog();
        }

        private async Task ProcessSelectionAsync()
        {
            var selection = await GetSelectionAsync(ServiceProvider);
            var activeDocPath = await GetActiveFilePathAsync(ServiceProvider);

            var message = $"Doc: {activeDocPath}\nText:\n\"{selection.ToString()}\"";
            DisplayMessage(message);
        }


        // My custom methods:
        private async Task<TextViewSelection> GetSelectionAsync(Microsoft.VisualStudio.Shell.IAsyncServiceProvider serviceProvider)
        {
            var service = await serviceProvider.GetServiceAsync(typeof(SVsTextManager));
            var txtManager = service as IVsTextManager2;

            txtManager.GetActiveView2(1, null, (uint)_VIEWFRAMETYPE.vftCodeWindow, out IVsTextView view);

            view.GetSelection(out int startLine, out int startCol, out int endLine, out int endCol);
            var start = new TextViewPosition(startLine, startCol);
            var end = new TextViewPosition(endLine, endCol);

            view.GetSelectedText(out string selectedText);
            var selection = new TextViewSelection(start, end, selectedText);

            return selection;
        }


        private async Task<string> GetActiveFilePathAsync(Microsoft.VisualStudio.Shell.IAsyncServiceProvider serviceProvider)
        {
            // VSTHRD010 
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(this.package.DisposalToken);

            var applicationObject = await serviceProvider.GetServiceAsync<DTE, DTE2>();
            return applicationObject.ActiveDocument.FullName; 
        }


        private void DisplayMessage(string message)
        {
            string title = "Doc Code Span";

            // Show a message box to prove we were here
            VsShellUtilities.ShowMessageBox(
                this.package,
                message,
                title,
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }


    public struct TextViewPosition
    {
        public TextViewPosition(int line, int column)
        {
            Line = line;
            Column = column;
        }

        public int Line { get; }
        public int Column { get; }


        public static bool operator <(TextViewPosition a, TextViewPosition b)
        {
            if (a.Line < b.Line)
            {
                return true;
            }
            else if (a.Line == b.Line)
            {
                return a.Column < b.Column;
            }
            else
            {
                return false;
            }
        }

        public static bool operator >(TextViewPosition a, TextViewPosition b)
        {
            if (a.Line > b.Line)
            {
                return true;
            }
            else if (a.Line == b.Line)
            {
                return a.Column > b.Column;
            }
            else
            {
                return false;
            }
        }

        public static TextViewPosition Min(TextViewPosition a, TextViewPosition b)
        {
            return a > b ? b : a;
        }

        public static TextViewPosition Max(TextViewPosition a, TextViewPosition b)
        {
            return a > b ? a : b;
        }

        public override string ToString()
        {
            return $"[{Line}:{Column}]";
        }
    }

    public struct TextViewSelection
    {
        public TextViewPosition StartPosition { get; set; }
        public TextViewPosition EndPosition { get; set; }
        public string Text { get; set; }

        public TextViewSelection(TextViewPosition a, TextViewPosition b, string text)
        {
            StartPosition = TextViewPosition.Min(a, b);
            EndPosition = TextViewPosition.Max(a, b);
            Text = text;
        }

        public override string ToString()
        {
            return $"({StartPosition.ToString()}-{EndPosition.ToString()})={Text}";
        }
    }
}

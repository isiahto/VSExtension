# Log

## Part 3: Add to context menu, Get selected code
- Added a context menu in code window
- Implemented execute command that use various apis:
		- DTE
		- SVsTextManager
- The development API seems to be updated, which using IAsyncServiceProvider instead of IServiceProvider, which requires some changes to the implementation
		- Refer to [MS Docs](https://docs.microsoft.com/en-us/visualstudio/extensibility/how-to-provide-an-asynchronous-visual-studio-service?view=vs-2019)
- Used _ as a discard variable: `_ = ProcessSelectionAsync()`
- Discovered `ThreadHelper.ThrowIfNotOnUIThread()` from warning `VSTHRD010`: Invoke single-threaded types on Main thread
- Also discovered `Assumes` class with `Assumes.Null()`, `Assumes.NotReachable()` and more
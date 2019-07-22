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


## Part 4: Show a popup Window
This part of the tutorial series is the shortest, it only briefly shows how dialog works in VS extension, which uses WPF controls.

Luckly I have quite some experience with MVVM in WPF from work, hence I'd skip this part since functionality is implemented on WPF side, and VS extension is only wiring up the custom control, which I am familiar of.
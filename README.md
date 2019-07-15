## VS Extension Project

### Motivation:
The motivation of this project is, while debugging, the debugger is not smart enough
to evaluate array indexer (i.e. someArr[10]), it would auto-highlight the variable (someArr, 10) and
shows their data-type, however, it is 

### Goal:
Create an extension for debugging that,
automatically evaluates highlighted expression, given that the highlighted text buffer is a valid expression.

### MVP:
Since expression is much more complicated when it involves other assemblies/namespaces/classes,
the first acceptable release would be:
`Able to evaluate array indexer`

#### Good to have:
- Smart enough to look forward and backward to complete the buffer as a valid expression.
- Setting dialog under `Tools` context menu



###### Reference
- https://michaelscodingspot.com/visual-studio-2017-extension-development-tutorial-part-2-add-menu-item/
- https://docs.microsoft.com/en-us/visualstudio/extensibility/extensibility-hello-world?view=vs-2019
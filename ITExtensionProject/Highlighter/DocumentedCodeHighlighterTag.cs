using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITExtensionProject.Highlighter
{
    public interface IEventAggregator
    {

    }

    public class DocumentedCodeHighlighterTag : TextMarkerTag
    {
        public DocumentedCodeHighlighterTag()
            : base("MarkerFormatDefinition/DocumentedCodeFormatDefinition") { }
    }

    class DocumentedCodeHighlighterTagger : ITagger<DocumentedCodeHighlighterTag>
    {
        public ITextView TextView { get; }
        public ITextBuffer Buffer { get; }
        public IEventAggregator EventAggregator { get; }

        public string DocFileName { get; set; }

        public DocumentedCodeHighlighterTagger(ITextView textView, ITextBuffer buffer, IEventAggregator eventAggregator)
        {
            TextView = textView;
            Buffer = buffer;
            EventAggregator = eventAggregator;
        }


        public string GetFileName(ITextBuffer buffer)
        {
            buffer.Properties.TryGetProperty(typeof(ITextDocument), out ITextDocument document);
            return document?.FilePath ?? throw new ArgumentException(nameof(buffer));
        }

        public void OnDocumentationAdded(DocumentationAddedEvent e)
        {
            var filepath = e.Filepath;
            if (filepath == DocFileName)
            {
                TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(
                    new SnapshotSpan(
                        Buffer.CurrentSnapshot, new Span(0, Buffer.CurrentSnapshot.Length - 1))
                        )
                    );
            }
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<DocumentedCodeHighlighterTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            var documentation = Services.DocumentationFileSerializer.Deserialize(DocFileName);
            var res = new List<ITagSpan<DocumentedCodeHighlighterTag>>();

            var currentSnapshot = Buffer.CurrentSnapshot;
            foreach (var fragment in documentation.Fragments)
            {
                int startPos = fragment.Selection.StartPosition;
                int length = fragment.Selection.EndPosition - fragment.Selection.StartPosition;
                var snapshotSpan = new SnapshotSpan(
                     currentSnapshot, new Span(startPos, length));
                res.Add(new TagSpan<DocumentedCodeHighlighterTag>(snapshotSpan,
                     new DocumentedCodeHighlighterTag()));
            }

            return res;
        }
    }

    public class DocumentationAddedEvent
    {
        public string Filepath { get; set; }
    }
}

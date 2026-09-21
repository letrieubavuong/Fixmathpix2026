using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace FixMathpix2025
{
    // Minimal ITextMarker service and marker implementation adapted from AvalonEdit samples
    public interface ITextMarkerService : IBackgroundRenderer, IVisualLineTransformer
    {
        ITextMarker Create(int startOffset, int length);
        void Remove(ITextMarker marker);
        IEnumerable<ITextMarker> TextMarkers { get; }
    }

    public interface ITextMarker
    {
        int StartOffset { get; }
        int Length { get; }
        Color? MarkerColor { get; set; }
        TextMarkerTypes MarkerTypes { get; set; }
        object ToolTip { get; set; }
        void Delete();
    }

    [Flags]
    public enum TextMarkerTypes
    {
        None = 0x0,
        SquigglyUnderline = 0x1,
        NormalUnderline = 0x2
    }

    public class TextMarkerService : DocumentColorizingTransformer, ITextMarkerService
    {
        private readonly TextDocument _document;
        private readonly List<TextMarker> markers = new List<TextMarker>();

        public TextMarkerService(TextDocument document)
        {
            _document = document ?? throw new ArgumentNullException(nameof(document));
        }

        public IEnumerable<ITextMarker> TextMarkers => markers;

        public ITextMarker Create(int startOffset, int length)
        {
            var m = new TextMarker(startOffset, length, this);
            markers.Add(m);
            _document.Changing += Document_Changing;
            return m;
        }

        private void Document_Changing(object sender, DocumentChangeEventArgs e)
        {
            // Simple strategy: delete markers that overlap the change
            var toDelete = markers.Where(m => e.Offset < m.StartOffset + m.Length && e.Offset + e.InsertionLength > m.StartOffset).ToList();
            foreach (var d in toDelete) d.Delete();
        }

        public void Remove(ITextMarker marker)
        {
            if (marker is TextMarker m)
            {
                markers.Remove(m);
            }
        }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            // Basic drawing for squiggly underline
            foreach (var marker in markers.OfType<TextMarker>())
            {
                if (marker.MarkerTypes.HasFlag(TextMarkerTypes.SquigglyUnderline) && marker.MarkerColor.HasValue)
                {
                    foreach (var r in BackgroundGeometryBuilder.GetRectsForSegment(textView, new TextSegment { StartOffset = marker.StartOffset, Length = marker.Length }))
                    {
                        var pen = new Pen(new SolidColorBrush(marker.MarkerColor.Value), 1);
                        pen.Freeze();
                        var y = r.Bottom - 2;
                        var x = r.Left;
                        var width = r.Width;

                        // Draw simple wave
                        int segments = Math.Max(2, (int)(width / 4));
                        var geo = new System.Windows.Media.StreamGeometry();
                        using (var ctx = geo.Open())
                        {
                            ctx.BeginFigure(new System.Windows.Point(x, y), false, false);
                            for (int i = 1; i <= segments; i++)
                            {
                                bool up = (i % 2) == 0;
                                ctx.LineTo(new System.Windows.Point(x + i * (width / segments), y + (up ? -2 : 2)), true, false);
                            }
                        }
                        geo.Freeze();
                        drawingContext.DrawGeometry(null, pen, geo);
                    }
                }
            }
        }

        public KnownLayer Layer => KnownLayer.Selection;

        protected override void ColorizeLine(DocumentLine line)
        {
            // No colorization, we rely on drawing for underlines
        }

        public void Transform(ITextRunConstructionContext context, IList<VisualLineElement> elements)
        {
            // No-op for now
        }

        private class TextMarker : TextSegment, ITextMarker
        {
            private readonly TextMarkerService service;

            public TextMarker(int start, int length, TextMarkerService service)
            {
                StartOffset = start;
                Length = length;
                this.service = service;
            }

            public Color? MarkerColor { get; set; }
            public TextMarkerTypes MarkerTypes { get; set; }
            public object ToolTip { get; set; }

            public void Delete()
            {
                service.Remove(this);
            }
        }
    }
}

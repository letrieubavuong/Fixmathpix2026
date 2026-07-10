using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System;

namespace FixMathpix2025
{
    public class LatexCompletionData : ICompletionData
    {
        public LatexCompletionData(string text, string description = null)
        {
            this.Text = text;
            this.Description = description;
        }

        public System.Windows.Media.ImageSource Image => null;

        public string Text { get; }

        // Nội dung hiển thị trong danh sách gợi ý.
        public object Content => this.Text;

        // Tooltip hiển thị khi di chuột qua một mục.
        public object Description { get; }

        public double Priority => 0;

        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            string insertionText = this.Text;
            int caretOffset = 0;

            switch (this.Text)
            {
                case "dfrac":
                    insertionText = "dfrac{}{}";
                    caretOffset = -3; // Đặt con trỏ vào giữa cặp ngoặc đầu tiên
                    break;
                case "sqrt":
                    insertionText = "sqrt[]{}";
                    caretOffset = -3; // Đặt con trỏ vào giữa cặp ngoặc vuông
                    break;
                // Thêm các trường hợp khác ở đây
            }

            textArea.Document.Replace(completionSegment, insertionText);

            // Di chuyển con trỏ nếu cần
            if (caretOffset != 0)
            {
                textArea.Caret.Offset += caretOffset;
            }
        }
    }
}
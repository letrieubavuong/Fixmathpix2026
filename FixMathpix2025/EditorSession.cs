using System;

namespace FixMathpix2025
{
    /// <summary>
    /// Đại diện cho trạng thái của tài liệu đang được mở trong trình soạn thảo.
    /// Không phụ thuộc vào UI (Window, TextEditor, MessageBox, OpenFileDialog).
    /// </summary>
    public class EditorSession
    {
        public string CurrentFilePath { get; private set; }
        public bool IsDirty { get; private set; }

        public bool HasFile => !string.IsNullOrEmpty(CurrentFilePath);

        public void MarkOpened(string path)
        {
            CurrentFilePath = path;
            IsDirty = false;
        }

        public void MarkSaved(string path)
        {
            CurrentFilePath = path;
            IsDirty = false;
        }

        public void MarkModified()
        {
            IsDirty = true;
        }

        public void MarkClosed()
        {
            CurrentFilePath = null;
            IsDirty = false;
        }
    }
}

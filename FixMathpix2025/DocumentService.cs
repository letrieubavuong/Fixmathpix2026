using System;
using System.IO;

namespace FixMathpix2025
{
    /// <summary>
    /// Xử lý đọc và ghi tài liệu từ đĩa.
    /// Không chứa logic UI hay MessageBox/FileDialog.
    /// </summary>
    public class DocumentService
    {
        public virtual string ReadText(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            return File.ReadAllText(filePath);
        }

        public virtual void WriteText(string filePath, string content)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            File.WriteAllText(filePath, content ?? string.Empty);
        }
    }
}

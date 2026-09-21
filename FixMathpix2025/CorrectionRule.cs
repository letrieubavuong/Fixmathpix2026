using System;
using System.ComponentModel;

namespace FixMathpix2025
{
    /// <summary>
    /// Model dữ liệu cho một quy tắc sửa lỗi chính tả.
    /// </summary>
    public class CorrectionRule : INotifyPropertyChanged
    {
        public CorrectionRule() { }

        public CorrectionRule(string findText, string replaceText)
        {
            _findText = findText;
            _replaceText = replaceText;
        }
        private string _findText;
        public string FindText
        {
            get => _findText;
            set
            {
                if (_findText != value)
                {
                    _findText = value;
                    OnPropertyChanged(nameof(FindText));
                }
            }
        }

        private string _replaceText;
        public string ReplaceText
        {
            get => _replaceText;
            set
            {
                if (_replaceText != value)
                {
                    _replaceText = value;
                    OnPropertyChanged(nameof(ReplaceText));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

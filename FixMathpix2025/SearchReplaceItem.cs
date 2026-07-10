using System.ComponentModel;

namespace FixMathpix2025
{
    public class SearchReplaceItem
    {
        public string FindText { get; set; }
        public string ReplaceText { get; set; }
        public bool MatchCase { get; set; }
        public bool WholeWord { get; set; }
        public bool UseRegex { get; set; }
    }
}
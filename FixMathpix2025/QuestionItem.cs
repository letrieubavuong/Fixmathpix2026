namespace FixMathpix2025
{
    public class QuestionItem
    {
        public string Content { get; set; }
        public int OriginalIndex { get; set; }
        public int Length { get; set; }
        public QuestionKind Kind { get; set; }

        public int Type
        {
            get => (int)Kind;
            set => Kind = (QuestionKind)value;
        }
    }
}

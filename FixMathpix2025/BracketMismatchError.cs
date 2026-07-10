namespace FixMathpix2025
{
    public class BracketMismatchError
    {
        public int Offset { get; }
        public int Length { get; }
        public string Message { get; }

        public BracketMismatchError(int offset, int length, string message)
        {
            Offset = offset;
            Length = length;
            Message = message;
        }
    }
}
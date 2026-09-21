using Xunit;
using FixMathpix2025;

namespace FixMathpix2025.Tests
{
    public class LatexTransformationTests
    {
        [Fact]
        public void FixMathtype_ShouldCleanWhitespaceAndExponentSubscriptBrackets()
        {
            string input = @"  \^{2} + _{x} + {  abc  } + [  xyz  ] .";
            string result = LatexTransformationService.FixMathtype(input);

            Assert.Contains("^2", result);
            Assert.Contains("_x", result);
            Assert.Contains("{abc", result);
            Assert.Contains("[xyz", result);
            Assert.DoesNotContain(@"\^{", result);
            Assert.DoesNotContain(@"_{", result);
        }

        [Fact]
        public void FixMathtype_ShouldHandleNullAndEmpty()
        {
            Assert.Null(LatexTransformationService.FixMathtype(null));
            Assert.Equal("", LatexTransformationService.FixMathtype(""));
        }

        [Fact]
        public void CleanupText_ShouldNormalizeWhitespaceAndPunctuation()
        {
            string input = "\t\tCho tam giac ABC , co goc A = 90  do .\r\n  Cau hoi ?  Dung !";
            string expected = "Cho tam giac ABC, co goc A = 90 do.\n Cau hoi? Dung!";

            string result = LatexTransformationService.CleanupText(input);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void CleanupText_ShouldHandleNullAndEmpty()
        {
            Assert.Null(LatexTransformationService.CleanupText(null));
            Assert.Equal("", LatexTransformationService.CleanupText(""));
        }

        [Fact]
        public void ProcessChuyenTex_GoldenTest_FullTransformation()
        {
            string input = "Cho hàm số \\frac{1}{2}x^2.\nLời giải:\nTa có \\mathrm{dx}.\nA. Option 1\nB. Option 2\nC. Option 3\nD. Option 4\n";
            string expected = "\\begin{ex}\nCho hàm số\\dfrac{1}{2}x^2.\\loigiai{Ta có dx.\\choice\n{Option 1}\n{Option 2}\n{Option 3}\n{Option 4}\\end{ex}";

            string result = LatexTransformationService.ProcessChuyenTex(input);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void ProcessChuyenTex_ShouldHandleNullAndEmpty()
        {
            Assert.Null(LatexTransformationService.ProcessChuyenTex(null));
            Assert.Equal("", LatexTransformationService.ProcessChuyenTex(""));
        }

        [Fact]
        public void FixEnumerate_ShouldReplaceEnumerateInValidEnvironments()
        {
            string input = "\\begin{ex}\n\\begin{enumerate}[1.]\n\\item Item 1\n\\end{enumerate}\n\\loigiai{\nTest\n}\n\\end{ex}";
            string result = LatexTransformationService.FixEnumerate(input, out int count);

            Assert.Equal(1, count);
            Assert.Contains("\\begin{listEX}[1]", result);
            Assert.Contains("\\end{listEX}", result);
            Assert.DoesNotContain("enumerate", result);
        }

        [Fact]
        public void FixEnumerate_ShouldNotReplaceEnumerateOutsideTargetEnvironments()
        {
            string input = "\\begin{center}\n\\begin{enumerate}\n\\item Item 1\n\\end{enumerate}\n\\end{center}";
            string result = LatexTransformationService.FixEnumerate(input, out int count);

            Assert.Equal(0, count);
            Assert.Equal(input, result);
        }

        [Fact]
        public void FixTikzpicture_ShouldNormalizeOptions()
        {
            string input = "\\begin{tikzpicture}[thick, red, scale=1.5]\n\\draw (0,0) -- (1,1);\n\\end{tikzpicture}";
            string result = LatexTransformationService.FixTikzpicture(input, out int count);

            Assert.Equal(1, count);
            Assert.Contains("draw=Mapcolor, very thick, scale=1.5", result);
        }

        [Fact]
        public void FixHevaContent_ShouldRemoveAmpersandAndNormalizeBackslashes()
        {
            string input = "x & + y = 1 \\\\   2x & - y = 3";
            string expected = "x  + y = 1 \\\\ 2x  - y = 3";

            string result = LatexTransformationService.FixHevaContent(input);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void TitleCaseMath_ShouldUppercaseTextAndPreserveMathMode()
        {
            string input = "dang 1: tinh gia tri cua $x^2 + y$ trong tam giac";
            string expected = "DANG 1: TINH GIA TRI CUA $x^2 + y$ TRONG TAM GIAC";

            string result = LatexTransformationService.TitleCaseMath(input);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void UppercaseSectionContent_ShouldUppercaseTextPreserveCommandsAndMath()
        {
            string input = "Khai niem $f(x)$ va \\textbf{dinh ly}";
            string expected = "KHAI NIEM $f(x)$ VA \\textbf{DINH LY}";

            string result = LatexTransformationService.UppercaseSectionContent(input);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void FormatListEX_ShouldFormatSelectedLines()
        {
            string input = "a. Item A\nb) Item B\n- Item C";
            string result = LatexTransformationService.FormatListEX(input, "2");

            Assert.StartsWith("\\begin{listEX}[2]", result);
            Assert.Contains("\\item Item A", result);
            Assert.Contains("\\item Item B", result);
            Assert.Contains("\\item Item C", result);
            Assert.EndsWith("\\end{listEX}", result);
        }

        [Fact]
        public void FormatEnumEX_ShouldFormatSelectedLines()
        {
            string input = "1. Item 1\n2) Item 2";
            string result = LatexTransformationService.FormatEnumEX(input, "3", "1.");

            Assert.StartsWith("\\begin{enumEX}[1.]{3}", result);
            Assert.Contains("\\item Item 1", result);
            Assert.Contains("\\item Item 2", result);
            Assert.EndsWith("\\end{enumEX}", result);
        }

        [Fact]
        public void WrapWithAlignStar_ShouldFormatEqualsAndWrapEnvironment()
        {
            string input = "A = B = C";
            string result = LatexTransformationService.WrapWithAlignStar(input);

            Assert.StartsWith("\\begin{align*}", result);
            Assert.Contains("A = B \\", result);
            Assert.Contains("= C", result);
            Assert.EndsWith("\\end{align*}", result);
        }

        [Fact]
        public void WrapWithEquation_ShouldCleanAlignAndWrapInEquation()
        {
            string input = "\\begin{align*} x = 1 \\tag{1} \\end{align*}";
            string result = LatexTransformationService.WrapWithEquation(input);

            Assert.Contains("\\begin{equation}", result);
            Assert.Contains("x = 1 \\label{1}", result);
            Assert.Contains("\\end{equation}", result);
            Assert.DoesNotContain("align*", result);
        }

        [Fact]
        public void WrapWithCauDS_ShouldWrapInChoiceTF()
        {
            string input = "a) Khang dinh A\nb) Khang dinh B";
            string result = LatexTransformationService.WrapWithCauDS(input);

            Assert.Contains("\\choiceTF[t]", result);
            Assert.Contains("{Khang dinh A}", result);
            Assert.Contains("{Khang dinh B}", result);
        }

        [Fact]
        public void WrapWithImmini_ShouldHandleCenterEnvironment()
        {
            string input = "\\begin{center}\n\\begin{tikzpicture}\n\\end{tikzpicture}\n\\end{center}";
            string result = LatexTransformationService.WrapWithImmini(input, withThm: true);

            Assert.StartsWith("\\immini[thm]{}{", result);
            Assert.Contains("\\begin{tikzpicture}", result);
        }
    }
}

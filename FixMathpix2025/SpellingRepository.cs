using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace FixMathpix2025
{
    /// <summary>
    /// Chịu trách nhiệm nạp và lưu các quy tắc sửa lỗi chính tả từ SpellingCorrections.xml.
    /// Ưu tiên file người dùng tại %AppData%\FixMathpix2025\SpellingCorrections.xml,
    /// nếu không có sẽ fallback về embedded resource nhúng trong assembly.
    /// </summary>
    public class SpellingRepository
    {
        private readonly string _userFilePath;
        private readonly Func<Stream> _getFallbackStream;

        public string UserFilePath => _userFilePath;

        public SpellingRepository(string userFilePath = null, Func<Stream> getFallbackStream = null)
        {
            if (string.IsNullOrEmpty(userFilePath))
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                _userFilePath = Path.Combine(appDataPath, "FixMathpix2025", "SpellingCorrections.xml");
            }
            else
            {
                _userFilePath = userFilePath;
            }

            _getFallbackStream = getFallbackStream ?? DefaultEmbeddedFallbackStream;
        }

        private static Stream DefaultEmbeddedFallbackStream()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = "FixMathpix2025.SpellingCorrections.xml";
            return assembly.GetManifestResourceStream(resourceName);
        }

        public virtual List<CorrectionRule> Load()
        {
            var rules = new List<CorrectionRule>();
            XmlDocument doc = new XmlDocument();

            if (File.Exists(_userFilePath))
            {
                doc.Load(_userFilePath);
            }
            else
            {
                using (Stream stream = _getFallbackStream?.Invoke())
                {
                    if (stream == null)
                    {
                        throw new FileNotFoundException("Không tìm thấy nguồn quy tắc sửa lỗi chính tả mặc định.");
                    }
                    doc.Load(stream);
                }
            }

            XmlNodeList nodes = doc.SelectNodes("//Correction");
            if (nodes != null)
            {
                foreach (XmlNode node in nodes)
                {
                    string find = node.Attributes?["find"]?.Value;
                    string replace = node.Attributes?["replace"]?.Value;
                    if (!string.IsNullOrEmpty(find) && replace != null)
                    {
                        rules.Add(new CorrectionRule { FindText = find, ReplaceText = replace });
                    }
                }
            }

            return rules;
        }

        public virtual Dictionary<string, string> LoadDictionary()
        {
            var dictionary = new Dictionary<string, string>();
            List<CorrectionRule> rules = Load();

            foreach (var rule in rules)
            {
                if (!string.IsNullOrEmpty(rule.FindText) && rule.ReplaceText != null && !dictionary.ContainsKey(rule.FindText))
                {
                    dictionary.Add(rule.FindText, rule.ReplaceText);
                }
            }

            return dictionary;
        }

        public virtual void Save(IEnumerable<CorrectionRule> rules)
        {
            if (rules == null)
                throw new ArgumentNullException(nameof(rules));

            string directory = Path.GetDirectoryName(_userFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.AppendChild(xmlDeclaration);

            XmlElement root = doc.CreateElement("Corrections");
            doc.AppendChild(root);

            foreach (var rule in rules)
            {
                XmlElement correctionNode = doc.CreateElement("Correction");
                correctionNode.SetAttribute("find", rule.FindText ?? string.Empty);
                correctionNode.SetAttribute("replace", rule.ReplaceText ?? string.Empty);
                root.AppendChild(correctionNode);
            }

            doc.Save(_userFilePath);
        }
    }
}

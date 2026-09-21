using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using FixMathpix2025;

namespace FixMathpix2025.Tests
{
    public class PersistenceTests : IDisposable
    {
        private readonly string _tempDir;

        public PersistenceTests()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), "FixMathpix2025_Tests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempDir);
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDir))
            {
                try { Directory.Delete(_tempDir, true); } catch { }
            }
        }

        #region SettingsRepository Tests

        [Fact]
        public void SettingsRepository_MissingFile_ReturnsDefaultSettings()
        {
            string file = Path.Combine(_tempDir, "settings.json");
            var repo = new SettingsRepository(file);

            var settings = repo.Load();

            Assert.NotNull(settings);
            Assert.Equal("Consolas", settings.FontFamily);
            Assert.Equal(14, settings.FontSize);
            Assert.Equal(2, settings.AutoSaveIntervalSeconds);
            Assert.True(settings.IsAutoSaveEnabled);
            Assert.NotNull(settings.HighlightingColors);
            Assert.NotNull(settings.Shortcuts);
            Assert.NotEmpty(settings.Shortcuts);
        }

        [Fact]
        public void SettingsRepository_SaveAndLoad_RoundtripMatches()
        {
            string file = Path.Combine(_tempDir, "settings.json");
            var repo = new SettingsRepository(file);

            var original = new EditorSettings
            {
                FontFamily = "Courier New",
                FontSize = 18,
                AutoSaveIntervalSeconds = 5,
                IsAutoSaveEnabled = false
            };
            original.HighlightingColors["Keyword"] = "#123456";
            original.Shortcuts["ApplicationCommands.Save"] = "Ctrl+Shift+S";

            repo.Save(original);

            var loaded = repo.Load();

            Assert.Equal("Courier New", loaded.FontFamily);
            Assert.Equal(18, loaded.FontSize);
            Assert.Equal(5, loaded.AutoSaveIntervalSeconds);
            Assert.False(loaded.IsAutoSaveEnabled);
            Assert.Equal("#123456", loaded.HighlightingColors["Keyword"]);
            Assert.Equal("Ctrl+Shift+S", loaded.Shortcuts["ApplicationCommands.Save"]);
        }

        [Fact]
        public void SettingsRepository_MalformedJson_ReturnsDefaultSettingsWithoutCrashing()
        {
            string file = Path.Combine(_tempDir, "settings.json");
            File.WriteAllText(file, "{ malformed json content ... ");
            var repo = new SettingsRepository(file);

            var settings = repo.Load();

            Assert.NotNull(settings);
            Assert.Equal("Consolas", settings.FontFamily);
            Assert.Equal(14, settings.FontSize);
        }

        [Fact]
        public void SettingsRepository_LegacyNullShortcuts_RestoresDefaults()
        {
            string file = Path.Combine(_tempDir, "settings.json");
            File.WriteAllText(file, @"{ ""FontFamily"": ""Consolas"", ""Shortcuts"": null }");
            var repo = new SettingsRepository(file);

            var settings = repo.Load();

            Assert.NotNull(settings.Shortcuts);
            Assert.NotEmpty(settings.Shortcuts);
            Assert.True(settings.Shortcuts.ContainsKey("local:CustomCommands.ToggleComment"));
        }

        [Fact]
        public void SettingsRepository_LegacyEmptyShortcuts_RestoresDefaults()
        {
            string file = Path.Combine(_tempDir, "settings.json");
            File.WriteAllText(file, @"{ ""FontFamily"": ""Consolas"", ""Shortcuts"": {} }");
            var repo = new SettingsRepository(file);

            var settings = repo.Load();

            Assert.NotNull(settings.Shortcuts);
            Assert.NotEmpty(settings.Shortcuts);
            Assert.True(settings.Shortcuts.ContainsKey("local:CustomCommands.ToggleComment"));
        }

        [Fact]
        public void SettingsRepository_NullHighlightingColors_ReturnsNonNullEmptyDictionary()
        {
            string file = Path.Combine(_tempDir, "settings.json");
            File.WriteAllText(file, @"{ ""FontFamily"": ""Consolas"", ""HighlightingColors"": null }");
            var repo = new SettingsRepository(file);

            var settings = repo.Load();

            Assert.NotNull(settings.HighlightingColors);
            Assert.Empty(settings.HighlightingColors);
        }

        [Fact]
        public void SettingsRepository_DefaultShortcutsMutability_InstancesAreIndependent()
        {
            string file1 = Path.Combine(_tempDir, "s1.json");
            string file2 = Path.Combine(_tempDir, "s2.json");

            var repo1 = new SettingsRepository(file1);
            var repo2 = new SettingsRepository(file2);

            var s1 = repo1.Load();
            var s2 = repo2.Load();

            s1.Shortcuts["local:CustomCommands.ToggleComment"] = "ModifiedShortcut";

            Assert.NotEqual(s1.Shortcuts["local:CustomCommands.ToggleComment"], s2.Shortcuts["local:CustomCommands.ToggleComment"]);
        }

        [Fact]
        public void SettingsRepository_SaveFailure_ThrowsException()
        {
            string invalidPath = Path.Combine(_tempDir, "non_existent_folder", "sub", "settings.json");
            // Make the parent path a file instead of directory so directory creation fails
            string blockingFile = Path.Combine(_tempDir, "non_existent_folder");
            File.WriteAllText(blockingFile, "blocking file");

            var repo = new SettingsRepository(invalidPath);

            Assert.ThrowsAny<Exception>(() => repo.Save(new EditorSettings()));
        }

        #endregion

        #region SpellingRepository Tests

        [Fact]
        public void SpellingRepository_UserFilePrecedence_LoadsUserFileOverEmbedded()
        {
            string userXml = Path.Combine(_tempDir, "SpellingCorrections.xml");
            File.WriteAllText(userXml, @"<?xml version=""1.0"" encoding=""utf-8""?>
<Corrections>
    <Correction find=""ruleUser"" replace=""ReplacementUser"" />
</Corrections>");

            var repo = new SpellingRepository(userXml);
            var rules = repo.Load();

            Assert.Single(rules);
            Assert.Equal("ruleUser", rules[0].FindText);
            Assert.Equal("ReplacementUser", rules[0].ReplaceText);
        }

        [Fact]
        public void SpellingRepository_EmbeddedFallback_WhenUserFileMissing()
        {
            string missingUserXml = Path.Combine(_tempDir, "Missing_Spelling.xml");
            var repo = new SpellingRepository(missingUserXml);

            var rules = repo.Load();

            Assert.NotNull(rules);
            Assert.NotEmpty(rules); // Should load default embedded rules
        }

        [Fact]
        public void SpellingRepository_SaveAndLoad_RoundtripWithSpecialCharsAndEmptyReplace()
        {
            string userXml = Path.Combine(_tempDir, "SpellingCorrections.xml");
            var repo = new SpellingRepository(userXml);

            var originalRules = new List<CorrectionRule>
            {
                new CorrectionRule("tom & jerry", "cat <and> mouse"),
                new CorrectionRule("deleteMe", "")
            };

            repo.Save(originalRules);

            var loaded = repo.Load();

            Assert.Equal(2, loaded.Count);
            Assert.Equal("tom & jerry", loaded[0].FindText);
            Assert.Equal("cat <and> mouse", loaded[0].ReplaceText);
            Assert.Equal("deleteMe", loaded[1].FindText);
            Assert.Equal("", loaded[1].ReplaceText);
        }

        [Fact]
        public void SpellingRepository_MalformedUserXml_ThrowsException()
        {
            string userXml = Path.Combine(_tempDir, "SpellingCorrections.xml");
            File.WriteAllText(userXml, "<Corrections><Correction find=\"unclosed tag>");

            var repo = new SpellingRepository(userXml);

            Assert.ThrowsAny<Exception>(() => repo.Load());
        }

        #endregion

        #region SearchHistoryRepository Tests

        [Fact]
        public void SearchHistoryRepository_MissingFile_ReturnsEmptyList()
        {
            string historyFile = Path.Combine(_tempDir, "search_history.json");
            var repo = new SearchHistoryRepository(historyFile);

            var list = repo.Load();

            Assert.NotNull(list);
            Assert.Empty(list);
        }

        [Fact]
        public void SearchHistoryRepository_SaveAndLoad_RoundtripPreservesAllFields()
        {
            string historyFile = Path.Combine(_tempDir, "search_history.json");
            var repo = new SearchHistoryRepository(historyFile);

            var items = new List<SearchReplaceItem>
            {
                new SearchReplaceItem
                {
                    FindText = @"\begin{ex}",
                    ReplaceText = @"\begin{bt}",
                    MatchCase = true,
                    WholeWord = false,
                    UseRegex = true
                }
            };

            repo.Save(items);

            var loaded = repo.Load();

            Assert.Single(loaded);
            Assert.Equal(@"\begin{ex}", loaded[0].FindText);
            Assert.Equal(@"\begin{bt}", loaded[0].ReplaceText);
            Assert.True(loaded[0].MatchCase);
            Assert.False(loaded[0].WholeWord);
            Assert.True(loaded[0].UseRegex);
        }

        [Fact]
        public void SearchHistoryRepository_MalformedJson_ThrowsException()
        {
            string historyFile = Path.Combine(_tempDir, "search_history.json");
            File.WriteAllText(historyFile, "{ malformed json }");
            var repo = new SearchHistoryRepository(historyFile);

            Assert.ThrowsAny<Exception>(() => repo.Load());
        }

        [Fact]
        public void SearchHistoryRepository_NullJsonContent_ReturnsEmptyList()
        {
            string historyFile = Path.Combine(_tempDir, "search_history.json");
            File.WriteAllText(historyFile, "null");
            var repo = new SearchHistoryRepository(historyFile);

            var list = repo.Load();

            Assert.NotNull(list);
            Assert.Empty(list);
        }

        #endregion
    }
}

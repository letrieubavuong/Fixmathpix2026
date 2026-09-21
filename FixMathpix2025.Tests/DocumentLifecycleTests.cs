using System;
using System.IO;
using Xunit;

namespace FixMathpix2025.Tests
{
    public class DocumentLifecycleTests
    {
        #region EditorSession Invariants

        [Fact]
        public void EditorSession_InitialState_IsCleanAndHasNoFile()
        {
            var session = new EditorSession();
            Assert.Null(session.CurrentFilePath);
            Assert.False(session.IsDirty);
            Assert.False(session.HasFile);
        }

        [Fact]
        public void EditorSession_MarkOpened_SetsPathAndClearsDirty()
        {
            var session = new EditorSession();
            session.MarkOpened(@"C:\test\sample.tex");

            Assert.Equal(@"C:\test\sample.tex", session.CurrentFilePath);
            Assert.False(session.IsDirty);
            Assert.True(session.HasFile);
        }

        [Fact]
        public void EditorSession_MarkModified_SetsDirtyTrue()
        {
            var session = new EditorSession();
            session.MarkOpened(@"C:\test\sample.tex");
            session.MarkModified();

            Assert.Equal(@"C:\test\sample.tex", session.CurrentFilePath);
            Assert.True(session.IsDirty);
        }

        [Fact]
        public void EditorSession_MarkSaved_UpdatesPathAndClearsDirty()
        {
            var session = new EditorSession();
            session.MarkOpened(@"C:\test\old.tex");
            session.MarkModified();

            session.MarkSaved(@"C:\test\new.tex");
            Assert.Equal(@"C:\test\new.tex", session.CurrentFilePath);
            Assert.False(session.IsDirty);
            Assert.True(session.HasFile);
        }

        [Fact]
        public void EditorSession_MarkClosed_ResetsAllState()
        {
            var session = new EditorSession();
            session.MarkOpened(@"C:\test\sample.tex");
            session.MarkModified();

            session.MarkClosed();
            Assert.Null(session.CurrentFilePath);
            Assert.False(session.IsDirty);
            Assert.False(session.HasFile);
        }

        #endregion

        #region DocumentService File I/O & Encoding

        [Fact]
        public void DocumentService_WriteAndReadText_PreservesExactVietnameseAndLatexContent()
        {
            var service = new DocumentService();
            string tempFile = Path.Combine(Path.GetTempPath(), "test_doc_" + Guid.NewGuid() + ".tex");

            try
            {
                string expectedContent = @"\begin{ex}
Một câu hỏi Vật lí chứa tiếng Việt: $E = mc^2$.
\choice
{Phương án A}
{\True Phương án B}
{Phương án C}
{Phương án D}
\loigiai{Lời giải chi tiết.}
\end{ex}";

                service.WriteText(tempFile, expectedContent);
                string actualContent = service.ReadText(tempFile);

                Assert.Equal(expectedContent, actualContent);
            }
            finally
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }

        [Fact]
        public void DocumentService_ReadText_NonExistentFile_ThrowsFileNotFoundException()
        {
            var service = new DocumentService();
            string nonExistentPath = Path.Combine(Path.GetTempPath(), "non_existent_" + Guid.NewGuid() + ".tex");

            Assert.Throws<FileNotFoundException>(() => service.ReadText(nonExistentPath));
        }

        #endregion

        #region ExternalFileChangeMonitor Logic & Own-Save Suppression

        [Fact]
        public void ExternalFileChangeMonitor_ComputeHash_ReturnsConsistentMD5()
        {
            string content = @"\begin{ex} Test content \end{ex}";
            string hash1 = ExternalFileChangeMonitor.ComputeHash(content);
            string hash2 = ExternalFileChangeMonitor.ComputeHash(content);

            Assert.NotEmpty(hash1);
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void ExternalFileChangeMonitor_EvaluateExternalChange_SuppressesOwnSave()
        {
            string tempFile = Path.Combine(Path.GetTempPath(), "monitor_test_" + Guid.NewGuid() + ".tex");
            File.WriteAllText(tempFile, "initial content");

            try
            {
                using (var monitor = new ExternalFileChangeMonitor())
                {
                    monitor.StartWatching(tempFile);
                    monitor.NotifySaveStarting("new saved content");

                    File.WriteAllText(tempFile, "new saved content");
                    DateTime writeTime = File.GetLastWriteTimeUtc(tempFile);
                    monitor.NotifySaveCompleted(tempFile, writeTime);

                    // Should suppress because save completed set expected save write time
                    bool changeDetected = monitor.EvaluateExternalChange("new saved content", false, out string diskContent);
                    Assert.False(changeDetected);
                }
            }
            finally
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }

        [Fact]
        public void ExternalFileChangeMonitor_EvaluateExternalChange_DetectsRealExternalModification()
        {
            string tempFile = Path.Combine(Path.GetTempPath(), "monitor_test_" + Guid.NewGuid() + ".tex");
            File.WriteAllText(tempFile, "initial content");

            try
            {
                using (var monitor = new ExternalFileChangeMonitor())
                {
                    monitor.StartWatching(tempFile);

                    // Wait slightly to ensure timestamp difference on disk
                    System.Threading.Thread.Sleep(100);
                    File.WriteAllText(tempFile, "externally modified content");

                    bool changeDetected = monitor.EvaluateExternalChange("initial content", true, out string diskContent);
                    Assert.True(changeDetected);
                    Assert.Equal("externally modified content", diskContent);
                }
            }
            finally
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }

        #endregion

        #region AutoSaveService Idempotency & Recovery

        [Fact]
        public void AutoSaveService_EnableDisable_IdempotentBehavior()
        {
            string tempPath = Path.Combine(Path.GetTempPath(), "autosave_" + Guid.NewGuid() + ".tex");
            var service = new AutoSaveService(tempPath);

            int subCount = 0;
            int unsubCount = 0;
            int startTimerCount = 0;
            int stopTimerCount = 0;

            Action sub = () => subCount++;
            Action unsub = () => unsubCount++;
            Action startTimer = () => startTimerCount++;
            Action stopTimer = () => stopTimerCount++;

            // Enable twice
            service.Enable(sub, startTimer);
            service.Enable(sub, startTimer);

            Assert.Equal(1, subCount);
            Assert.Equal(2, startTimerCount);
            Assert.True(service.IsRegistered);
            Assert.True(service.IsTimerRunning);

            // Disable twice
            service.Disable(unsub, stopTimer);
            service.Disable(unsub, stopTimer);

            Assert.Equal(1, unsubCount);
            Assert.Equal(2, stopTimerCount);
            Assert.False(service.IsRegistered);
            Assert.False(service.IsTimerRunning);
        }

        [Fact]
        public void AutoSaveService_SaveAndReadSnapshot_RecoveryWorkflow()
        {
            string tempPath = Path.Combine(Path.GetTempPath(), "autosave_rec_" + Guid.NewGuid() + ".tex");
            var service = new AutoSaveService(tempPath);

            try
            {
                Assert.False(service.HasAutoSaveSnapshot());
                Assert.Null(service.ReadAutoSaveSnapshot());

                string snapshotContent = @"\begin{ex} Recovery snapshot content \end{ex}";
                bool saved = service.SaveSnapshot(snapshotContent);

                Assert.True(saved);
                Assert.True(service.HasAutoSaveSnapshot());
                Assert.Equal(snapshotContent, service.ReadAutoSaveSnapshot());

                service.ClearAutoSaveSnapshot();
                Assert.False(service.HasAutoSaveSnapshot());
            }
            finally
            {
                service.ClearAutoSaveSnapshot();
            }
        }

        [Fact]
        public void AutoSaveService_OnApplicationClosing_DeletesSnapshotIfEnabled()
        {
            string tempPath = Path.Combine(Path.GetTempPath(), "autosave_close_" + Guid.NewGuid() + ".tex");
            var service = new AutoSaveService(tempPath);

            try
            {
                service.SaveSnapshot("draft");
                Assert.True(service.HasAutoSaveSnapshot());

                // If autosave is disabled, closing does NOT delete snapshot
                service.OnApplicationClosing(false);
                Assert.True(service.HasAutoSaveSnapshot());

                // If autosave is enabled, closing deletes snapshot
                service.OnApplicationClosing(true);
                Assert.False(service.HasAutoSaveSnapshot());
            }
            finally
            {
                service.ClearAutoSaveSnapshot();
            }
        }

        #endregion
    }
}

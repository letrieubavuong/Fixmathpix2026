using System;

namespace FixMathpix2025
{
    /// <summary>
    /// Dịch vụ quản lý thứ tự commit và áp dụng cài đặt EditorSettings.
    /// Đảm bảo tính nguyên tử (atomicity) khi lưu cài đặt: persist tệp TRƯỚC,
    /// chỉ khi persist thành công mới cập nhật _currentSettings và áp dụng runtime settings.
    /// </summary>
    public static class SettingsCommitService
    {
        /// <summary>
        /// Thực hiện quy trình Lưu cài đặt: Persist -> Update Current -> Apply Runtime.
        /// Thất bại khi persist sẽ ném exception, không làm biến đổi currentSettings hay runtime settings.
        /// </summary>
        public static bool CommitSave(
            EditorSettings candidate,
            SettingsRepository repository,
            Action<EditorSettings> applyRuntimeAction,
            ref EditorSettings currentSettings)
        {
            if (candidate == null) throw new ArgumentNullException(nameof(candidate));
            if (repository == null) throw new ArgumentNullException(nameof(repository));

            // Persist FIRST
            repository.Save(candidate);

            // Update state & Apply runtime ONLY after persist succeeds
            currentSettings = candidate;
            applyRuntimeAction?.Invoke(candidate);
            return true;
        }

        /// <summary>
        /// Thực hiện quy trình Áp dụng (Apply) cài đặt runtime: Update Current -> Apply Runtime (KHÔNG persist).
        /// </summary>
        public static bool CommitApply(
            EditorSettings candidate,
            Action<EditorSettings> applyRuntimeAction,
            ref EditorSettings currentSettings)
        {
            if (candidate == null) throw new ArgumentNullException(nameof(candidate));

            currentSettings = candidate;
            applyRuntimeAction?.Invoke(candidate);
            return true;
        }
    }
}

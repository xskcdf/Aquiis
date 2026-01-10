using System;

namespace Aquiis.SimpleStart.Shared.Services
{
    public class ToastService
    {
        public event Action<ToastMessage>? OnShow;

        public void ShowSuccess(string message, string? title = null)
        {
            ShowToast(new ToastMessage
            {
                Type = ToastType.Success,
                Title = title ?? "Success",
                Message = message,
                Duration = 15000
            });
        }

        public void ShowError(string message, string? title = null)
        {
            ShowToast(new ToastMessage
            {
                Type = ToastType.Error,
                Title = title ?? "Error",
                Message = message,
                Duration = 17000
            });
        }

        public void ShowWarning(string message, string? title = null)
        {
            ShowToast(new ToastMessage
            {
                Type = ToastType.Warning,
                Title = title ?? "Warning",
                Message = message,
                Duration = 16000
            });
        }

        public void ShowInfo(string message, string? title = null)
        {
            ShowToast(new ToastMessage
            {
                Type = ToastType.Info,
                Title = title ?? "Info",
                Message = message,
                Duration = 15000
            });
        }

        private void ShowToast(ToastMessage message)
        {
            OnShow?.Invoke(message);
        }
    }

    public class ToastMessage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public ToastType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int Duration { get; set; } = 5000;
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

    public enum ToastType
    {
        Success,
        Error,
        Warning,
        Info
    }
}

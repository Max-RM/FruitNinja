using System;
using System.Diagnostics;

namespace Microsoft.Phone.Shell
{
    public enum IdleDetectionMode
    {
        Enabled = 0,
        Disabled = 1
    }

    public sealed class PhoneApplicationService
    {
        private static readonly PhoneApplicationService CurrentInstance = new();
        public static PhoneApplicationService Current => CurrentInstance;
        public IdleDetectionMode ApplicationIdleDetectionMode { get; set; }
    }
}

namespace Microsoft.Phone.Tasks
{
    public enum MarketplaceContentType
    {
        Applications = 0,
        Music = 1,
        ApplicationsAndGames = 1
    }

    public sealed class MarketplaceDetailTask
    {
        public MarketplaceContentType ContentType { get; set; }
        public void Show() { }
    }

    public sealed class WebBrowserTask
    {
        public string URL { get; set; }
        public void Show()
        {
            if (string.IsNullOrWhiteSpace(URL)) return;
            try { Process.Start(new ProcessStartInfo(URL) { UseShellExecute = true }); }
            catch { }
        }
    }

    public sealed class SaveRingtoneTask
    {
        public Uri Source { get; set; }
        public string DisplayName { get; set; }
        public event EventHandler<object> Completed;
        public void Show()
        {
            Completed?.Invoke(this, null);
        }
    }
}

namespace Microsoft.Phone.Info
{
    public static class DeviceExtendedProperties
    {
        public static object GetValue(string property)
        {
            return property switch
            {
                "DeviceManufacturer" => "Unknown",
                "DeviceName" => "Desktop",
                "DeviceFirmwareVersion" => "1.0",
                "DeviceHardwareVersion" => "1.0",
                "DeviceTotalMemory" => (long)1024 * 1024 * 1024,
                _ => "Unknown"
            };
        }
    }
}

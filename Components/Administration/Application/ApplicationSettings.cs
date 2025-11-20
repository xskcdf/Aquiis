namespace Aquiis.SimpleStart.Components.Administration.Application
{
    public class ApplicationSettings
    {
        public string AppName { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Repository { get; set; } = string.Empty;
        public bool SoftDeleteEnabled { get; set; }
        public string SchemaVersion { get; set; } = "1.0.0";
    }
}
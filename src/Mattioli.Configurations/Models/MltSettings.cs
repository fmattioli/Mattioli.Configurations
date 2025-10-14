namespace Mattioli.Configurations.Models
{
    public class MltSettings
    {
        public required string OpenTelemetryColectorUrl { get; set; }
        public required string ApplicationName { get; set; }
        public required string Dsn { get; set; }
        public required double TracesSampleRate { get; set; }
        public required bool AttachStacktrace { get; set; }
        public required string DiagnosticLevel { get; set; }
        public required string Url { get; set; }
    }
}

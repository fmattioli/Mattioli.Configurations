namespace Mattioli.Configurations.Models
{
    public class MltSettings
    {
        public string OpenTelemetryColectorUrl { get; set; }
        public string ApplicationName { get; set; }
        public required string Dsn { get; set; }
        public double TracesSampleRate { get; set; }
        public bool AttachStacktrace { get; set; }
        public string DiagnosticLevel { get; set; }
    }
}

namespace Coderaw.Settings.Models
{
    public record CorsSettings
    {
        public IReadOnlyCollection<string> AllowedIPs { get; init; } = [];
    }
}
namespace Coderaw.Settings.Models
{
    public record CorsSettings
    {
        public List<string> AllowedIPs { get; init; } = [];
    }
}
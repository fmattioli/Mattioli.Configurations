using Feijuca.Auth.Models;

namespace Coderaw.Settings.Models
{
    public record FeijucaAuthSettings(Client Client, Secrets Secrets, ServerSettings ServerSettings, Realm Realm, ClientScopes ClientScopes);
}

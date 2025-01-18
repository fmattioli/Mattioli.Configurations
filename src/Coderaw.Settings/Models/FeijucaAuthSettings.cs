using Feijuca.Auth.Models;

namespace Coderaw.Settings.Models
{
    public class FeijucaAuthSettings
    {
        public Client Client { get; set; }
        public Secrets? Secrets { get; set; }
        public ServerSettings ServerSettings { get; set; }
        public IReadOnlyCollection<Realm> Realms { get; init; }
        public ClientScopes? ClientScopes { get; set; }
    }

}

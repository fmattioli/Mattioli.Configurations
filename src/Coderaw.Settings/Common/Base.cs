using Feijuca.Auth.Models;

namespace Coderaw.Settings.Common
{
    public class Base(User user, Tenant tenant)
    {
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public User User { get; set; } = user;
        public Tenant Tenant { get; set; } = tenant;
    }
}

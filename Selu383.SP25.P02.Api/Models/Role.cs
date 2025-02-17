using Microsoft.AspNetCore.Identity;

namespace Selu383.SP25.P02.Api.Models
{
    public class Role : IdentityRole<int>
    {
        public ICollection<UserRole> UserRoles { get; set; }
    }
}

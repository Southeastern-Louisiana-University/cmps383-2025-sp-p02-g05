using Microsoft.AspNetCore.Identity;
using Selu383.SP25.P02.Api.Models;

namespace Selu383.SP25.P02.Api.Models
{
    public class UserRole : IdentityUserRole<int>
    {
        public ApplicationUser User { get; set; }
        public Role Role { get; set; }
    }
}

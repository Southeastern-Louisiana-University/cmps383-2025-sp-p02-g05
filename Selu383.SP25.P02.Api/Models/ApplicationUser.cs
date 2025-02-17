using Microsoft.AspNetCore.Identity;

namespace Selu383.SP25.P02.Api.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
       public ICollection<UserRole> UserRoles { get; set; } 
    }
}


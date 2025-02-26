using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Selu383.SP25.P02.Api.Features.Users
{
    public class UserDto
    {
        public int Id { get; set; }

        [Required]
        public required string Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
      
        [JsonIgnore]
        public string Password { get; set; }
        public required List<string> Roles { get; set; }
    }

}
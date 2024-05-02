using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace BookLibrary.Model
{
    public class ApplicationUser : IdentityUser
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}

using BookLibrary.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {            
        }
        public DbSet<Books> BooksList { get; set; }
        public DbSet<Student> StudentList { get; set; }
        public DbSet<LibraryRecord> LibraryRecord { get; set; }
       

    }
}

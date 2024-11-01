/*using System.Reflection;
using Microsoft.EntityFrameworkCore;
using first_api_backend.Models;

namespace first_api_backend.Context;

public class ApplicationDbContext : DbContext {
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

  // Add DbSets for your entities
  public DbSet<Company> Companies { get; set; }
}*/

using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using first_api_backend.Models;

namespace first_api_backend.Context
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser> // Ändra här
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

        // Lägg till DbSets för dina entiteter
        public DbSet<Company> Companies { get; set; }

        // Om du har andra modeller kan du lägga till dem här, exempelvis:
        // public DbSet<AnotherModel> AnotherModels { get; set; }
    }
}

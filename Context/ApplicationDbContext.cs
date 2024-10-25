using System.Reflection;
using Microsoft.EntityFrameworkCore;
using first_api_backend.Models;

namespace first_api_backend.Context;

public class ApplicationDbContext : DbContext {
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

  // Add DbSets for your entities
  public DbSet<Company> Companies { get; set; }
}
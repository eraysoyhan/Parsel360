using Microsoft.EntityFrameworkCore;
using Parsel360.API.Models; // User s�n�f� burada

namespace Parsel360.API.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		public DbSet<User> Users { get; set; }
	}
}

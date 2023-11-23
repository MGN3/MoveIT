using Microsoft.EntityFrameworkCore;

namespace MoveIT.Models {
	public class UserContext : DbContext {

		public DbSet<User> Users { get; set; }

		public UserContext(DbContextOptions<UserContext> options) : base(options) { }
		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.Entity<User>(users => {
				users.ToTable("Users");
				users.HasKey(p => p.UserId);
				users.Property(p => p.Name).IsRequired().HasMaxLength(200);
				users.Property(p => p.Email).IsRequired().HasMaxLength(500);
				// Unique index for every email added to de database
				users.HasIndex(p => p.Email).IsUnique();
				users.Property(p => p.Password).IsRequired().HasMaxLength(26);
			});
		}
	}
}

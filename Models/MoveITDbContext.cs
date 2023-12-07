using Microsoft.EntityFrameworkCore;

namespace MoveIT.Models {
	public class MoveITDbContext : DbContext {

		public DbSet<User> Users { get; set; }
		public DbSet<Product> Products { get; set; }

		public MoveITDbContext(DbContextOptions<MoveITDbContext> options) : base(options) { }
		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			//Products
			modelBuilder.Entity<Product>(products => {
				products.ToTable("Product");
				products.HasKey(p => p.ProductId);
				products.Property(p => p.Category).IsRequired().HasMaxLength(200);
				products.Property(p => p.Name).HasMaxLength(500);
				products.Property(p => p.Price).IsRequired();
				products.Property(p => p.UrlImg).IsRequired();//.IsRequired(false) optional vs tasks.Ignore
				products.Property(p => p.Description).IsRequired();
			});
			//Users
			modelBuilder.Entity<User>(users => {
				users.ToTable("Users");
				users.HasKey(p => p.UserId);
				users.Property(p => p.Name).IsRequired().HasMaxLength(200);
				users.Property(p => p.Email).IsRequired().HasMaxLength(254);
				// Unique index for every email added to de database
				users.HasIndex(p => p.Email).IsUnique();
				users.Property(p => p.Password).IsRequired().HasMaxLength(64);
			});
		}
	}
}

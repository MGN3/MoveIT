using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MoveIT {
	public class ProductContext : DbContext {
		public DbSet<Product> Products { get; set;}

		public ProductContext (DbContextOptions<ProductContext> options) : base(options) { }
		protected override void OnModelCreating(ModelBuilder modelBuilder) {

			modelBuilder.Entity<Product>(products => {
				products.ToTable("Product");
				products.HasKey(p => p.ProductId);
				products.Property(p => p.Category).IsRequired().HasMaxLength(200);
				products.Property(p => p.Name).HasMaxLength(500);
				products.Property(p => p.Price).IsRequired();
				products.Property(p => p.UrlImg).IsRequired();//.IsRequired(false) optional vs tasks.Ignore
				products.Property(p => p.Description).IsRequired();
			});

			/*
			 * 	public int Id { get; set; }
		public string Category { get; set; }
		public string Name { get; set; }
		public decimal Price { get; set; }
		public string UrlImg { get; set; }
		public string Description { get; set; }
			 **/
		}
	}
}

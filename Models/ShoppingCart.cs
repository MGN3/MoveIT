using System;
using System.Collections.Generic;

namespace MoveIT.Models {
	public class ShoppingCart {
		public Guid ShoppingCartId { get; set; }
		public DateTime Created { get; set; }
		public decimal TotalPrice { get; set; }

		// list of products
		public List<Product> Products { get; set; }

		// User relation
		public Guid UserId { get; set; }
		public User User { get; set; }

		//add empty constructor.
	}
	
}
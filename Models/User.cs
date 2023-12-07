using System.ComponentModel.DataAnnotations;

namespace MoveIT.Models {
	public class User : IUser {
		public Guid UserId { get; set; }
		public string Name { get; set; }
		[EmailAddress]
		public string Email { get; set; }
		public string Password { get; set; }

		//Entity framework needs a constructor without parameters
		public User() {
		}	
		public User(Guid _userId, string _name, string _email, string _password) {
			UserId = _userId;
			Name = _name;
			Email = _email;
			Password = _password;
		}
	}
}

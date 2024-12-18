﻿namespace UserService.Models
{
	public class User
	{
		public int Id { get; set; } // Primary Key
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string Password { get; set; } // Use hashed password in production
	}
}

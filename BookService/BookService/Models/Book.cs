using System.ComponentModel.DataAnnotations;

namespace BookService.Models
{
	public class Book
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string Title { get; set; }

		[Required]
		public string Author { get; set; }

		public int YearPublished { get; set; }

		public string Genre { get; set; }
	}
}

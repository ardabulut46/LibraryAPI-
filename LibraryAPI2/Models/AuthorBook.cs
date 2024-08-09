using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAPI2.Models
{
    public class AuthorBook
    {
        public int AuthorsId { get; set; }
        public int BooksId {  get; set; }

        [ForeignKey(nameof(AuthorsId))]
        public Author? Author { get; set; }

        [ForeignKey(nameof(BooksId))]
        public Book? Book { get; set; } 
    }
}

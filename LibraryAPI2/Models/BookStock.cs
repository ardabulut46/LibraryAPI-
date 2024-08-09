using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAPI2.Models
{
    public class BookStock
    {
        [Key]
        public int BookId { get; set; }

        public int StockCount { get; set; }

        [ForeignKey(nameof(BookId))]
        public Book? Book { get; set; }

    }
}

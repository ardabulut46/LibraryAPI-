using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAPI2.Models
{
   
    public class BookCopy
    {
        public int Id { get; set; }

        public int? DonatorId { get; set; }
        [ForeignKey(nameof(DonatorId))]
        public Donator? Donator { get; set; }

        public int BookId { get; set; }
        public bool IsAvailable { get; set; }

        [ForeignKey(nameof(BookId))]
        public Book? Book { get; set; }

        
    }
}

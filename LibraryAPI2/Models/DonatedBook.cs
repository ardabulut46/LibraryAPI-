using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAPI2.Models
{
    public class DonatedBook
    {
        public int Id { get; set; }

        public int BookId { get; set; }

        public int? DonatorId { get; set; }

        public string? UserId { get; set; }

        public int HowMany { get; set; } // aynı kitaptan kaç tane 

        [ForeignKey(nameof(DonatorId))]
        public Donator? Donator { get; set; }

        [ForeignKey(nameof(BookId))]
        public Book? Book { get; set; }

        [ForeignKey(nameof(UserId))]
        public AppUser? AppUser { get; set; }
    }
}

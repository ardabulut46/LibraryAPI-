using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using LibraryAPI2.Models;

namespace LibraryAPI2.Models
{
    public class Location
    {
        [Key]
        [StringLength(6, MinimumLength = 3)]
        [Column(TypeName = "varchar(6)")]
        public string Shelf { get; set; } = "";

        public List<Book>? Books { get; set; }

    }
}

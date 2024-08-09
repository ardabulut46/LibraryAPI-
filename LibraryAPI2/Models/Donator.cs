using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LibraryAPI2.Models
{
    public class Donator
    {
        public int Id { get; set; }

        public string? UserId { get; set; }

        public string Name { get; set; } = "";

        public string Email { get; set; } = "";

        public List<BookCopy>? BookCopies { get; set; }

    }
}

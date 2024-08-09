using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAPI2.Models
{
    public class Penalty
    {
        public int Id { get; set; } 

        public string MemberId { get; set; } = "";
        public int BookCopyIdNotReturned { get; set; }
        public int TotalPenalty { get; set; }

        [ForeignKey(nameof(MemberId))]
        public AppUser? Member { get; set; }

        [ForeignKey(nameof(BookCopyIdNotReturned))]
        public BookCopy? BookCopy { get; set; }
    }                
}

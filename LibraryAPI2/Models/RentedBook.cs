using LibraryAPI2.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAPI2.Models
{
    public class RentedBook
    {
        public string MemberId { get; set; } = "";
        public int BookId { get; set; }
        public int CopyId {  get; set; }

        public bool IsReturned { get; set; }

        public DateTime RentDate { get; set; }  

        public DateTime? ReturnDate { get; set; } 
        
        public DateTime MustReturnDate {  get; set; } = DateTime.Now.AddMonths(1);

        [ForeignKey(nameof(CopyId))]
        public BookCopy? BookCopy { get; set; }

        [ForeignKey(nameof(BookId))]
        public Book? Book { get; set; }

        [ForeignKey(nameof(MemberId))]
        public AppUser? AppUser { get; set; }
    }
}

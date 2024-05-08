using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.VisualBasic;

namespace BookLibrary.Model
{
    public class LibraryRecord
    {
        [Key]
        public int LibraryRecordId { get; set; }

        [ForeignKey("Books")]
        public int BookId { get; set; } //book Id
        public Books Books { get; set; }

        [ForeignKey("Student")]
        public int StudentId { get; set; } 
        public Student Student { get; set; }
        public DateTime IssuedDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public decimal FineAmount { get; set; }



    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookLibrary.Model
{
    public class FineRecord
    {
        [Key]
        public int FineId { get; set; }
        public decimal FineAmount { get; set; }
        [ForeignKey(nameof(FineRecord))] 
        public int LibraryRecordId { get; set; }
        public LibraryRecord LibraryRecord { get; set; }
    }
}

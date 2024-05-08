using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookLibrary.Model
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }
        public string SName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        //[ForeignKey("LibraryRecord")]
        //public int RecordId {  get; set; }
        //public LibraryRecord LibraryRecord{  get; set; }
        
    }
}

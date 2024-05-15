using BookLibrary.Data;
using BookLibrary.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibraryRecordCntroller : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public LibraryRecordCntroller(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/LibraryRecord/BorrowBook
        [HttpPost("BorrowBook")]
        public async Task<IActionResult> BorrowBook(int studentId, int bookId)
        {
            var student = await _context.StudentList.FindAsync(studentId);
            if (student == null)
            {
                return NotFound("Student not found");
            }

            var book = await _context.BooksList.FindAsync(bookId);
            if (book == null)
            {
                return NotFound("Book not found");
            }

            var existingLibraryRecord = await _context.LibraryRecord
                 .FirstOrDefaultAsync(lr => lr.BookId == bookId && lr.ReturnDate > DateTime.Now);

            if (existingLibraryRecord != null)
            {
                return BadRequest("Book is already borrowed by another student");
            }

            var libraryRecord = new LibraryRecord
            {
                StudentId = studentId,
                BookId = bookId,
                IssuedDate = DateTime.Now,
                ReturnDate = DateTime.Now.AddDays(14) // Assuming 14 days loan period
            };

            _context.LibraryRecord.Add(libraryRecord);
            await _context.SaveChangesAsync();

            var response = new
            {
                Message = "Book borrowed successfully",
                StudentName = student.SName,
                BookTitle = book.Title
            };

            // Return the response object
            return Ok(response);
        }

        // POST: api/LibraryRecord/ReturnBook
        [HttpPost("ReturnBook")]
        public async Task<IActionResult> ReturnBook(int libraryRecordId)
        {
            var libraryRecord = await _context.LibraryRecord.FindAsync(libraryRecordId);
            if (libraryRecord == null)
            {
                return NotFound("Library record not found");
            }

            libraryRecord.ReturnDate = DateTime.Now;

            // Calculate any fine 
            var fineAmount = CalculateFine(libraryRecord);
            if (fineAmount > 0)
            {
                var fineRecord = new FineRecord
                {
                    FineAmount = fineAmount,
                    LibraryRecordId = libraryRecordId
                };
                _context.FineRecords.Add(fineRecord);
            }

            await _context.SaveChangesAsync();

            return Ok("Book returned successfully");
        }

        // Helper method to calculate fine based on return date and issued date
        private decimal CalculateFine(LibraryRecord libraryRecord)
        {
            const decimal finePerDay = 0.50m; // Fine per day
            var daysLate = (int)(DateTime.Now - libraryRecord.ReturnDate).TotalDays;
            if (daysLate > 0)
            {
                return daysLate * finePerDay;
            }
            return 0;
        }
    }
}

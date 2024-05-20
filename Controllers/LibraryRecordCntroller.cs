using BookLibrary.Data;
using BookLibrary.Model;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
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

            var borrowedBooksCount = await _context.LibraryRecord
                 .CountAsync(lr => lr.StudentId == studentId && lr.ReturnDate > DateTime.Now);
            if (borrowedBooksCount == 2)
            {
                return BadRequest("The student has already borrowed two books");
            }

            if (book.Quantity <= 0)
            {
                return BadRequest("The book is out of stock");
            }

            //var existingLibraryRecord = await _context.LibraryRecord
            //     .FirstOrDefaultAsync(lr => lr.BookId == bookId && lr.ReturnDate > DateTime.Now);

            //if (existingLibraryRecord != null)
            //{
            //    return BadRequest("Book is already borrowed by another student");
            //}

            var libraryRecord = new LibraryRecord
            {
                StudentId = studentId,
                BookId = bookId,
                IssuedDate = DateTime.Now,
                ReturnDate = DateTime.Now.AddDays(14) // Assuming 14 days loan period
            };

            book.Quantity--;
            _context.LibraryRecord.Add(libraryRecord);
            await _context.SaveChangesAsync();
            var response = new
            {
                Message = "Book borrowed successfully",
                StudentName = student.SName,
                BookTitle = book.Title
            };
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
            var book = await _context.BooksList.FindAsync(libraryRecord.BookId);
            if (book == null)
            {
                return NotFound("Book not found");
            }
            libraryRecord.ReturnDate = DateTime.Now;

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
            book.Quantity += 1;

            await _context.SaveChangesAsync();

            return Ok("Book returned successfully");
        }

        // Helper method to calculate fine based on return date and issued date
        private decimal CalculateFine(LibraryRecord libraryRecord)
        {
            const decimal finePerDay = 0.50m;
            var daysLate = (int)(DateTime.Now - libraryRecord.ReturnDate).TotalDays;
            if (daysLate > 0)
            {
                return daysLate * finePerDay;
            }
            return 0;
        }

        [HttpGet("GetBorrowingStudent")]
        public async Task<IActionResult> GetBorrowingStudent(int bookId)
        {
            // Find the library record for the specified bookId
            var libraryRecord = await _context.LibraryRecord
                .Include(lr => lr.Books)
                .FirstOrDefaultAsync(lr => lr.BookId == bookId && lr.ReturnDate > DateTime.Now);

            if (libraryRecord == null)
            {
                return NotFound("No student has borrowed this book");
            }

            if (libraryRecord.Books == null)
            {
                return NotFound("Book information not found");
            }

            // Retrieve the student who borrowed the book
            var student = await _context.StudentList.FindAsync(libraryRecord.StudentId);
            if (student == null)
            {
                return NotFound("Student not found");
            }

            var response = new
            {
                StudentName = student.SName,
                BookTitle = libraryRecord.Books.Title
            };
            return Ok(response);
        }

        [HttpGet("GetAllBorrowingBooks")]
        public async Task<IActionResult> GetAllBorrowingBooks()
        {
            var borrowingRecords = await _context.LibraryRecord
                .Include(lr => lr.Student)
                .Include(lr => lr.Books)
                .Where(lr => lr.ReturnDate > DateTime.Now)
                .ToListAsync();
            if (borrowingRecords == null || !borrowingRecords.Any())
            {
                return NotFound("No borrowing records found.");
            }
            return Ok(borrowingRecords);
        }

    }
}

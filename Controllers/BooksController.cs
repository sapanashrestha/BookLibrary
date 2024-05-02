using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookLibrary.Data;
using BookLibrary.Model;
using Microsoft.AspNetCore.JsonPatch;
using BookLibrary.Services.Implementation;
using BookLibrary.Services.Interface;

namespace BookLibrary.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IBookService _bookService;
        public BooksController(ApplicationDbContext context, IBookService bookService)
        {
            _context = context;
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Books>>> GetBooksList() //return list of books
        {

            if (_bookService.GetAllBooks != null)
            {
                return Ok(await _bookService.GetAllBooks());
            }
            return NotFound();

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Books>> GetBooks(int id) //single book
        {
            var book = await _bookService.GetBooks(id);

            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBooks(int id, Books book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }
            var result = await _bookService.PutBooks(id, book);

            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Books>> PostBooks(Books books)
        {
            _bookService.PostBooks(books);
            return CreatedAtAction("GetBooks", new { id = books.Id }, books);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooks(int id)
        {
            if (await _bookService.DeleteBooks(id))
            {
                return NoContent();

            }
            return BadRequest();
        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchBooks(int id, [FromBody] JsonPatchDocument<Books> patchDoc)
        {
            var book = await _context.BooksList.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            patchDoc.ApplyTo(book, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BooksExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        private bool BooksExists(int id)
        {
            return _context.BooksList.Any(e => e.Id == id);
        }
    }
}

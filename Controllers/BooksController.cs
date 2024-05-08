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
using BookLibrary.Mapper;
using BookLibrary.DTO;
using AutoMapper;

namespace BookLibrary.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;
        public BooksController(ApplicationDbContext context, IBookService bookService, IMapper mapper)
        {
            _context = context;
            _bookService = bookService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetBooksDTO>>> GetBooksList() //return list of books
        {
            var books = await _bookService.GetAllBooks();
            if (books != null)
            {
                var booksDTO = _mapper.Map<IEnumerable<GetBooksDTO>>(books);
                return Ok(booksDTO);
            }
            return NotFound();

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetBooksDTO>> GetBooks(int id) //single book
        {
            var book = await _bookService.GetBooks(id);

            if (book != null)
            {
                var bookDTO = _mapper.Map<GetBooksDTO>(book);
                return Ok(bookDTO);
            }
            return NotFound();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBooks(int id, PutBooksDTO bookDTO)
        {
            if (id != bookDTO.Id)
            {
                return BadRequest();
            }

            var book = _mapper.Map<Books>(bookDTO);

            var result = await _bookService.PutBooks(id, book);

            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<PostBooksDTO>> PostBooks(PostBooksDTO books)
        {
            var bookDTO = _mapper.Map<Books>(books); 
            var createdBook = await _bookService.PostBooks(bookDTO);
            return CreatedAtAction("GetBooks", new { id = createdBook.Id }, bookDTO);
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

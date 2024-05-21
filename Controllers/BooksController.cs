﻿using System;
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
using Microsoft.AspNetCore.Authorization;

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
        // [Authorize]
        public async Task<ActionResult<PostBooksDTO>> PostBooks(PostBooksDTO books)
        {
            if (books == null)
            {
                return BadRequest("Invalid book data.");
            }

            var book = _mapper.Map<Books>(books);

            if (books.Image != null)
            {
                var imageFileName = UploadImage(books.Image);
                book.ImageUrl = Url.Content($"~/images/books/{imageFileName}");
            }

            var createdBook = await _bookService.PostBooks(book);
            var bookToReturn = _mapper.Map<GetBooksDTO>(createdBook);

            return CreatedAtAction("GetBooks", new { id = createdBook.Id }, bookToReturn);
        }

        [HttpPost("BookAtBulk")]
        public async Task<ActionResult<IEnumerable<GetBooksDTO>>> PostBooksList([FromBody] List<PostBooksDTO> books)
        {
            if (books == null || !books.Any())
                return BadRequest("No books provided");

            var bookEntities = _mapper.Map<List<Books>>(books);

            _context.BooksList.AddRange(bookEntities);
            await _context.SaveChangesAsync();

            var createdBookDTOs = _mapper.Map<List<GetBooksDTO>>(bookEntities);

            return CreatedAtAction(nameof(GetBooksList), createdBookDTOs);
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
        private string UploadImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return null;

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/books");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                image.CopyTo(fileStream);
            }

            return uniqueFileName;
        }

        //private void DeleteImage(string fileName)
        //{
        //    if (string.IsNullOrEmpty(fileName))
        //        return;

        //    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/books", fileName);
        //    if (File.Exists(filePath))
        //    {
        //        File.Delete(filePath);
        //    }
        //}

    }
}

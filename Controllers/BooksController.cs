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
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace BookLibrary.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public BooksController(ApplicationDbContext context, IBookService bookService, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _bookService = bookService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

		//[HttpGet]
		//public async Task<ActionResult<IQueryable<GetBooksDTO>>> GetBooksList() //return list of books
		//{
		//    var books = await _bookService.GetAllBooks();
		//    if (books != null)
		//    {
		//        var booksDTO = _mapper.Map<IQueryable<GetBooksDTO>>(books);
		//        return Ok(booksDTO);
		//    }
		//    return NotFound();
		//}

		[HttpGet]
		public async Task<ActionResult> GetBooksList(int pageNumber = 1, int pageSize = 6)
		{
			var totalBooks = await _context.BooksList.CountAsync();
			var totalPages = (int)Math.Ceiling(totalBooks / (double)pageSize);

			var books = await _bookService.GetAllBooks();
			if (books != null)
			{
				var paginatedBooks = books
					.Skip((pageNumber - 1) * pageSize)
					.Take(pageSize)
					.ToList();

				var booksDTO = _mapper.Map<List<GetBooksDTO>>(paginatedBooks);
				var response = new
				{
					Data = booksDTO,
					PageNumber = pageNumber,
					PageSize = pageSize,
					TotalPages = totalPages,
					TotalBooks = totalBooks
				};

				return Ok(response);
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

        [HttpGet("ByTitle/{title}")]
        public async Task<ActionResult<IQueryable<GetBooksDTO>>> GetBooksByTitle(string title)
        {
            var firstCharacter = title.FirstOrDefault(); 

            var booksQuery = _context.BooksList
                .Where(b => b.Title.Contains(title) && b.Title.StartsWith(firstCharacter.ToString()))
                .AsNoTracking();

            if (!await booksQuery.AnyAsync())
            {
                return NotFound();
            }

            var booksDTOQuery = booksQuery.Select(book => _mapper.Map<GetBooksDTO>(book));
            return Ok(booksDTOQuery);
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
        //[Authorize]
        public async Task<ActionResult<PostBooksDTO>> PostBooks(PostBooksDTO books)
        {
            if (books == null)
            {
                return BadRequest("Invalid book data.");
            }
            var existingBook = await _bookService.GetBookByTitleAsync(books.Title);
            if (existingBook != null)
            {
                return Conflict("A book with the same title already exists.");
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

		#region commentedcode
		//[HttpPost("BookAtBulk")]
		//public async Task<ActionResult> PostBooksList([FromForm] List<PostBulkBooksDTO> books)
		//{
		//    if (books == null || !books.Any())
		//        return BadRequest("No books provided");
		//    if (images == null || images.Count != books.Count)
		//        return BadRequest("Number of images and books must match.");
		//    List<Books> bookEntities = new List<Books>();
		//    for (int i = 0; i < books.Count; i++)
		//    {
		//        var bookDTO = books[i];
		//        if (bookDTO == null)
		//            return BadRequest("Invalid book data.");
		//        var book = new Books
		//        {
		//            Title = bookDTO.Title,
		//            Author = bookDTO.Author,
		//            Publication = bookDTO.Publication,
		//            PublicationDate = bookDTO.PublicationDate,
		//            Price = bookDTO.Price,
		//            Quantity = bookDTO.Quantity
		//        };
		//        var image = images[i];
		//        if (image != null)
		//        {
		//            var imageFileName = UploadImage(image);
		//            book.ImageUrl = Url.Content($"~/images/books/{imageFileName}");
		//        }
		//        bookEntities.Add(book);
		//    }
		//    _context.BooksList.AddRange(bookEntities);
		//    await _context.SaveChangesAsync();
		//    var createdBookDTOs = _mapper.Map<List<GetBooksDTO>>(bookEntities);
		//    return CreatedAtAction(nameof(GetBooksList), createdBookDTOs);
		//}
		#endregion

		[HttpPost("BookAtBulk")]
        public async Task<ActionResult> PostBooksList([FromForm] IFormFileCollection images, [FromForm] string booksJson)
        {
            if (string.IsNullOrEmpty(booksJson))
            {
                return BadRequest("No books provided");
            }
            var books = JsonConvert.DeserializeObject<List<PostBulkBooksDTO>>(booksJson);
            if (books == null || !books.Any())
            {
                return BadRequest("No books provided");
            }
            if (images == null || images.Count != books.Count)
            {
                return BadRequest("Number of images and books must match.");
            }
            List<Books> bookEntities = new List<Books>();
            for (int i = 0; i < books.Count; i++)
            {
                var bookDTO = books[i];
                if (bookDTO == null)
                {
                    return BadRequest("Invalid book data.");
                }
                var book = new Books
                {
                    Title = bookDTO.Title,
                    Author = bookDTO.Author,
                    Publication = bookDTO.Publication,
                    PublicationDate = bookDTO.PublicationDate,
                    Price = bookDTO.Price,
                    Quantity = bookDTO.Quantity
                };
                var image = images[i];
                if (image != null)
                {
                    var imageFileName = UploadImage(image);
                    book.ImageUrl = Url.Content($"~/images/books/{imageFileName}");
                }
                bookEntities.Add(book);
            }
            _context.BooksList.AddRange(bookEntities);
            await _context.SaveChangesAsync();
            var createdBookDTOs = _mapper.Map<List<GetBooksDTO>>(bookEntities);
            return CreatedAtAction(nameof(GetBooksList), createdBookDTOs);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooks(int id)
        {
            var book = await _bookService.GetBooks(id);

            if (book == null)
            {
                return NotFound();
            }
            DeleteImage(book.ImageUrl);

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
        private void DeleteImage(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return;
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images/books", Path.GetFileName(imageUrl));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}

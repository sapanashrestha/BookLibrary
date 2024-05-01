using BookLibrary.Model;
using BookLibrary.Services.Interface;
using Microsoft.AspNetCore.JsonPatch;

namespace BookLibrary.Services.Implementation
{
    public class BookService : IBookService
    {
        bool IBookService.DeleteBooks(int id)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Books> IBookService.GetBooks()
        {
            throw new NotImplementedException();
        }

        Books IBookService.GetBooks(int id)
        {
            throw new NotImplementedException();
        }

        bool IBookService.PatchBooks(int id, JsonPatchDocument<Books> patchDoc)
        {
            throw new NotImplementedException();
        }

        Books IBookService.PostBooks(Books book)
        {
            throw new NotImplementedException();
        }

        bool IBookService.PutBooks(int id, Books book)
        {
            throw new NotImplementedException();
        }
    }
}

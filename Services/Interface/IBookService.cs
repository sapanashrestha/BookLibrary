using BookLibrary.Model;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace BookLibrary.Services.Interface
{
    public interface IBookService
    {
        IEnumerable<Books> GetBooks();
        Books GetBooks(int id);
        bool PutBooks(int id, Books book);
        Books PostBooks(Books book);
        bool DeleteBooks(int id);
        bool PatchBooks(int id, [FromBody] JsonPatchDocument<Books> patchDoc);
    }
}

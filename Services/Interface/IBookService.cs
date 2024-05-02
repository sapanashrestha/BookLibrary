using BookLibrary.Model;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace BookLibrary.Services.Interface
{
    public interface IBookService
    {
        Task<IEnumerable<Books>> GetAllBooks();
        Task<Books> GetBooks(int id);
        Task<bool> PutBooks(int id, Books book);
        Task<Books> PostBooks(Books book);
        Task<bool> DeleteBooks(int id);
        bool PatchBooks(int id, [FromBody] JsonPatchDocument<Books> patchDoc);
    }
}

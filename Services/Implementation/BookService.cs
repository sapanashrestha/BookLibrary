using BookLibrary.Model;
using BookLibrary.Repository.Interface;
using BookLibrary.Services.Interface;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace BookLibrary.Services.Implementation
{
    public class BookService : IBookService
    {
        private readonly IGenericRepository<Books>  _genericRepository;
        public BookService(IGenericRepository<Books> genericRepository)
        {
            _genericRepository = genericRepository;
        }
        public async Task<IEnumerable<Books>> GetAllBooks()
        {
            return await _genericRepository.GetAll();
        }
        public async Task<bool> DeleteBooks(int id)
        {
          if(await _genericRepository.Delete(id))
                return true;
          return false;
        }

        public IEnumerable<Books> GetBooks()
        {
            throw new NotImplementedException();
        }

        public async Task<Books> GetBooks(int id)
        {          
            return await _genericRepository.Get(id);

        }

        public bool PatchBooks(int id, [FromBody] JsonPatchDocument<Books> patchDoc)
        {
            throw new NotImplementedException();
        }

        public async Task<Books> PostBooks(Books book)
        {
           return await _genericRepository.Post(book);
        }

        public async Task<bool> PutBooks(int id, Books book)
        {
           if( await _genericRepository.Put(id, book))
                return true;
           return false;
        }
    }
}

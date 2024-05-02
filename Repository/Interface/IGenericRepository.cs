using BookLibrary.Model;

namespace BookLibrary.Repository.Interface
{
    public interface IGenericRepository <T> where T : class
    {
        Task<bool> Delete(int id);
        Task<IEnumerable<T>> GetAll();
        Task<T> Get(int id);
        Task<bool> Put(int id, T entity);
        Task<T> Post(T entity);

    }
}

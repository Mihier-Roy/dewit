using Dewit.Core.Entities;

namespace Dewit.Core.Interfaces
{
    public interface IRepository<T>
        where T : EntityBase
    {
        IEnumerable<T> List();
        T? GetById(int id);
        void Add(T item);
        void Update(T item);
        void Remove(T item);
    }
}

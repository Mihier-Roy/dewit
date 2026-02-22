using Dewit.Core.Entities;
using Dewit.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dewit.Data.Data.Repositories
{
    public class Repository<T> : IRepository<T>
        where T : EntityBase
    {
        private readonly DewitDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(DewitDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public IEnumerable<T> List() => _dbSet.ToList();

        public T? GetById(int id) => _dbSet.Find(id);

        public void Add(T item)
        {
            _dbSet.Add(item);
            _context.SaveChanges();
        }

        public void Update(T item)
        {
            _dbSet.Update(item);
            _context.SaveChanges();
        }

        public void Remove(T item)
        {
            _dbSet.Remove(item);
            _context.SaveChanges();
        }
    }
}
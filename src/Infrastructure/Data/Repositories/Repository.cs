using Microsoft.EntityFrameworkCore;
using Dewit.Core.Entities;
using Dewit.Core.Interfaces;

namespace Dewit.Infrastructure.Data.Repositories
{
	public class Repository<T> : IRepository<T> where T : EntityBase
	{
		private readonly DewitDbContext _dbContext;

		public Repository(DewitDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public T GetById(int id) => _dbContext.Set<T>().Find(id);

		public IEnumerable<T> List() => _dbContext.Set<T>().AsEnumerable();

		public void Add(T item)
		{
			_dbContext.Set<T>().Add(item);
			_dbContext.SaveChanges();
		}

		public void Update(T item)
		{
			_dbContext.Entry(item).State = EntityState.Modified;
			_dbContext.SaveChanges();
		}

		public void Remove(T item)
		{
			_dbContext.Set<T>().Remove(item);
			_dbContext.SaveChanges();
		}
	}
}
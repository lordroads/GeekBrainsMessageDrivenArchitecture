using System.Collections.Concurrent;

namespace Messaging.Interfaces.Impl;

public class InMemoryRepository<T> : IInMemoryRepository<T> where T : class
{
    private readonly ConcurrentBag<T> _repository = new ConcurrentBag<T>();


    public void AddOrUpdate(T entity)
    {
        _repository.Add(entity);
    }

    public IEnumerable<T> GetAll()
    {
        return _repository;
    }
}

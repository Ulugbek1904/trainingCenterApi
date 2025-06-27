﻿using Microsoft.EntityFrameworkCore;

namespace trainingCenter.Infrastructure.brokers.storage
{
    public interface IStorageBroker
    {
        DbSet<T> Set<T>() where T : class;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        public ValueTask<T> InsertAsync<T>(T entity) where T : class;
        public IQueryable<T> SelectAll<T>() where T : class;
        public ValueTask<T> SelectByIdAsync<T>(Guid id) where T : class;
        public ValueTask<T> UpdateAsync<T>(T entity) where T : class;
        public ValueTask<T> DeleteAsync<T>(T entity) where T : class;
        ValueTask<T?> SelectByKeyAsync<T>(params object[] keyValues) where T : class;
        ValueTask<T> SelectByIdAsync<T>(int id) where T : class;
    }
}
using Pantheon.Abstractions.Bases;
using System.Linq.Expressions;

namespace Pantheon.Abstractions.Contracts
{
	public interface IRepository<TEntity, TKey>
		where TEntity : EntityBase<TKey>
		where TKey : IEquatable<TKey>
	{
		Task<IEnumerable<TEntity>> GetAllAsync(ISpecifications<TEntity>? baseSpecifications = null);
		Task<TEntity> GetByIdAsync(TKey id, ISpecifications<TEntity>? baseSpecifications = null);
		Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector, ISpecifications<TEntity>? baseSpecifications = null);
		Task<TResult> GetByIdAsync<TResult>(TKey id, Expression<Func<TEntity, TResult>> selector, ISpecifications<TEntity>? baseSpecifications = null);
		Task<TEntity> GetFirstByExpressionAsync(ISpecifications<TEntity> baseSpecifications);
		Task<TEntity> CreateAsync(TEntity? entity);
		Task UpdateAsync(TEntity? entity);
		Task DeleteAsync(TKey id);
	}
}
